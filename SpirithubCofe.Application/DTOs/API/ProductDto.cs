using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Application.DTOs.API;

/// <summary>
/// Product DTO for API responses
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Product ID
    /// </summary>
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
    /// Additional notes in English
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Additional notes in Arabic
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
    /// Compatibility description in English
    /// </summary>
    public string? Compatibility { get; set; }

    /// <summary>
    /// Compatibility description in Arabic
    /// </summary>
    public string? CompatibilityAr { get; set; }

    /// <summary>
    /// Product uses/applications in English
    /// </summary>
    public string? Uses { get; set; }

    /// <summary>
    /// Product uses/applications in Arabic
    /// </summary>
    public string? UsesAr { get; set; }

    /// <summary>
    /// Whether this product is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is a digital product
    /// </summary>
    public bool IsDigital { get; set; } = false;

    /// <summary>
    /// Whether this product is featured
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Whether this product is organic certified
    /// </summary>
    public bool IsOrganic { get; set; } = false;

    /// <summary>
    /// Whether this product is fair trade certified
    /// </summary>
    public bool IsFairTrade { get; set; } = false;

    /// <summary>
    /// Image alt text for SEO and accessibility
    /// </summary>
    public string? ImageAlt { get; set; }

    /// <summary>
    /// Image alt text in Arabic
    /// </summary>
    public string? ImageAltAr { get; set; }

    /// <summary>
    /// Product launch date
    /// </summary>
    public DateTime? LaunchDate { get; set; }

    /// <summary>
    /// Product expiry or best before date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Sort order for manual product ordering
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Display order for sorting products within a category
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Coffee origin/region
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Tasting notes in English
    /// </summary>
    public string? TastingNotes { get; set; }

    /// <summary>
    /// Tasting notes in Arabic
    /// </summary>
    public string? TastingNotesAr { get; set; }

    /// <summary>
    /// Brewing instructions in English
    /// </summary>
    public string? BrewingInstructions { get; set; }

    /// <summary>
    /// Brewing instructions in Arabic
    /// </summary>
    public string? BrewingInstructionsAr { get; set; }

    /// <summary>
    /// Roast level in English
    /// </summary>
    public string? RoastLevel { get; set; }

    /// <summary>
    /// Roast level in Arabic
    /// </summary>
    public string? RoastLevelAr { get; set; }

    /// <summary>
    /// Processing method in English
    /// </summary>
    public string? Process { get; set; }

    /// <summary>
    /// Processing method in Arabic
    /// </summary>
    public string? ProcessAr { get; set; }

    /// <summary>
    /// Coffee variety in English
    /// </summary>
    public string? Variety { get; set; }

    /// <summary>
    /// Coffee variety in Arabic
    /// </summary>
    public string? VarietyAr { get; set; }

    /// <summary>
    /// Growing altitude in meters
    /// </summary>
    public int? Altitude { get; set; }

    /// <summary>
    /// Farm or origin name in English
    /// </summary>
    public string? Farm { get; set; }

    /// <summary>
    /// Farm or origin name in Arabic
    /// </summary>
    public string? FarmAr { get; set; }

    /// <summary>
    /// Additional meta keywords for SEO
    /// </summary>
    public string? MetaKeywords { get; set; }

    /// <summary>
    /// Product tags (comma-separated)
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// SEO meta title
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// SEO meta description
    /// </summary>
    public string? MetaDescription { get; set; }

    /// <summary>
    /// URL slug for SEO-friendly URLs
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Category ID this product belongs to
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Main product image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    // Legacy fields for backward compatibility
    /// <summary>
    /// Product price in OMR (from default variant)
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Discount price in OMR (from default variant)
    /// </summary>
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// Weight in grams (from default variant)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Weight unit (from default variant)
    /// </summary>
    public string? WeightUnit { get; set; }

    /// <summary>
    /// Stock quantity (from default variant)
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Category information
    /// </summary>
    public CategorySummaryDto Category { get; set; } = new();

    /// <summary>
    /// Product images
    /// </summary>
    public List<ProductImageDto> Images { get; set; } = new();

    /// <summary>
    /// Product variants (if any)
    /// </summary>
    public List<ProductVariantDto> Variants { get; set; } = new();

    /// <summary>
    /// Average rating (1-5 stars)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Product summary DTO for listing and search results
/// </summary>
public class ProductSummaryDto
{
    /// <summary>
    /// Product ID
    /// </summary>
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
    /// Product price in OMR
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Sale price in OMR (if on sale)
    /// </summary>
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Whether the product is on sale
    /// </summary>
    public bool IsOnSale { get; set; }

    /// <summary>
    /// Whether the product is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Primary image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Category slug
    /// </summary>
    public string CategorySlug { get; set; } = string.Empty;

    /// <summary>
    /// Average rating (1-5 stars)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Whether the product is in stock
    /// </summary>
    public bool InStock => StockQuantity > 0;
}

/// <summary>
/// Product image DTO
/// </summary>
public class ProductImageDto
{
    /// <summary>
    /// Image ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full URL to the image
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Alt text for the image
    /// </summary>
    public string? AltText { get; set; }

    /// <summary>
    /// Whether this is the primary image
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Product variant DTO
/// </summary>
public class ProductVariantDto
{
    /// <summary>
    /// Variant ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// SKU specific to this variant
    /// </summary>
    public string VariantSku { get; set; } = string.Empty;

    /// <summary>
    /// Weight of this variant in grams
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Unit of weight (g, kg, etc.)
    /// </summary>
    public string WeightUnit { get; set; } = "g";

    /// <summary>
    /// Regular price in OMR
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Discounted price in OMR (optional)
    /// </summary>
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// Length in centimeters
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// Width in centimeters
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// Height in centimeters
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Low stock threshold for alerts
    /// </summary>
    public int LowStockThreshold { get; set; } = 5;

    /// <summary>
    /// Whether this variant is active and available for purchase
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is the default variant for the product
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Display order for sorting variants
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// When this variant was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this variant was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Check if this variant is in stock
    /// </summary>
    public bool IsInStock => StockQuantity > 0;

    /// <summary>
    /// Check if stock is low
    /// </summary>
    public bool IsLowStock => StockQuantity <= LowStockThreshold && StockQuantity > 0;

    /// <summary>
    /// Get the effective price (discount price if available, otherwise regular price)
    /// </summary>
    public decimal EffectivePrice => DiscountPrice ?? Price;

    /// <summary>
    /// Check if this variant has a discount
    /// </summary>
    public bool HasDiscount => DiscountPrice.HasValue && DiscountPrice < Price;

    /// <summary>
    /// Calculate discount percentage
    /// </summary>
    public decimal DiscountPercentage => HasDiscount ? ((Price - DiscountPrice!.Value) / Price) * 100 : 0;
}

/// <summary>
/// Create product request DTO
/// </summary>
public class CreateProductRequestDto
{
    /// <summary>
    /// Unique SKU for the product
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Product name in English
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product name in Arabic
    /// </summary>
    [StringLength(200)]
    public string? NameAr { get; set; }

    /// <summary>
    /// Product description in English
    /// </summary>
    [StringLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Product description in Arabic
    /// </summary>
    [StringLength(2000)]
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// Product price in OMR
    /// </summary>
    [Required]
    [Range(0.001, 999.999)]
    public decimal Price { get; set; }

    /// <summary>
    /// Sale price in OMR (if on sale)
    /// </summary>
    [Range(0.001, 999.999)]
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Weight in grams (for default variant)
    /// </summary>
    [Range(1, 10000)]
    public decimal Weight { get; set; } = 250;

    /// <summary>
    /// Unit of weight (g, kg, etc.)
    /// </summary>
    [StringLength(10)]
    public string WeightUnit { get; set; } = "g";

    /// <summary>
    /// Discounted price in OMR (optional, for default variant)
    /// </summary>
    [Range(0.001, 999.999)]
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// Category ID
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Whether the product is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the product is featured
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Stock quantity
    /// </summary>
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// Minimum stock level for alerts
    /// </summary>
    [Range(0, int.MaxValue)]
    public int MinStockLevel { get; set; } = 5;

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Roast level
    /// </summary>
    [StringLength(50)]
    public string? RoastLevel { get; set; }

    /// <summary>
    /// Processing method
    /// </summary>
    [StringLength(50)]
    public string? ProcessingMethod { get; set; }

    /// <summary>
    /// Origin country/region
    /// </summary>
    [StringLength(100)]
    public string? Origin { get; set; }

    /// <summary>
    /// Altitude in meters
    /// </summary>
    [Range(0, 5000)]
    public int? Altitude { get; set; }

    /// <summary>
    /// Harvest date
    /// </summary>
    public DateTime? HarvestDate { get; set; }

    /// <summary>
    /// Additional notes in English
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Additional notes in Arabic
    /// </summary>
    [StringLength(1000)]
    public string? NotesAr { get; set; }

    /// <summary>
    /// Aromatic profile description in English
    /// </summary>
    [StringLength(500)]
    public string? AromaticProfile { get; set; }

    /// <summary>
    /// Aromatic profile description in Arabic
    /// </summary>
    [StringLength(500)]
    public string? AromaticProfileAr { get; set; }
}

/// <summary>
/// Update product request DTO
/// </summary>
public class UpdateProductRequestDto : CreateProductRequestDto
{
    // Inherits all properties from CreateProductRequestDto
}

/// <summary>
/// Product search and filter parameters
/// </summary>
public class ProductSearchDto
{
    /// <summary>
    /// Search query (searches in name, description, SKU)
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Category ID filter
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Category slug filter
    /// </summary>
    public string? CategorySlug { get; set; }

    /// <summary>
    /// Minimum price filter
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Roast level filter
    /// </summary>
    public string? RoastLevel { get; set; }

    /// <summary>
    /// Processing method filter
    /// </summary>
    public string? ProcessingMethod { get; set; }

    /// <summary>
    /// Origin filter
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Show only featured products
    /// </summary>
    public bool? FeaturedOnly { get; set; }

    /// <summary>
    /// Show only products on sale
    /// </summary>
    public bool? OnSaleOnly { get; set; }

    /// <summary>
    /// Show only products in stock
    /// </summary>
    public bool? InStockOnly { get; set; } = true;

    /// <summary>
    /// Sort by (name, price, rating, created)
    /// </summary>
    public string? SortBy { get; set; } = "name";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size (max 100)
    /// </summary>
    public int PageSize { get; set; } = 20;
}