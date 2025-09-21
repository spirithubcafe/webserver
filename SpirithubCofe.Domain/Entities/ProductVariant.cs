namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents different variants of a product (different weights, prices, etc.)
/// </summary>
public class ProductVariant
{
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When this variant was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The product this variant belongs to
    /// </summary>
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    
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