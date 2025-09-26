using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Application.DTOs.API;

/// <summary>
/// Simplified Product DTO for API that matches the existing entity structure
/// </summary>
public class SimpleProductDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(2000)]
    public string? Description { get; set; }
    
    [StringLength(2000)]
    public string? DescriptionAr { get; set; }
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    [StringLength(1000)]
    public string? NotesAr { get; set; }
    
    [StringLength(500)]
    public string? AromaticProfile { get; set; }
    
    [StringLength(500)]
    public string? AromaticProfileAr { get; set; }
    
    public int? Intensity { get; set; }
    
    [StringLength(200)]
    public string? Compatibility { get; set; }
    
    [StringLength(200)]
    public string? CompatibilityAr { get; set; }
    
    [StringLength(500)]
    public string? Uses { get; set; }
    
    [StringLength(500)]
    public string? UsesAr { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsDigital { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public bool IsOrganic { get; set; } = false;
    public bool IsFairTrade { get; set; } = false;
    
    [StringLength(200)]
    public string? ImageAlt { get; set; }
    
    [StringLength(200)]
    public string? ImageAltAr { get; set; }
    
    public DateTime? LaunchDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int SortOrder { get; set; }
    public int DisplayOrder { get; set; }
    
    [StringLength(200)]
    public string? Origin { get; set; }
    
    [StringLength(1000)]
    public string? TastingNotes { get; set; }
    
    [StringLength(1000)]
    public string? TastingNotesAr { get; set; }
    
    [StringLength(2000)]
    public string? BrewingInstructions { get; set; }
    
    [StringLength(2000)]
    public string? BrewingInstructionsAr { get; set; }
    
    [StringLength(100)]
    public string? RoastLevel { get; set; }
    
    [StringLength(100)]
    public string? RoastLevelAr { get; set; }
    
    [StringLength(100)]
    public string? Process { get; set; }
    
    [StringLength(100)]
    public string? ProcessAr { get; set; }
    
    [StringLength(100)]
    public string? Variety { get; set; }
    
    [StringLength(100)]
    public string? VarietyAr { get; set; }
    
    public int? Altitude { get; set; }
    
    [StringLength(200)]
    public string? Farm { get; set; }
    
    [StringLength(200)]
    public string? FarmAr { get; set; }
    
    [StringLength(500)]
    public string? MetaKeywords { get; set; }
    
    [StringLength(500)]
    public string? Tags { get; set; }
    
    [StringLength(200)]
    public string? MetaTitle { get; set; }
    
    [StringLength(500)]
    public string? MetaDescription { get; set; }
    
    [StringLength(200)]
    public string? Slug { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties info
    public string? CategoryName { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    // Simplified variant info (just the first/main variant)
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public int StockQuantity { get; set; }
    
    // Main image URL
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Create/Update request for simplified products
/// </summary>
public class CreateSimpleProductRequestDto
{
    [Required]
    [StringLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? NameAr { get; set; }
    
    [StringLength(2000)]
    public string? Description { get; set; }
    
    [StringLength(2000)]
    public string? DescriptionAr { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    // Basic variant info for simple API
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    public decimal? DiscountPrice { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public decimal Weight { get; set; } = 250; // Default 250g
    
    public string WeightUnit { get; set; } = "g";
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    [StringLength(200)]
    public string? Origin { get; set; }
    
    [StringLength(100)]
    public string? RoastLevel { get; set; }
}

/// <summary>
/// Update request for simplified products
/// </summary>
public class UpdateSimpleProductRequestDto : CreateSimpleProductRequestDto
{
}