using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service for managing products in the admin area
/// </summary>
public class ProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with their categories and variants
    /// </summary>
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.MainImage)
            .Include(p => p.GalleryImages)
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .OrderBy(p => p.Category!.DisplayOrder)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    public async Task<List<Product>> GetProductsByCategoryAsync(int? categoryId)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.Category!.DisplayOrder)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Search products by name or SKU
    /// </summary>
    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllProductsAsync();
        }

        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .Where(p => p.Name.Contains(searchTerm) || 
                       p.NameAr!.Contains(searchTerm) || 
                       p.Sku.Contains(searchTerm))
            .OrderBy(p => p.Category!.DisplayOrder)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Get all categories for filtering
    /// </summary>
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .Include(p => p.GalleryImages)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (product == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        // Check if product has any active variants with stock
        var hasStock = product.Variants.Any(v => v.IsActive && v.StockQuantity > 0);
        if (hasStock)
        {
            throw new InvalidOperationException("Cannot delete product that has variants with stock. Please clear stock first.");
        }

        // Note: Reviews, variants, and images will be automatically deleted due to cascade delete configuration
        // No need to manually delete them - EF Core will handle this automatically
        
        try
        {
            // Log what will be deleted for audit purposes
            var reviewCount = product.Reviews.Count;
            var variantCount = product.Variants.Count;
            var imageCount = product.GalleryImages.Count;
            
            _logger.LogInformation("Attempting to delete product '{ProductName}' (ID: {ProductId}) with {ReviewCount} review(s), {VariantCount} variant(s), and {ImageCount} image(s)", 
                product.Name, product.Id, reviewCount, variantCount, imageCount);
            
            // This will automatically cascade delete:
            // - All ProductReviews (configured with DeleteBehavior.Cascade)
            // - All ProductVariants (configured with DeleteBehavior.Cascade)
            // - All ProductImages (configured with DeleteBehavior.Cascade)
            // - Any other related entities with cascade delete configured
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            // Log successful deletion with cascade information
            _logger.LogInformation("Product '{ProductName}' (ID: {ProductId}) deleted successfully along with {ReviewCount} review(s), {VariantCount} variant(s), and {ImageCount} image(s)", 
                product.Name, product.Id, reviewCount, variantCount, imageCount);
            
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to delete product due to database constraints: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    /// <summary>
    /// Toggle product active status
    /// </summary>
    public async Task<bool> ToggleProductStatusAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get total stock for a product (sum of all active variants)
    /// </summary>
    public async Task<int> GetProductTotalStockAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(v => v.ProductId == productId && v.IsActive)
            .SumAsync(v => v.StockQuantity);
    }

    /// <summary>
    /// Get the minimum price for a product (from active variants)
    /// </summary>
    public async Task<decimal?> GetProductMinPriceAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(v => v.ProductId == productId && v.IsActive)
            .MinAsync(v => (decimal?)v.EffectivePrice);
    }

    /// <summary>
    /// Get products with filters
    /// </summary>
    public async Task<List<Product>> GetProductsWithFiltersAsync(
        int? categoryId = null, 
        string? searchTerm = null, 
        string? status = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                   p.NameAr!.Contains(searchTerm) || 
                                   p.Sku.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            switch (status.ToLower())
            {
                case "active":
                    query = query.Where(p => p.IsActive);
                    break;
                case "inactive":
                    query = query.Where(p => !p.IsActive);
                    break;
                case "outofstock":
                    query = query.Where(p => p.Variants.Sum(v => v.StockQuantity) == 0);
                    break;
            }
        }

        return await query
            .OrderBy(p => p.Category!.DisplayOrder)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    public async Task<Product> UpdateProductAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }
}