using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents an item within an order
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    
    /// <summary>
    /// Reference to the order
    /// </summary>
    [Required]
    public int OrderId { get; set; }
    
    /// <summary>
    /// Reference to the product
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    
    /// <summary>
    /// Reference to the product variant (if applicable)
    /// </summary>
    public int? ProductVariantId { get; set; }
    
    /// <summary>
    /// Product name (snapshot at time of order)
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Product variant info (snapshot at time of order)
    /// </summary>
    [MaxLength(255)]
    public string? VariantInfo { get; set; }
    
    /// <summary>
    /// Quantity ordered
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Unit price at time of order
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Tax percentage applied to this item
    /// </summary>
    [Range(0, 100)]
    public decimal TaxPercentage { get; set; }
    
    /// <summary>
    /// Tax amount for this item
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Total amount for this item (quantity * unit price + tax)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    
    // Navigation properties
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; }
}