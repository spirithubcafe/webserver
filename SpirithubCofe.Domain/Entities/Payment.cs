using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a payment transaction
/// </summary>
public class Payment
{
    public int Id { get; set; }
    
    /// <summary>
    /// Related order ID
    /// </summary>
    [Required]
    public int OrderId { get; set; }
    
    /// <summary>
    /// Payment reference number from gateway
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string PaymentReference { get; set; } = string.Empty;
    
    /// <summary>
    /// Transaction ID from payment gateway
    /// </summary>
    [MaxLength(100)]
    public string? TransactionId { get; set; }
    
    /// <summary>
    /// Payment amount
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Currency code (e.g., OMR, USD)
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "OMR";
    
    /// <summary>
    /// Payment status (Pending, Completed, Failed, Cancelled, Refunded)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Payment method (Credit Card, Debit Card, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
    
    /// <summary>
    /// Payment gateway used (Thawani, Bank Muscat, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Gateway { get; set; } = string.Empty;
    
    /// <summary>
    /// Gateway response data (JSON format)
    /// </summary>
    public string? GatewayResponse { get; set; }
    
    /// <summary>
    /// Error message if payment failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// When the payment was initiated
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the payment status was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the payment was completed (if successful)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual Order? Order { get; set; }
}