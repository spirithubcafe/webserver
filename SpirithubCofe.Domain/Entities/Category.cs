namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a product category with bilingual support
/// </summary>
public class Category
{
    public int Id { get; set; }
    
    /// <summary>
    /// Unique slug for the category (used in URLs)
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Category name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Category name in Arabic
    /// </summary>
    public string? NameAr { get; set; }
    
    /// <summary>
    /// Category description in English
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Category description in Arabic
    /// </summary>
    public string? DescriptionAr { get; set; }
    
    /// <summary>
    /// Path to the category image relative to wwwroot
    /// </summary>
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// Whether the category is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether the category should be displayed on the homepage
    /// </summary>
    public bool IsDisplayedOnHomepage { get; set; } = true;
    
    /// <summary>
    /// Display order for sorting categories
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// When the category was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the category was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Products belonging to this category
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}