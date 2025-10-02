namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Nool Oman shipping rates for different cities
/// </summary>
public class NoolShippingRate
{
    public int Id { get; set; }

    /// <summary>
    /// Reference to the Nool shipping method
    /// </summary>
    public int ShippingMethodId { get; set; }

    /// <summary>
    /// Reference to the city
    /// </summary>
    public int CityId { get; set; }

    /// <summary>
    /// Shipping rate in OMR
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Whether this rate is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ShippingMethod ShippingMethod { get; set; } = null!;
    public virtual City City { get; set; } = null!;
}