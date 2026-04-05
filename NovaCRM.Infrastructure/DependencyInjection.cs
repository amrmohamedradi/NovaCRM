using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Interfaces;
using NovaCRM.Infrastructure.Data;
using NovaCRM.Infrastructure.Handlers;
using NovaCRM.Infrastructure.Repositories;
using NovaCRM.Infrastructure.Services;

namespace NovaCRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

        
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlite(config.GetConnectionString("DefaultConnection")));

        
        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength         = 6;
                opts.Password.RequireDigit           = true;
                opts.Password.RequireUppercase       = true;
                opts.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}



