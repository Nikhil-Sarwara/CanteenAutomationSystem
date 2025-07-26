// In: tests/CanteenAutomation.Tests/Integration/AuthEndpointsTests.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions; // Add this using statement

public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output; // Add this field

    // Update the constructor to accept the output helper
    public AuthEndpointsTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output; // Store the output helper
    }

    [Fact]
    public async Task Register_Login_And_Access_Protected_Endpoint_Successfully()
    {
        // === Step 1: Register a new user ===
        var registerModel = new
        {
            email = $"testuser{Guid.NewGuid()}@example.com",
            username = $"testuser{Guid.NewGuid()}",
            password = "Password123!"
        };
        var registerContent = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");
        var registerResponse = await _client.PostAsync("/api/auth/register", registerContent);
        registerResponse.EnsureSuccessStatusCode();

        // === Step 2: Login with the new user to get a JWT ===
        var loginModel = new { username = registerModel.username, password = registerModel.password };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        loginResponse.EnsureSuccessStatusCode();
        var loginResponseString = await loginResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(loginResponseString);
        var token = jsonDoc.RootElement.GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token));

        // === Step 3: Access a protected endpoint using the JWT ===
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var profileResponse = await _client.GetAsync("/api/auth/profile");

        // --- NEW DEBUGGING CODE ---
        // If the response is not successful, print the details to the test output
        if (!profileResponse.IsSuccessStatusCode)
        {
            var errorContent = await profileResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"ERROR: Failed to access protected endpoint.");
            _output.WriteLine($"Status Code: {profileResponse.StatusCode}");
            _output.WriteLine($"Response Content: {errorContent}");
        }
        // --- END DEBUGGING CODE ---

        // The assertion that is currently failing
        Assert.True(profileResponse.IsSuccessStatusCode);

        var profileContent = await profileResponse.Content.ReadAsStringAsync();
        Assert.Contains(registerModel.username, profileContent);
    }
}