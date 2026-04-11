using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovaCRM.Application.Interfaces;
using NovaCRM.Infrastructure.BackgroundServices;
using NovaCRM.Infrastructure.Data;
using NovaCRM.Infrastructure.Handlers;
using NovaCRM.Infrastructure.Services;

namespace NovaCRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlite(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {

                opts.Password.RequiredLength         = 6;
                opts.Password.RequireDigit           = true;
                opts.Password.RequireUppercase       = true;
                opts.Password.RequireNonAlphanumeric = false;

                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
                opts.Lockout.AllowedForNewUsers      = true;

                opts.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IJwtService, JwtService>();

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = config.GetConnectionString("Redis") ?? "localhost:6379";
            opts.InstanceName = "NovaCRM_";
        });
        
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddSingleton<ITokenBlacklistService, RedisTokenBlacklistService>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddHostedService<FollowUpReminderService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
