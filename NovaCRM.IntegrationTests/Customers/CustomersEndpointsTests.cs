using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using NovaCRM.IntegrationTests.Helpers;

namespace NovaCRM.IntegrationTests.Customers;

public class CustomersEndpointsTests(TestWebAppFactory factory)
    : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Get_customers_without_token_returns_401()
    {
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_customers_as_viewer_returns_200_with_paged_result()
    {
        await AuthHelper.AsViewerAsync(_client);

        var response = await _client.GetAsync("/api/customers?page=1&pageSize=3");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = json.GetProperty("data");
        data.GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
        data.GetProperty("totalCount").GetInt32().Should().BeGreaterThanOrEqualTo(5);
        data.GetProperty("page").GetInt32().Should().Be(1);
        data.GetProperty("pageSize").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task Get_customers_search_filters_results()
    {
        await AuthHelper.AsViewerAsync(_client);

        var response = await _client.GetAsync("/api/customers?search=alice");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var items = json.GetProperty("data").GetProperty("items");

        items.GetArrayLength().Should().BeGreaterThan(0);

        foreach (var item in items.EnumerateArray())
        {
            var name  = item.GetProperty("fullName").GetString() ?? "";
            var email = item.GetProperty("email").GetString() ?? "";
            (name.Contains("alice", StringComparison.OrdinalIgnoreCase) ||
             email.Contains("alice", StringComparison.OrdinalIgnoreCase))
                .Should().BeTrue();
        }
    }

    [Fact]
    public async Task Create_customer_as_sales_returns_201()
    {
        await AuthHelper.AsSalesAsync(_client);

        var response = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Integration Test User",
            email    = $"integration-{Guid.NewGuid()}@test.com",
            status   = "Active"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeTrue();
        json.GetProperty("data").GetProperty("fullName").GetString()
            .Should().Be("Integration Test User");
    }

    [Fact]
    public async Task Create_customer_as_viewer_returns_403()
    {
        await AuthHelper.AsViewerAsync(_client);

        var response = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Should Fail",
            email    = "fail@test.com",
            status   = "Active"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_customer_with_invalid_body_returns_400_with_field_errors()
    {
        await AuthHelper.AsSalesAsync(_client);

        var response = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "",
            email    = "bad",
            status   = "Active"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("success").GetBoolean().Should().BeFalse();
        json.GetProperty("fieldErrors").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_customer_by_id_returns_detail_with_collections()
    {

        await AuthHelper.AsSalesAsync(_client);
        var create = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Detail Test",
            email    = $"detail-{Guid.NewGuid()}@test.com",
            status   = "Active"
        });
        var created = await create.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var id = created.GetProperty("data").GetProperty("id").GetString();

        var response = await _client.GetAsync($"/api/customers/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var data = json.GetProperty("data");
        data.GetProperty("fullName").GetString().Should().Be("Detail Test");
        data.TryGetProperty("contacts", out _).Should().BeTrue();
        data.TryGetProperty("deals", out _).Should().BeTrue();
        data.TryGetProperty("notes", out _).Should().BeTrue();
        data.TryGetProperty("attachments", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Delete_customer_as_admin_returns_200_and_customer_no_longer_visible()
    {

        await AuthHelper.AsAdminAsync(_client);
        var create = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "To Be Deleted",
            email    = $"delete-{Guid.NewGuid()}@test.com",
            status   = "Lead"
        });
        var created = await create.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var id = created.GetProperty("data").GetProperty("id").GetString();

        var deleteResp = await _client.DeleteAsync($"/api/customers/{id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResp = await _client.GetAsync($"/api/customers/{id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_customer_as_sales_returns_403()
    {

        await AuthHelper.AsAdminAsync(_client);
        var create = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Protected",
            email    = $"protected-{Guid.NewGuid()}@test.com",
            status   = "Active"
        });
        var created = await create.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var id = created.GetProperty("data").GetProperty("id").GetString();

        await AuthHelper.AsSalesAsync(_client);
        var deleteResp = await _client.DeleteAsync($"/api/customers/{id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
