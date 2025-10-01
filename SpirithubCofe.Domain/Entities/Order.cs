using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a customer order
/// </summary>
public class Order
{
    public int Id { get; set; }
    
    /// <summary>
    /// Order number (unique identifier)
    /// </summary>
    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// User who placed the order
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Order status (Pending, Processing, Shipped, Delivered, Cancelled)
    /// </summary>
    [Required]
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Payment status (Unpaid, Paid, Failed, Refunded)
    /// </summary>
    [Required]
    public string PaymentStatus { get; set; } = "Unpaid";
    
    /// <summary>
    /// Subtotal amount (before tax and shipping)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal SubTotal { get; set; }
    
    /// <summary>
    /// Tax amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Shipping cost
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal ShippingCost { get; set; }
    
    /// <summary>
    /// Total amount (subtotal + tax + shipping)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Customer's first name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's phone number
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Shipping address line 1
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    /// <summary>
    /// Shipping address line 2 (optional)
    /// </summary>
    [MaxLength(255)]
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Selected country ID
    /// </summary>
    public int CountryId { get; set; }
    
    /// <summary>
    /// Selected city ID
    /// </summary>
    public int CityId { get; set; }
    
    /// <summary>
    /// Postal code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Selected shipping method ID
    /// </summary>
    public int ShippingMethodId { get; set; }
    
    /// <summary>
    /// Shipping tracking number (if available)
    /// </summary>
    [MaxLength(100)]
    public string? TrackingNumber { get; set; }
    
    /// <summary>
    /// Additional notes from customer
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// When the order was placed
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the order was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual Country? Country { get; set; }
    public virtual City? City { get; set; }
    public virtual ShippingMethod? ShippingMethod { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}