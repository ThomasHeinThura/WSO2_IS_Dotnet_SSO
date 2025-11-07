using DistributionManagement.Application.Interfaces;
using DistributionManagement.Application.Services;
using DistributionManagement.Infrastructure.Data;
using DistributionManagement.Infrastructure.ExternalServices;
using DistributionManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DistributionManagement.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (configuration["Database:Provider"] == "PostgreSQL")
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString ?? "Data Source=distribution.db");
            }
        });

        // Add HttpClient with logging
        services.AddHttpClient<IAuthenticationService, WSO2AuthenticationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<WSO2AuthenticationService>>();
            logger.LogInformation("HttpClient configured for WSO2AuthenticationService");
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}
