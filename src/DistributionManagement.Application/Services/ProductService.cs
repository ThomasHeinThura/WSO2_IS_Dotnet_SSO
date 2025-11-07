using DistributionManagement.Application.DTOs;
using DistributionManagement.Application.Interfaces;
using DistributionManagement.Domain.Entities;

namespace DistributionManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto, string username)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            StockQuantity = productDto.StockQuantity,
            Category = productDto.Category,
            Sku = productDto.Sku,
            IsActive = productDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = username
        };

        var createdProduct = await _productRepository.AddAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, ProductDto productDto, string username)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return null;

        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.StockQuantity = productDto.StockQuantity;
        existingProduct.Category = productDto.Category;
        existingProduct.Sku = productDto.Sku;
        existingProduct.IsActive = productDto.IsActive;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        existingProduct.UpdatedBy = username;

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        return MapToDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Category = product.Category,
            Sku = product.Sku,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
