// In: tests/CanteenAutomation.Tests/Integration/OrderEndpointsTests.cs
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using CanteenAutomation.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace CanteenAutomation.Tests.Integration;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrderEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    // A helper to create an authenticated client
    private async Task AuthenticateClientAsync(string role = "User")
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var userName = $"{role.ToLower()}_{Guid.NewGuid()}";
        var user = new IdentityUser(userName) { Email = $"{userName}@example.com" };
        await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, role);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { username = userName, password = "Password123!" });
        var token = (await loginResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task AuthenticatedUser_CanPlaceAValidOrder()
    {
        // Arrange: First, an admin must create a menu item
        await AuthenticateClientAsync("Admin");
        var menuItemDto = new MenuItemDto { Name = "Test Pizza", Price = 15 };
        var postResponse = await _client.PostAsJsonAsync("/api/menu", menuItemDto);
        var createdMenuItem = await postResponse.Content.ReadFromJsonAsync<MenuItemDto>();
        Assert.NotNull(createdMenuItem);

        // Arrange: Now, a regular user places an order
        await AuthenticateClientAsync("User");
        var orderRequest = new
        {
            items = new[]
            {
                new { menuItemId = createdMenuItem.Id, quantity = 2 }
            }
        };

        // Act
        var orderResponse = await _client.PostAsJsonAsync("/api/order", orderRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        var createdOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(createdOrder);
        Assert.Equal(30, createdOrder.TotalAmount); // 2 * 15
        Assert.Equal("Pending", createdOrder.Status);
    }

    [Fact]
    public async Task OrderWithNoItems_IsRejectedWithBadRequest()
    {
        // Arrange
        await AuthenticateClientAsync("User");
        var invalidOrderRequest = new { items = Array.Empty<object>() };

        // Act
        var orderResponse = await _client.PostAsJsonAsync("/api/order", invalidOrderRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, orderResponse.StatusCode);
    }

    [Fact]
    public async Task StaffAndAdmin_CanViewOrders_ButUserIsForbidden()
    {
        // Arrange: An admin creates a menu item
        await AuthenticateClientAsync("Admin");
        var menuItemDto = new MenuItemDto { Name = "Staff Meal", Price = 10 };
        var postResponse = await _client.PostAsJsonAsync("/api/menu", menuItemDto);
        var createdMenuItem = await postResponse.Content.ReadFromJsonAsync<MenuItemDto>();
        Assert.NotNull(createdMenuItem);

        // Arrange: A regular user places an order
        await AuthenticateClientAsync("User");
        var orderRequest = new { items = new[] { new { menuItemId = createdMenuItem.Id, quantity = 1 } } };
        await _client.PostAsJsonAsync("/api/order", orderRequest);

        // --- Act & Assert for different roles ---

        // 1. Staff can view orders
        await AuthenticateClientAsync("Staff");
        var staffGetResponse = await _client.GetAsync("/api/order");
        Assert.Equal(HttpStatusCode.OK, staffGetResponse.StatusCode);

        // 2. Admin can also view orders
        await AuthenticateClientAsync("Admin");
        var adminGetResponse = await _client.GetAsync("/api/order");
        Assert.Equal(HttpStatusCode.OK, adminGetResponse.StatusCode);

        // 3. Regular user is forbidden
        await AuthenticateClientAsync("User");
        var userGetResponse = await _client.GetAsync("/api/order");
        Assert.Equal(HttpStatusCode.Forbidden, userGetResponse.StatusCode);
    }
}