using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpirithubCofe.Domain.Entities;

public class ShippingZone
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    [MaxLength(100)]
    public string? NameAr { get; set; }
    
    public int ShippingMethodId { get; set; }
    public ShippingMethod? ShippingMethod { get; set; }
    
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ShippingRate> ShippingRates { get; set; } = [];
}

public class ShippingRate
{
    public int Id { get; set; }
    
    public int ShippingZoneId { get; set; }
    public ShippingZone? ShippingZone { get; set; }
    
    public int CityId { get; set; }
    public City? City { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal Rate { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal MinOrderAmount { get; set; } = 0; // Free shipping threshold
    
    public int EstimatedDays { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}