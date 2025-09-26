using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.DTOs.API;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.Services.API;

/// <summary>
/// Service for handling category operations for API
/// </summary>
public interface ICategoryApiService
{
    Task<ApiResponse<PaginatedResponse<CategoryDto>>> GetCategoriesAsync(int page = 1, int pageSize = 20, bool activeOnly = true);
    Task<ApiResponse<List<CategorySummaryDto>>> GetCategorySummariesAsync(bool activeOnly = true);
    Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponse<CategoryDto>> GetCategoryBySlugAsync(string slug);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request);
    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    Task<ApiResponse<bool>> ToggleCategoryStatusAsync(int id);
}

/// <summary>
/// Implementation of category API service
/// </summary>
public class CategoryApiService : ICategoryApiService
{
    private readonly IApplicationDbContext _context;

    public CategoryApiService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResponse<CategoryDto>>> GetCategoriesAsync(int page = 1, int pageSize = 20, bool activeOnly = true)
    {
        try
        {
            // Ensure page and pageSize are valid
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Categories.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(c => c.IsActive);
            }

            var totalItems = await query.CountAsync();
            
            var categories = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Description = c.Description,
                    DescriptionAr = c.DescriptionAr,
                    ImageUrl = !string.IsNullOrEmpty(c.ImagePath) ? $"/uploads/categories/{c.ImagePath}" : null,
                    IsActive = c.IsActive,
                    IsDisplayedOnHomepage = c.IsDisplayedOnHomepage,
                    DisplayOrder = c.DisplayOrder,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            var paginatedResponse = PaginatedResponse<CategoryDto>.Create(categories, page, pageSize, totalItems);
            return ApiResponse<PaginatedResponse<CategoryDto>>.SuccessResponse(paginatedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResponse<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories", ex.Message);
        }
    }

    public async Task<ApiResponse<List<CategorySummaryDto>>> GetCategorySummariesAsync(bool activeOnly = true)
    {
        try
        {
            var query = _context.Categories.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(c => c.IsActive);
            }

            var categories = await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategorySummaryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    ImageUrl = !string.IsNullOrEmpty(c.ImagePath) ? $"/uploads/categories/{c.ImagePath}" : null,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    DisplayOrder = c.DisplayOrder
                })
                .ToListAsync();

            return ApiResponse<List<CategorySummaryDto>>.SuccessResponse(categories);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CategorySummaryDto>>.ErrorResponse("An error occurred while retrieving category summaries", ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Description = c.Description,
                    DescriptionAr = c.DescriptionAr,
                    ImageUrl = !string.IsNullOrEmpty(c.ImagePath) ? $"/uploads/categories/{c.ImagePath}" : null,
                    IsActive = c.IsActive,
                    IsDisplayedOnHomepage = c.IsDisplayedOnHomepage,
                    DisplayOrder = c.DisplayOrder,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("Category not found");
            }

            return ApiResponse<CategoryDto>.SuccessResponse(category);
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category", ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryBySlugAsync(string slug)
    {
        try
        {
            var category = await _context.Categories
                .Where(c => c.Slug == slug)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Description = c.Description,
                    DescriptionAr = c.DescriptionAr,
                    ImageUrl = !string.IsNullOrEmpty(c.ImagePath) ? $"/uploads/categories/{c.ImagePath}" : null,
                    IsActive = c.IsActive,
                    IsDisplayedOnHomepage = c.IsDisplayedOnHomepage,
                    DisplayOrder = c.DisplayOrder,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("Category not found");
            }

            return ApiResponse<CategoryDto>.SuccessResponse(category);
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category", ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request)
    {
        try
        {
            // Check if slug already exists
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == request.Slug);
            
            if (existingCategory != null)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("A category with this slug already exists");
            }

            var category = new Category
            {
                Slug = request.Slug,
                Name = request.Name,
                NameAr = request.NameAr,
                Description = request.Description,
                DescriptionAr = request.DescriptionAr,
                IsActive = request.IsActive,
                IsDisplayedOnHomepage = request.IsDisplayedOnHomepage,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Slug = category.Slug,
                Name = category.Name,
                NameAr = category.NameAr,
                Description = category.Description,
                DescriptionAr = category.DescriptionAr,
                ImageUrl = !string.IsNullOrEmpty(category.ImagePath) ? $"/uploads/categories/{category.ImagePath}" : null,
                IsActive = category.IsActive,
                IsDisplayedOnHomepage = category.IsDisplayedOnHomepage,
                DisplayOrder = category.DisplayOrder,
                ProductCount = 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.ErrorResponse("An error occurred while creating the category", ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("Category not found");
            }

            // Check if slug already exists (excluding current category)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == request.Slug && c.Id != id);
            
            if (existingCategory != null)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("A category with this slug already exists");
            }

            category.Slug = request.Slug;
            category.Name = request.Name;
            category.NameAr = request.NameAr;
            category.Description = request.Description;
            category.DescriptionAr = request.DescriptionAr;
            category.IsActive = request.IsActive;
            category.IsDisplayedOnHomepage = request.IsDisplayedOnHomepage;
            category.DisplayOrder = request.DisplayOrder;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Slug = category.Slug,
                Name = category.Name,
                NameAr = category.NameAr,
                Description = category.Description,
                DescriptionAr = category.DescriptionAr,
                ImageUrl = !string.IsNullOrEmpty(category.ImagePath) ? $"/uploads/categories/{category.ImagePath}" : null,
                IsActive = category.IsActive,
                IsDisplayedOnHomepage = category.IsDisplayedOnHomepage,
                DisplayOrder = category.DisplayOrder,
                ProductCount = await _context.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive),
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            return ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.ErrorResponse("An error occurred while updating the category", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
            {
                return ApiResponse<bool>.ErrorResponse("Category not found");
            }

            // Check if category has products
            if (category.Products.Any())
            {
                return ApiResponse<bool>.ErrorResponse("Cannot delete category that contains products. Please move or delete all products first.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the category", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ToggleCategoryStatusAsync(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return ApiResponse<bool>.ErrorResponse("Category not found");
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = category.IsActive ? "activated" : "deactivated";
            return ApiResponse<bool>.SuccessResponse(true, $"Category {status} successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse("An error occurred while toggling category status", ex.Message);
        }
    }
}