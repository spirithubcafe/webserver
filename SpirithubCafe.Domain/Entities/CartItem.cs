using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    public int? ProductVariantId { get; set; }
    public string? VariantInfo { get; set; } // JSON string for variant details
    
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public Cart? Cart { get; set; }
    
    // Foreign key
    [Required]
    public int CartId { get; set; }
}