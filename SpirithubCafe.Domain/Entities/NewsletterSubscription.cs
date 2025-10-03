using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Represents a newsletter subscription
/// </summary>
public class NewsletterSubscription
{
    public int Id { get; set; }

    /// <summary>
    /// Subscriber's email address
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Subscriber's name (optional)
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the subscription is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the subscription was created
    /// </summary>
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the subscription was cancelled (if applicable)
    /// </summary>
    public DateTime? UnsubscribedAt { get; set; }

    /// <summary>
    /// Additional metadata (JSON format)
    /// </summary>
    public string? Metadata { get; set; }
}