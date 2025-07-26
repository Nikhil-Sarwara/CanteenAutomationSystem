// In: tests/CanteenAutomation.Tests/Integration/MenuEndpointsTests.cs
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CanteenAutomation.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

public class MenuEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    // private readonly WebApplicationFactory<Program> _factory;
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public MenuEndpointsTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    // A helper method to create a user, assign a role, and return a logged-in HttpClient
    private async Task<HttpClient> CreateAuthenticatedClientAsync(string role = "User")
    {
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var userName = $"test_{role.ToLower()}_{Guid.NewGuid()}";
        var user = new IdentityUser { UserName = userName, Email = $"{userName}@example.com" };
        await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, role);

        // Login to get token
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username = userName, password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();
        var jsonDoc = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var token = jsonDoc.RootElement.GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task NormalUser_CanGetMenu_ButIsForbiddenFromPosting()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("User");

        // Act 1: Get the menu
        var getResponse = await client.GetAsync("/api/menu");

        // Assert 1: Should be successful
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Act 2: Try to post a new menu item
        var newItem = new MenuItemDto { Name = "Forbidden Salad", Price = 9.99m };
        var postResponse = await client.PostAsJsonAsync("/api/menu", newItem);

        // Assert 2: Should be forbidden
        Assert.Equal(HttpStatusCode.Forbidden, postResponse.StatusCode);
    }

    [Fact]
    public async Task AdminUser_CanPostNewMenuItem()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("Admin");
        var newItem = new MenuItemDto { Name = "Admin Burger", Price = 15.00m, Description = "Burger for admins" };

        // Act
        var postResponse = await client.PostAsJsonAsync("/api/menu", newItem);
        _output.WriteLine(await postResponse.Content.ReadAsStringAsync()); // Log response for debugging

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        // Verify the item was actually created by fetching it
        var getResponse = await client.GetAsync("/api/menu");
        var menu = await getResponse.Content.ReadFromJsonAsync<List<MenuItemDto>>();

        Assert.NotNull(menu);
        Assert.Contains(menu, m => m.Name == "Admin Burger");
    }
}