using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    // Navigation properties
    public Product? Product { get; set; }
    public Order? Order { get; set; }
    
    // Foreign key
    [Required]
    public int OrderId { get; set; }
}