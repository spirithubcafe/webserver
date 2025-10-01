using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Payment gateway settings for Bank Muscat
/// </summary>
public class PaymentGatewaySettings
{
    public int Id { get; set; }

    /// <summary>
    /// Gateway name (e.g., Bank Muscat)
    /// </summary>
    [MaxLength(100)]
    public string GatewayName { get; set; } = "Bank Muscat";

    /// <summary>
    /// Gateway title for display
    /// </summary>
    [MaxLength(200)]
    public string Title { get; set; } = "Bank Muscat";

    /// <summary>
    /// Gateway description for display
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = "Pay securely by Credit or Debit card or internet banking through Bank Muscat Secure Servers.";

    /// <summary>
    /// Enable/Disable payment gateway
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Merchant ID provided by Bank Muscat
    /// </summary>
    [MaxLength(100)]
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// Access Code provided by Bank Muscat
    /// </summary>
    [MaxLength(200)]
    public string AccessCode { get; set; } = string.Empty;

    /// <summary>
    /// Working Key provided by Bank Muscat (encrypted)
    /// </summary>
    [MaxLength(500)]
    public string WorkingKey { get; set; } = string.Empty;

    /// <summary>
    /// Sandbox mode enabled/disabled
    /// </summary>
    public bool IsSandboxMode { get; set; } = true;

    /// <summary>
    /// Live gateway URL
    /// </summary>
    [MaxLength(500)]
    public string LiveGatewayUrl { get; set; } = "https://secure.checkout.visa.com/payment/";

    /// <summary>
    /// Sandbox gateway URL
    /// </summary>
    [MaxLength(500)]
    public string SandboxGatewayUrl { get; set; } = "https://secure.checkout.visa.com/payment/";

    /// <summary>
    /// Return URL after successful payment
    /// </summary>
    [MaxLength(500)]
    public string ReturnUrl { get; set; } = "/api/payment/callback/success";

    /// <summary>
    /// Cancel URL when payment is cancelled
    /// </summary>
    [MaxLength(500)]
    public string CancelUrl { get; set; } = "/api/payment/callback/cancel";

    /// <summary>
    /// Currency code (OMR for Omani Rial)
    /// </summary>
    [MaxLength(10)]
    public string Currency { get; set; } = "OMR";

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last updated date
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}