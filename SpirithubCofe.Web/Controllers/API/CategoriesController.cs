using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpirithubCofe.Application.DTOs.API;
using SpirithubCofe.Application.Services.API;

namespace SpirithubCofe.Web.Controllers.API;

/// <summary>
/// Categories API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryApiService _categoryApiService;

    public CategoriesController(ICategoryApiService categoryApiService)
    {
        _categoryApiService = categoryApiService;
    }

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="activeOnly">Show only active categories (default: true)</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CategoryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CategoryDto>>), 400)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<CategoryDto>>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool activeOnly = true)
    {
        var result = await _categoryApiService.GetCategoriesAsync(page, pageSize, activeOnly);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get category summaries (simplified category info)
    /// </summary>
    /// <param name="activeOnly">Show only active categories (default: true)</param>
    /// <returns>List of category summaries</returns>
    [HttpGet("summaries")]
    [ProducesResponseType(typeof(ApiResponse<List<CategorySummaryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<CategorySummaryDto>>), 400)]
    public async Task<ActionResult<ApiResponse<List<CategorySummaryDto>>>> GetCategorySummaries(
        [FromQuery] bool activeOnly = true)
    {
        var result = await _categoryApiService.GetCategorySummariesAsync(activeOnly);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(int id)
    {
        var result = await _categoryApiService.GetCategoryByIdAsync(id);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Get category by slug
    /// </summary>
    /// <param name="slug">Category slug</param>
    /// <returns>Category details</returns>
    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryBySlug(string slug)
    {
        var result = await _categoryApiService.GetCategoryBySlugAsync(slug);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Create a new category (Admin only)
    /// </summary>
    /// <param name="request">Create category request</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 401)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 403)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Validation failed", errors));
        }

        var result = await _categoryApiService.CreateCategoryAsync(request);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data!.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update an existing category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Update category request</param>
    /// <returns>Updated category</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 401)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 403)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Validation failed", errors));
        }

        var result = await _categoryApiService.UpdateCategoryAsync(id, request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Message == "Category not found")
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Delete a category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 401)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 403)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
    {
        var result = await _categoryApiService.DeleteCategoryAsync(id);
        
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Message == "Category not found")
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Toggle category active status (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:int}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 401)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 403)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleCategoryStatus(int id)
    {
        var result = await _categoryApiService.ToggleCategoryStatusAsync(id);
        
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Message == "Category not found")
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }
}