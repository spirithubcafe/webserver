using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.DTOs.API;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.Services.API;

/// <summary>
/// Simplified service for handling product operations for API
/// </summary>
public interface ISimpleProductApiService
{
    Task<ApiResponse<PaginatedResponse<SimpleProductDto>>> GetProductsAsync(ProductSearchDto searchDto);
    Task<ApiResponse<SimpleProductDto>> GetProductByIdAsync(int id);
    Task<ApiResponse<SimpleProductDto>> GetProductBySkuAsync(string sku);
    Task<ApiResponse<List<SimpleProductDto>>> GetFeaturedProductsAsync(int count = 8);
    Task<ApiResponse<List<SimpleProductDto>>> GetProductsByCategoryAsync(int categoryId, int count = 20);
    Task<ApiResponse<SimpleProductDto>> CreateProductAsync(CreateSimpleProductRequestDto request);
    Task<ApiResponse<SimpleProductDto>> UpdateProductAsync(int id, UpdateSimpleProductRequestDto request);
    Task<ApiResponse<bool>> DeleteProductAsync(int id);
    Task<ApiResponse<bool>> ToggleProductStatusAsync(int id);
    Task<ApiResponse<bool>> UpdateStockAsync(int id, int quantity);
}

/// <summary>
/// Implementation of simplified product API service
/// </summary>
public class SimpleProductApiService : ISimpleProductApiService
{
    private readonly IApplicationDbContext _context;

    public SimpleProductApiService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResponse<SimpleProductDto>>> GetProductsAsync(ProductSearchDto searchDto)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Query))
            {
                query = query.Where(p => p.Name.Contains(searchDto.Query) || 
                                        (p.NameAr != null && p.NameAr.Contains(searchDto.Query)) ||
                                        (p.Sku.Contains(searchDto.Query)));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.CategorySlug))
            {
                query = query.Where(p => p.Category != null && p.Category.Slug == searchDto.CategorySlug);
            }

            if (searchDto.FeaturedOnly.HasValue && searchDto.FeaturedOnly.Value)
            {
                query = query.Where(p => p.IsFeatured);
            }

            // Only show active products by default
            query = query.Where(p => p.IsActive);

            // Apply sorting
            var isDescending = searchDto.SortDirection?.ToLower() == "desc";
            query = searchDto.SortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "created" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "updated" => isDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
                "displayorder" => isDescending ? query.OrderByDescending(p => p.DisplayOrder) : query.OrderBy(p => p.DisplayOrder),
                _ => query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name)
            };

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var productDtos = products.Select(p => MapToSimpleProductDto(p, p.Variants.FirstOrDefault())).ToList();

            var paginatedResponse = new PaginatedResponse<SimpleProductDto>
            {
                Items = productDtos,
                TotalItems = totalCount,
                CurrentPage = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };

            return ApiResponse<PaginatedResponse<SimpleProductDto>>.SuccessResponse(paginatedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResponse<SimpleProductDto>>.ErrorResponse($"Failed to get products: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SimpleProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("Product not found");
            }

            var productDto = MapToSimpleProductDto(product, product.Variants.FirstOrDefault());
            return ApiResponse<SimpleProductDto>.SuccessResponse(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<SimpleProductDto>.ErrorResponse($"Failed to get product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SimpleProductDto>> GetProductBySkuAsync(string sku)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .FirstOrDefaultAsync(p => p.Sku == sku);

            if (product == null)
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("Product not found");
            }

            var productDto = MapToSimpleProductDto(product, product.Variants.FirstOrDefault());
            return ApiResponse<SimpleProductDto>.SuccessResponse(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<SimpleProductDto>.ErrorResponse($"Failed to get product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SimpleProductDto>>> GetFeaturedProductsAsync(int count = 8)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderBy(p => p.DisplayOrder)
                .Take(count)
                .ToListAsync();

            var productDtos = products.Select(p => MapToSimpleProductDto(p, p.Variants.FirstOrDefault())).ToList();
            return ApiResponse<List<SimpleProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SimpleProductDto>>.ErrorResponse($"Failed to get featured products: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SimpleProductDto>>> GetProductsByCategoryAsync(int categoryId, int count = 20)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Where(p => p.IsActive && p.CategoryId == categoryId)
                .OrderBy(p => p.DisplayOrder)
                .Take(count)
                .ToListAsync();

            var productDtos = products.Select(p => MapToSimpleProductDto(p, p.Variants.FirstOrDefault())).ToList();
            return ApiResponse<List<SimpleProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SimpleProductDto>>.ErrorResponse($"Failed to get products by category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SimpleProductDto>> CreateProductAsync(CreateSimpleProductRequestDto request)
    {
        try
        {
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("Category not found");
            }

            // Check if SKU already exists
            if (await _context.Products.AnyAsync(p => p.Sku == request.Sku))
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("SKU already exists");
            }

            var product = new Product
            {
                Sku = request.Sku,
                Name = request.Name,
                NameAr = request.NameAr,
                Description = request.Description,
                DescriptionAr = request.DescriptionAr,
                Notes = request.Notes,
                Origin = request.Origin,
                RoastLevel = request.RoastLevel,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Create a default variant with the pricing info
            var variant = new ProductVariant
            {
                ProductId = product.Id,
                VariantSku = $"{request.Sku}-{request.Weight}{request.WeightUnit}",
                Weight = request.Weight,
                WeightUnit = request.WeightUnit,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                StockQuantity = request.StockQuantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var productDto = MapToSimpleProductDto(product, variant);
            return ApiResponse<SimpleProductDto>.SuccessResponse(productDto, "Product created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SimpleProductDto>.ErrorResponse($"Failed to create product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SimpleProductDto>> UpdateProductAsync(int id, UpdateSimpleProductRequestDto request)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("Product not found");
            }

            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("Category not found");
            }

            // Check if SKU already exists for other products
            if (await _context.Products.AnyAsync(p => p.Sku == request.Sku && p.Id != id))
            {
                return ApiResponse<SimpleProductDto>.ErrorResponse("SKU already exists");
            }

            // Update product
            product.Sku = request.Sku;
            product.Name = request.Name;
            product.NameAr = request.NameAr;
            product.Description = request.Description;
            product.DescriptionAr = request.DescriptionAr;
            product.Notes = request.Notes;
            product.Origin = request.Origin;
            product.RoastLevel = request.RoastLevel;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;
            product.IsFeatured = request.IsFeatured;
            product.UpdatedAt = DateTime.UtcNow;

            // Update the main variant
            var variant = product.Variants.FirstOrDefault();
            if (variant != null)
            {
                variant.VariantSku = $"{request.Sku}-{request.Weight}{request.WeightUnit}";
                variant.Weight = request.Weight;
                variant.WeightUnit = request.WeightUnit;
                variant.Price = request.Price;
                variant.DiscountPrice = request.DiscountPrice;
                variant.StockQuantity = request.StockQuantity;
                variant.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var productDto = MapToSimpleProductDto(product, variant);
            return ApiResponse<SimpleProductDto>.SuccessResponse(productDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SimpleProductDto>.ErrorResponse($"Failed to update product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return ApiResponse<bool>.ErrorResponse("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ToggleProductStatusAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return ApiResponse<bool>.ErrorResponse("Product not found");
            }

            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, $"Product {(product.IsActive ? "activated" : "deactivated")} successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to toggle product status: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateStockAsync(int id, int quantity)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return ApiResponse<bool>.ErrorResponse("Product not found");
            }

            var variant = product.Variants.FirstOrDefault();
            if (variant != null)
            {
                variant.StockQuantity = quantity;
                variant.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return ApiResponse<bool>.SuccessResponse(true, "Stock updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to update stock: {ex.Message}");
        }
    }

    private SimpleProductDto MapToSimpleProductDto(Product product, ProductVariant? variant = null)
    {
        return new SimpleProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            NameAr = product.NameAr,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            Notes = product.Notes,
            NotesAr = product.NotesAr,
            AromaticProfile = product.AromaticProfile,
            AromaticProfileAr = product.AromaticProfileAr,
            Intensity = product.Intensity,
            Compatibility = product.Compatibility,
            CompatibilityAr = product.CompatibilityAr,
            Uses = product.Uses,
            UsesAr = product.UsesAr,
            IsActive = product.IsActive,
            IsDigital = product.IsDigital,
            IsFeatured = product.IsFeatured,
            IsOrganic = product.IsOrganic,
            IsFairTrade = product.IsFairTrade,
            ImageAlt = product.ImageAlt,
            ImageAltAr = product.ImageAltAr,
            LaunchDate = product.LaunchDate,
            ExpiryDate = product.ExpiryDate,
            SortOrder = product.SortOrder,
            DisplayOrder = product.DisplayOrder,
            Origin = product.Origin,
            TastingNotes = product.TastingNotes,
            TastingNotesAr = product.TastingNotesAr,
            BrewingInstructions = product.BrewingInstructions,
            BrewingInstructionsAr = product.BrewingInstructionsAr,
            RoastLevel = product.RoastLevel,
            RoastLevelAr = product.RoastLevelAr,
            Process = product.Process,
            ProcessAr = product.ProcessAr,
            Variety = product.Variety,
            VarietyAr = product.VarietyAr,
            Altitude = product.Altitude,
            Farm = product.Farm,
            FarmAr = product.FarmAr,
            MetaKeywords = product.MetaKeywords,
            Tags = product.Tags,
            MetaTitle = product.MetaTitle,
            MetaDescription = product.MetaDescription,
            Slug = product.Slug,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            AverageRating = product.AverageRating,
            ReviewCount = product.ReviewCount,
            // Variant info
            Price = variant?.Price,
            DiscountPrice = variant?.DiscountPrice,
            Weight = variant?.Weight,
            WeightUnit = variant?.WeightUnit,
            StockQuantity = variant?.StockQuantity ?? 0,
            // Image
            ImageUrl = product.MainImage?.ImagePath
        };
    }
}