using DistributionManagement.Application.Interfaces;
using DistributionManagement.Application.Services;
using DistributionManagement.Infrastructure.Data;
using DistributionManagement.Infrastructure.ExternalServices;
using DistributionManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DistributionManagement.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddHttpClient<IAuthenticationService, WSO2AuthenticationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        return services;
    }
}
