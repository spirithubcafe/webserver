using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service for managing categories in the admin area and frontend
/// </summary>
public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all categories for admin management
    /// </summary>
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get active categories for frontend display
    /// </summary>
    public async Task<List<Category>> GetActiveCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get categories to display on homepage
    /// </summary>
    public async Task<List<Category>> GetHomepageCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive && c.IsDisplayedOnHomepage)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Get category by slug
    /// </summary>
    public async Task<Category?> GetCategoryBySlugAsync(string slug)
    {
        return await _context.Categories
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        // Ensure slug is unique
        var existingSlug = await _context.Categories
            .AnyAsync(c => c.Slug == category.Slug);
        
        if (existingSlug)
        {
            throw new InvalidOperationException($"Category with slug '{category.Slug}' already exists.");
        }

        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        // Ensure slug is unique (excluding current category)
        var existingSlug = await _context.Categories
            .AnyAsync(c => c.Slug == category.Slug && c.Id != category.Id);
        
        if (existingSlug)
        {
            throw new InvalidOperationException($"Category with slug '{category.Slug}' already exists.");
        }

        // Detach any existing tracked entities with the same ID to prevent tracking conflicts
        var trackedEntity = _context.ChangeTracker.Entries<Category>()
            .FirstOrDefault(e => e.Entity.Id == category.Id);
        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
        }

        // Get the existing entity from the database
        var existingCategory = await _context.Categories.FindAsync(category.Id);
        if (existingCategory == null)
        {
            throw new InvalidOperationException($"Category with ID {category.Id} not found.");
        }

        // Update the properties of the tracked entity
        existingCategory.Name = category.Name;
        existingCategory.NameAr = category.NameAr;
        existingCategory.Slug = category.Slug;
        existingCategory.Description = category.Description;
        existingCategory.DescriptionAr = category.DescriptionAr;
        existingCategory.ImagePath = category.ImagePath;
        existingCategory.DisplayOrder = category.DisplayOrder;
        existingCategory.IsActive = category.IsActive;
        existingCategory.IsDisplayedOnHomepage = category.IsDisplayedOnHomepage;
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingCategory;
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (category == null)
        {
            return false;
        }

        // Check if category has products
        if (category.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete category that contains products. Please move or delete the products first.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Toggle category active status
    /// </summary>
    public async Task<bool> ToggleCategoryStatusAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        category.IsActive = !category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Toggle category homepage display
    /// </summary>
    public async Task<bool> ToggleCategoryHomepageDisplayAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        category.IsDisplayedOnHomepage = !category.IsDisplayedOnHomepage;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Search categories by name
    /// </summary>
    public async Task<List<Category>> SearchCategoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllCategoriesAsync();
        }

        return await _context.Categories
            .Where(c => c.Name.Contains(searchTerm) || 
                       (c.NameAr != null && c.NameAr.Contains(searchTerm)) ||
                       c.Slug.Contains(searchTerm))
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get categories with filters
    /// </summary>
    public async Task<List<Category>> GetCategoriesWithFiltersAsync(string? searchTerm = null, string? status = null)
    {
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm) || 
                                   (c.NameAr != null && c.NameAr.Contains(searchTerm)) ||
                                   c.Slug.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            switch (status.ToLower())
            {
                case "active":
                    query = query.Where(c => c.IsActive);
                    break;
                case "inactive":
                    query = query.Where(c => !c.IsActive);
                    break;
                case "homepage":
                    query = query.Where(c => c.IsDisplayedOnHomepage);
                    break;
                case "hidden":
                    query = query.Where(c => !c.IsDisplayedOnHomepage);
                    break;
            }
        }

        return await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Check if slug is available
    /// </summary>
    public async Task<bool> IsSlugAvailableAsync(string slug, int? excludeCategoryId = null)
    {
        return !await _context.Categories
            .AnyAsync(c => c.Slug == slug && (excludeCategoryId == null || c.Id != excludeCategoryId));
    }

    /// <summary>
    /// Generate a unique slug from name
    /// </summary>
    public async Task<string> GenerateUniqueSlugAsync(string name, int? excludeCategoryId = null)
    {
        var baseSlug = GenerateSlug(name);
        var slug = baseSlug;
        var counter = 1;

        while (!await IsSlugAvailableAsync(slug, excludeCategoryId))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    /// <summary>
    /// Generate slug from name
    /// </summary>
    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("#", "")
            .Replace("@", "")
            .Replace("$", "")
            .Replace("%", "")
            .Replace("^", "")
            .Replace("*", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("|", "")
            .Replace("\\", "")
            .Replace("/", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("=", "")
            .Replace("+", "")
            .Replace("_", "-");
    }

    /// <summary>
    /// Reorder categories
    /// </summary>
    public async Task ReorderCategoriesAsync(List<(int CategoryId, int NewOrder)> reorderData)
    {
        foreach (var (categoryId, newOrder) in reorderData)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                category.DisplayOrder = newOrder;
                category.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }
}