namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a coffee product with bilingual support
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    /// <summary>
    /// Unique SKU for the product
    /// </summary>
    public string Sku { get; set; } = string.Empty;
    
    /// <summary>
    /// Product name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product name in Arabic
    /// </summary>
    public string? NameAr { get; set; }
    
    /// <summary>
    /// Product description in English
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Product description in Arabic
    /// </summary>
    public string? DescriptionAr { get; set; }
    
    /// <summary>
    /// Additional notes about the product in English
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Additional notes about the product in Arabic
    /// </summary>
    public string? NotesAr { get; set; }
    
    /// <summary>
    /// Aromatic profile description in English
    /// </summary>
    public string? AromaticProfile { get; set; }
    
    /// <summary>
    /// Aromatic profile description in Arabic
    /// </summary>
    public string? AromaticProfileAr { get; set; }
    
    /// <summary>
    /// Coffee intensity level (1-10)
    /// </summary>
    public int? Intensity { get; set; }
    
    /// <summary>
    /// Compatibility information in English (e.g., "Compatible with Nespresso machines")
    /// </summary>
    public string? Compatibility { get; set; }
    
    /// <summary>
    /// Compatibility information in Arabic
    /// </summary>
    public string? CompatibilityAr { get; set; }
    
    /// <summary>
    /// Usage instructions or recommendations in English
    /// </summary>
    public string? Uses { get; set; }
    
    /// <summary>
    /// Usage instructions or recommendations in Arabic
    /// </summary>
    public string? UsesAr { get; set; }
    
    /// <summary>
    /// Whether the product is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this is a digital product (no physical shipping required)
    /// </summary>
    public bool IsDigital { get; set; } = false;
    
    /// <summary>
    /// Display order for sorting products within a category
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// When the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the product was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Category this product belongs to
    /// </summary>
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    /// <summary>
    /// Main product image
    /// </summary>
    public virtual ProductImage? MainImage { get; set; }
    
    /// <summary>
    /// Gallery images (max 5)
    /// </summary>
    public virtual ICollection<ProductImage> GalleryImages { get; set; } = new List<ProductImage>();
    
    /// <summary>
    /// Product variants with different weights, prices, and stock
    /// </summary>
    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    
    /// <summary>
    /// Customer reviews for this product
    /// </summary>
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    
    /// <summary>
    /// Calculate average rating from approved reviews
    /// </summary>
    public decimal AverageRating => Reviews.Where(r => r.IsApproved).Any() 
        ? (decimal)Reviews.Where(r => r.IsApproved).Average(r => r.Rating) 
        : 0;
    
    /// <summary>
    /// Count of approved reviews
    /// </summary>
    public int ReviewCount => Reviews.Count(r => r.IsApproved);
}