using System.Net.Http.Json;
using System.Text.Json;

namespace NovaCRM.IntegrationTests.Helpers;

public static class AuthHelper
{

    public const string AdminEmail    = "admin@novacrm.com";
    public const string AdminPassword = "Admin@123";
    public const string SalesEmail    = "sales@novacrm.com";
    public const string SalesPassword = "Sales@123";
    public const string ViewerEmail   = "viewer@novacrm.com";
    public const string ViewerPassword= "Viewer@123";

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public static async Task<string> GetTokenAsync(
        HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email, password });

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        return json.GetProperty("data").GetProperty("token").GetString()!;
    }

    public static async Task<HttpClient> AsAdminAsync(HttpClient client)
    {
        var token = await GetTokenAsync(client, AdminEmail, AdminPassword);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<HttpClient> AsSalesAsync(HttpClient client)
    {
        var token = await GetTokenAsync(client, SalesEmail, SalesPassword);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<HttpClient> AsViewerAsync(HttpClient client)
    {
        var token = await GetTokenAsync(client, ViewerEmail, ViewerPassword);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
