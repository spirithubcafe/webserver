namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Shipping method configuration (Pickup, Nool Oman, Aramex)
/// </summary>
public class ShippingMethod
{
    public int Id { get; set; }

    /// <summary>
    /// Method type: Pickup, NoolOman, Aramex
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Display name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Description in English
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Description in Arabic
    /// </summary>
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// Whether this shipping method is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// API configuration for Aramex (JSON format)
    /// </summary>
    public string? ApiConfiguration { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for Nool rates
    /// </summary>
    public virtual ICollection<NoolShippingRate> NoolRates { get; set; } = new List<NoolShippingRate>();
}