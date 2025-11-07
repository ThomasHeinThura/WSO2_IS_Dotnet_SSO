using DistributionManagement.Application.DTOs;
using DistributionManagement.Domain.Entities;

namespace DistributionManagement.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UserInfoDto> GetUserInfoAsync(string accessToken);
    Task<bool> ValidateTokenAsync(string token);
}

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(ProductDto productDto, string username);
    Task<ProductDto?> UpdateProductAsync(Guid id, ProductDto productDto, string username);
    Task<bool> DeleteProductAsync(Guid id);
}

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
