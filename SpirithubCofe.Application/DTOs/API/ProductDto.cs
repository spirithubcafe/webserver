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
    /// Product price in OMR
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Sale price in OMR (if on sale)
    /// </summary>
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Weight in grams
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Roast level (Light, Medium, Dark)
    /// </summary>
    public string? RoastLevel { get; set; }

    /// <summary>
    /// Processing method (Natural, Washed, Honey)
    /// </summary>
    public string? ProcessingMethod { get; set; }

    /// <summary>
    /// Origin country/region
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Altitude in meters
    /// </summary>
    public int? Altitude { get; set; }

    /// <summary>
    /// Harvest date
    /// </summary>
    public DateTime? HarvestDate { get; set; }

    /// <summary>
    /// Whether the product is active and visible
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the product is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Whether the product is on sale
    /// </summary>
    public bool IsOnSale { get; set; }

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Minimum stock level for alerts
    /// </summary>
    public int MinStockLevel { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }

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
    /// Variant name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Variant name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Variant price in OMR
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Weight in grams
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Whether the variant is active
    /// </summary>
    public bool IsActive { get; set; }
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
    /// Weight in grams
    /// </summary>
    [Range(1, 10000)]
    public int Weight { get; set; } = 250;

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