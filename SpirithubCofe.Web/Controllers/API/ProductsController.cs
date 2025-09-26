using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using SpirithubCofe.Application.DTOs.API;
using SpirithubCofe.Application.Services.API;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Web.Controllers.API;

/// <summary>
/// API Controller for managing products (simplified version)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ISimpleProductApiService _productService;

    public ProductsController(ISimpleProductApiService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get paginated list of products with search and filtering
    /// </summary>
    /// <param name="query">Search term for product name, name in Arabic, or SKU</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="categorySlug">Filter by category slug</param>
    /// <param name="featuredOnly">Show only featured products</param>
    /// <param name="sortBy">Sort field (name, created, updated, displayorder)</param>
    /// <param name="sortDirection">Sort direction (asc, desc)</param>
    /// <param name="page">Page number (starts from 1)</param>
    /// <param name="pageSize">Items per page (max 100)</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SimpleProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<SimpleProductDto>>>> GetProducts(
        [FromQuery] string? query = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? categorySlug = null,
        [FromQuery] bool? featuredOnly = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = "asc",
        [FromQuery][Range(1, int.MaxValue)] int page = 1,
        [FromQuery][Range(1, 100)] int pageSize = 20)
    {
        var searchDto = new ProductSearchDto
        {
            Query = query,
            CategoryId = categoryId,
            CategorySlug = categorySlug,
            FeaturedOnly = featuredOnly,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        };

        var result = await _productService.GetProductsAsync(searchDto);
        
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SimpleProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<SimpleProductDto>>> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get a specific product by SKU
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <returns>Product details</returns>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ApiResponse<SimpleProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<SimpleProductDto>>> GetProductBySku(string sku)
    {
        var result = await _productService.GetProductBySkuAsync(sku);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get featured products
    /// </summary>
    /// <param name="count">Number of products to return (max 50)</param>
    /// <returns>List of featured products</returns>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResponse<List<SimpleProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<List<SimpleProductDto>>>> GetFeaturedProducts(
        [FromQuery][Range(1, 50)] int count = 8)
    {
        var result = await _productService.GetFeaturedProductsAsync(count);
        return Ok(result);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="count">Number of products to return (max 100)</param>
    /// <returns>List of products in the category</returns>
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<SimpleProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<List<SimpleProductDto>>>> GetProductsByCategory(
        int categoryId,
        [FromQuery][Range(1, 100)] int count = 20)
    {
        var result = await _productService.GetProductsByCategoryAsync(categoryId, count);
        return Ok(result);
    }

    /// <summary>
    /// Create a new product (Admin only)
    /// </summary>
    /// <param name="request">Product creation data</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SimpleProductDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<SimpleProductDto>>> CreateProduct([FromBody] CreateSimpleProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(ApiResponse<SimpleProductDto>.ErrorResponse(string.Join(", ", errors)));
        }

        var result = await _productService.CreateProductAsync(request);
        
        if (result.Success)
            return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Update an existing product (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SimpleProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<SimpleProductDto>>> UpdateProduct(int id, [FromBody] UpdateSimpleProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(ApiResponse<SimpleProductDto>.ErrorResponse(string.Join(", ", errors)));
        }

        var result = await _productService.UpdateProductAsync(id, request);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Toggle product active status (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:int}/toggle-status")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleProductStatus(int id)
    {
        var result = await _productService.ToggleProductStatusAsync(id);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }

    /// <summary>
    /// Update product stock quantity (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Stock update data</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:int}/stock")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStock(int id, [FromBody] UpdateStockRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(ApiResponse<bool>.ErrorResponse(string.Join(", ", errors)));
        }

        var result = await _productService.UpdateStockAsync(id, request.Quantity);
        
        if (result.Success)
            return Ok(result);
        
        if (result.Message == "Product not found")
            return NotFound(result);
        
        return BadRequest(result);
    }
}

/// <summary>
/// Request for updating stock quantity
/// </summary>
public class UpdateStockRequestDto
{
    /// <summary>
    /// New stock quantity
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
    public int Quantity { get; set; }
}