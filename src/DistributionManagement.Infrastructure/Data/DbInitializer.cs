using DistributionManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DistributionManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Products.AnyAsync())
        {
            return;
        }

        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product A", Description = "Description for Product A", Price = 99.99m, StockQuantity = 100, Category = "Category 1", Sku = "SKU001" },
            new() { Id = Guid.NewGuid(), Name = "Product B", Description = "Description for Product B", Price = 149.99m, StockQuantity = 75, Category = "Category 1", Sku = "SKU002" },
            new() { Id = Guid.NewGuid(), Name = "Product C", Description = "Description for Product C", Price = 199.99m, StockQuantity = 50, Category = "Category 2", Sku = "SKU003" },
            new() { Id = Guid.NewGuid(), Name = "Product D", Description = "Description for Product D", Price = 249.99m, StockQuantity = 60, Category = "Category 2", Sku = "SKU004" },
            new() { Id = Guid.NewGuid(), Name = "Product E", Description = "Description for Product E", Price = 299.99m, StockQuantity = 40, Category = "Category 3", Sku = "SKU005" },
            new() { Id = Guid.NewGuid(), Name = "Product F", Description = "Description for Product F", Price = 349.99m, StockQuantity = 30, Category = "Category 3", Sku = "SKU006" },
            new() { Id = Guid.NewGuid(), Name = "Product G", Description = "Description for Product G", Price = 399.99m, StockQuantity = 25, Category = "Category 4", Sku = "SKU007" },
            new() { Id = Guid.NewGuid(), Name = "Product H", Description = "Description for Product H", Price = 449.99m, StockQuantity = 20, Category = "Category 4", Sku = "SKU008" },
            new() { Id = Guid.NewGuid(), Name = "Product I", Description = "Description for Product I", Price = 499.99m, StockQuantity = 15, Category = "Category 5", Sku = "SKU009" },
            new() { Id = Guid.NewGuid(), Name = "Product J", Description = "Description for Product J", Price = 549.99m, StockQuantity = 10, Category = "Category 5", Sku = "SKU010" },
            new() { Id = Guid.NewGuid(), Name = "Product K", Description = "Description for Product K", Price = 599.99m, StockQuantity = 5, Category = "Category 6", Sku = "SKU011" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
