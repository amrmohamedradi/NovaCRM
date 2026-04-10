using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using NovaCRM.IntegrationTests.Helpers;

namespace NovaCRM.IntegrationTests.Auth;

public class AuthEndpointsTests(TestWebAppFactory factory)
    : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Register_with_valid_data_returns_201_with_token()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "New User",
            email    = $"newuser-{Guid.NewGuid()}@example.com",
            password = "Secret1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeTrue();
        json.GetProperty("data").GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_with_duplicate_email_returns_400()
    {

        var email = $"dup-{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new
            { fullName = "First", email, password = "Secret1" });

        var response = await _client.PostAsJsonAsync("/api/auth/register", new
            { fullName = "Second", email, password = "Secret1" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_with_invalid_email_returns_400_with_field_errors()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Alice",
            email    = "not-an-email",
            password = "Secret1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeFalse();
        json.GetProperty("fieldErrors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Login_with_correct_credentials_returns_200_with_token()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email    = AuthHelper.AdminEmail,
            password = AuthHelper.AdminPassword
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeTrue();
        json.GetProperty("data").GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        json.GetProperty("data").GetProperty("role").GetString().Should().Be("Admin");
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email    = AuthHelper.AdminEmail,
            password = "WrongPassword1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_with_unknown_email_returns_401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email    = "nobody@example.com",
            password = "Secret1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_then_reuse_same_token_returns_401()
    {

        var token = await AuthHelper.GetTokenAsync(_client, AuthHelper.SalesEmail, AuthHelper.SalesPassword);
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var before = await _client.GetAsync("/api/customers");
        before.StatusCode.Should().Be(HttpStatusCode.OK);

        var logout = await _client.PostAsync("/api/auth/logout", null);
        logout.StatusCode.Should().Be(HttpStatusCode.OK);

        var after = await _client.GetAsync("/api/customers");
        after.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
