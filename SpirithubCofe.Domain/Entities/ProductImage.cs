namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents an image associated with a product
/// </summary>
public class ProductImage
{
    public int Id { get; set; }
    
    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Path to the image file relative to wwwroot
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Alt text for the image in English
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// Alt text for the image in Arabic
    /// </summary>
    public string? AltTextAr { get; set; }
    
    /// <summary>
    /// Whether this is the main product image
    /// </summary>
    public bool IsMain { get; set; } = false;
    
    /// <summary>
    /// Display order for gallery images
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Image width in pixels
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Image height in pixels
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// When the image was uploaded
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The product this image belongs to
    /// </summary>
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
}