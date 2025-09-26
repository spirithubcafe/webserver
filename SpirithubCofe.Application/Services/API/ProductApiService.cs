using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.DTOs.API;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.Services.API;

/// <summary>
/// Simplified service for handling product operations for API
/// </summary>
public interface IProductApiService
{
    Task<ApiResponse<PaginatedResponse<ProductDto>>> GetProductsAsync(ProductSearchDto searchDto);
    Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
    Task<ApiResponse<ProductDto>> GetProductBySkuAsync(string sku);
    Task<ApiResponse<List<ProductDto>>> GetFeaturedProductsAsync(int count = 8);
    Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId, int count = 20);
    Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequestDto request);
    Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductRequestDto request);
    Task<ApiResponse<bool>> DeleteProductAsync(int id);
    Task<ApiResponse<bool>> ToggleProductStatusAsync(int id);
    Task<ApiResponse<bool>> UpdateStockAsync(int id, int quantity);
}

/// <summary>
/// Implementation of simplified product API service
/// </summary>
public class ProductApiService : IProductApiService
{
    private readonly IApplicationDbContext _context;

    public ProductApiService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResponse<ProductDto>>> GetProductsAsync(ProductSearchDto searchDto)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Include(p => p.GalleryImages)
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

            var productDtos = products.Select(p => MapToProductDto(p)).ToList();

            var paginatedResponse = new PaginatedResponse<ProductDto>
            {
                Items = productDtos,
                TotalItems = totalCount,
                CurrentPage = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };

            return ApiResponse<PaginatedResponse<ProductDto>>.SuccessResponse(paginatedResponse);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResponse<ProductDto>>.ErrorResponse($"Failed to get products: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Include(p => p.GalleryImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductDto>.SuccessResponse(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse($"Failed to get product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductDto>> GetProductBySkuAsync(string sku)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Include(p => p.GalleryImages)
                .FirstOrDefaultAsync(p => p.Sku == sku);

            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductDto>.SuccessResponse(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse($"Failed to get product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductDto>>> GetFeaturedProductsAsync(int count = 8)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Include(p => p.GalleryImages)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderBy(p => p.DisplayOrder)
                .Take(count)
                .ToListAsync();

            var productDtos = products.Select(p => MapToProductDto(p)).ToList();
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResponse($"Failed to get featured products: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId, int count = 20)
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants.Where(v => v.IsActive))
                .Include(p => p.MainImage)
                .Include(p => p.GalleryImages)
                .Where(p => p.IsActive && p.CategoryId == categoryId)
                .OrderBy(p => p.DisplayOrder)
                .Take(count)
                .ToListAsync();

            var productDtos = products.Select(p => MapToProductDto(p)).ToList();
            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResponse($"Failed to get products by category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequestDto request)
    {
        try
        {
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Category not found");
            }

            // Check if SKU already exists
            if (await _context.Products.AnyAsync(p => p.Sku == request.Sku))
            {
                return ApiResponse<ProductDto>.ErrorResponse("SKU already exists");
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

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductDto>.SuccessResponse(productDto, "Product created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse($"Failed to create product: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductRequestDto request)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Product not found");
            }

            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Category not found");
            }

            // Check if SKU already exists for other products
            if (await _context.Products.AnyAsync(p => p.Sku == request.Sku && p.Id != id))
            {
                return ApiResponse<ProductDto>.ErrorResponse("SKU already exists");
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

            var productDto = MapToProductDto(product);
            return ApiResponse<ProductDto>.SuccessResponse(productDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse($"Failed to update product: {ex.Message}");
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

    private ProductDto MapToProductDto(Product product)
    {
        var defaultVariant = product.Variants.FirstOrDefault(v => v.IsDefault) ?? product.Variants.FirstOrDefault();
        
        return new ProductDto
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
            
            // Legacy fields from default variant
            Price = defaultVariant?.Price,
            DiscountPrice = defaultVariant?.DiscountPrice,
            Weight = defaultVariant?.Weight,
            WeightUnit = defaultVariant?.WeightUnit,
            StockQuantity = defaultVariant?.StockQuantity ?? 0,
            
            // Main image
            ImageUrl = product.MainImage?.ImagePath,
            
            // Category summary
            Category = product.Category != null ? new CategorySummaryDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                NameAr = product.Category.NameAr,
                Slug = product.Category.Slug,
                IsActive = product.Category.IsActive
            } : new CategorySummaryDto(),
            
            // All variants list
            Variants = product.Variants.OrderBy(v => v.DisplayOrder).Select(v => new ProductVariantDto
            {
                Id = v.Id,
                VariantSku = v.VariantSku,
                Weight = v.Weight,
                WeightUnit = v.WeightUnit,
                Price = v.Price,
                DiscountPrice = v.DiscountPrice,
                Length = v.Length,
                Width = v.Width,
                Height = v.Height,
                StockQuantity = v.StockQuantity,
                LowStockThreshold = v.LowStockThreshold,
                IsActive = v.IsActive,
                IsDefault = v.IsDefault,
                DisplayOrder = v.DisplayOrder,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            }).ToList(),
            
            // All images list (main image + gallery images)
            Images = new List<ProductImageDto>()
                .Concat(product.MainImage != null ? new[] {
                    new ProductImageDto
                    {
                        Id = product.MainImage.Id,
                        ImageUrl = product.MainImage.ImagePath ?? string.Empty,
                        AltText = product.MainImage.AltText,
                        IsPrimary = true,
                        DisplayOrder = 0
                    }
                } : Array.Empty<ProductImageDto>())
                .Concat(product.GalleryImages.OrderBy(img => img.DisplayOrder).Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImagePath ?? string.Empty,
                    AltText = img.AltText,
                    IsPrimary = false,
                    DisplayOrder = img.DisplayOrder
                }))
                .OrderBy(img => img.DisplayOrder)
                .ToList()
        };
    }
}