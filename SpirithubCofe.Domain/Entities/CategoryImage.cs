namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents an image associated with a category
/// </summary>
public class CategoryImage
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
    /// The category this image belongs to
    /// </summary>
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}