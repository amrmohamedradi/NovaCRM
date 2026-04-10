using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace NovaCRM.IntegrationTests.Helpers;

public class TestWebAppFactory : WebApplicationFactory<Program>
{

    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"novacrm-test-{Guid.NewGuid()}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {

                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}",

                ["JwtSettings:Secret"]                 = "test-secret-key-32-chars-minimum-hmac!!",
                ["JwtSettings:Issuer"]                 = "NovaCRM.API",
                ["JwtSettings:Audience"]               = "NovaCRM.Client",

                ["EmailSettings:Enabled"]              = "false",

                ["RateLimitSettings:AuthPermitLimit"]  = "1000",
                ["RateLimitSettings:ApiPermitLimit"]   = "1000",
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (File.Exists(_dbPath))
            try { File.Delete(_dbPath); } catch {  }
    }
}
