using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Subtotal { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ShippingCost { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Total { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "OMR";
    
    public string? PaymentReference { get; set; }
    public string? ShippingProvider { get; set; }
    public string? TrackingNumber { get; set; }
    
    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ShippingAddress? ShippingAddress { get; set; }
    
    // Foreign key for shipping address
    public int? ShippingAddressId { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Refunded = 5
}