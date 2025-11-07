using DistributionManagement.Application.DTOs;
using DistributionManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DistributionManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "yks_admin,yks_user,yks_test")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products");
            return StatusCode(500, new { message = "Failed to retrieve products" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "yks_admin,yks_user,yks_test")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to retrieve product" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "yks_admin,yks_user")]
    public async Task<IActionResult> Create([FromBody] ProductDto productDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            var product = await _productService.CreateProductAsync(productDto, username);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new { message = "Failed to create product" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "yks_admin,yks_user")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto productDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            var product = await _productService.UpdateProductAsync(id, productDto, username);
            if (product == null)
                return NotFound(new { message = "Product not found" });
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to update product" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "yks_admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { message = "Product not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to delete product" });
        }
    }
}
