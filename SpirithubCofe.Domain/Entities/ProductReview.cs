namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a customer review for a product
/// </summary>
public class ProductReview
{
    public int Id { get; set; }
    
    /// <summary>
    /// Rating from 1 to 5 stars
    /// </summary>
    public int Rating { get; set; }
    
    /// <summary>
    /// Review title in English
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Review title in Arabic
    /// </summary>
    public string? TitleAr { get; set; }
    
    /// <summary>
    /// Review content in English
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// Review content in Arabic
    /// </summary>
    public string? ContentAr { get; set; }
    
    /// <summary>
    /// Customer's name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the review is approved by admin
    /// </summary>
    public bool IsApproved { get; set; } = false;
    
    /// <summary>
    /// Whether the review is featured/highlighted
    /// </summary>
    public bool IsFeatured { get; set; } = false;
    
    /// <summary>
    /// Admin notes about the review
    /// </summary>
    public string? AdminNotes { get; set; }
    
    /// <summary>
    /// ID of the admin who approved/rejected the review
    /// </summary>
    public string? ApprovedByUserId { get; set; }
    
    /// <summary>
    /// When the review was approved
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    
    /// <summary>
    /// When the review was submitted
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the review was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The product this review is for
    /// </summary>
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    
    /// <summary>
    /// User who wrote the review (if registered user)
    /// </summary>
    public string? UserId { get; set; }
}