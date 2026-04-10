using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using NovaCRM.Application;
using NovaCRM.Application.Common;
using NovaCRM.Application.Interfaces;
using NovaCRM.Infrastructure;
using NovaCRM.Infrastructure.Data.Seed;
using NovaCRM.API.Middleware;
using NovaCRM.API.Services;
using Scalar.AspNetCore;
using Serilog;

if (Log.Logger.GetType().Name == "SilentLogger")
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
}

try
{
    Log.Information("Starting NovaCRM API...");

    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.EnvironmentName != "Testing")
    {
        builder.Host.UseSerilog((ctx, services, config) =>
            config.ReadFrom.Configuration(ctx.Configuration)
                  .ReadFrom.Services(services)
                  .Enrich.FromLogContext());
    }

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience            = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey         = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
            };

            opts.Events = new JwtBearerEvents
            {
                OnTokenValidated = async ctx =>
                {
                    var blacklist = ctx.HttpContext.RequestServices
                        .GetRequiredService<ITokenBlacklistService>();

                    var jti = ctx.Principal?
                        .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (jti is not null && await blacklist.IsBlacklistedAsync(jti))
                        ctx.Fail("Token has been revoked. Please log in again.");
                }
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", policy => 
            policy.RequireRole("Admin"));

        options.AddPolicy("CanWriteData", policy => 
            policy.RequireRole("Admin", "Sales", "Manager"));

        options.FallbackPolicy = options.DefaultPolicy; 
    });

    var rl = builder.Configuration.GetSection("RateLimitSettings");
    builder.Services.AddRateLimiter(opts =>
    {

        opts.AddFixedWindowLimiter("auth", o =>
        {
            o.Window              = TimeSpan.FromSeconds(rl.GetValue("AuthWindowSeconds", 60));
            o.PermitLimit         = rl.GetValue("AuthPermitLimit", 10);
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            o.QueueLimit          = 0;
        });

        opts.AddFixedWindowLimiter("api", o =>
        {
            o.Window              = TimeSpan.FromSeconds(rl.GetValue("ApiWindowSeconds", 60));
            o.PermitLimit         = rl.GetValue("ApiPermitLimit", 60);
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            o.QueueLimit          = 2;
        });

        opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        opts.OnRejected = async (ctx, token) =>
        {
            ctx.HttpContext.Response.ContentType = "application/json";
            await ctx.HttpContext.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(
                    "Too many requests. Please slow down and try again shortly."),
                token);
        };
    });

    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
            opts.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter()));

    builder.Services.AddOpenApi();

    var corsOrigins = builder.Configuration
        .GetSection("CorsSettings:AllowedOrigins")
        .Get<string[]>();

    builder.Services.AddCors(opts =>
        opts.AddDefaultPolicy(policy =>
        {
            if (corsOrigins is null or { Length: 0 })
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            else
                policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod();
        }));

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<SecurityHeadersMiddleware>();

    app.UseRateLimiter();

    if (app.Environment.EnvironmentName != "Testing")
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | CorrelationId: {CorrelationId}";
            opts.EnrichDiagnosticContext = (diag, http) =>
            {
                diag.Set("CorrelationId", http.Items["CorrelationId"]?.ToString() ?? "N/A");
                diag.Set("UserAgent",     http.Request.Headers["User-Agent"].FirstOrDefault() ?? "N/A");
                diag.Set("ClientIP",      http.Connection.RemoteIpAddress?.ToString() ?? "N/A");
            };
        });
    }

    app.UseCors();
    app.MapOpenApi();
    app.MapScalarApiReference(opts =>
    {
        opts.Title = "NovaCRM API";
        opts.Theme = ScalarTheme.Purple;
    });
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    await DataSeeder.SeedAsync(app.Services);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "NovaCRM API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
