using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public enum ShippingMethodType
{
    Pickup = 1,
    NoolOman = 2,
    Aramex = 3
}

public class ShippingMethod
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    [MaxLength(100)]
    public string? NameAr { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string CarrierCode { get; set; } = "";
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? DescriptionAr { get; set; }
    
    public ShippingMethodType Type { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    
    // Pickup specific settings
    public bool IsFreePickup { get; set; } = true;
    
    // NOOL specific settings
    public string? NoolApiKey { get; set; }
    public string? NoolAccountNumber { get; set; }
    public int DeliveryDays { get; set; } = 1; // Default 1 day for NOOL
    
    // Aramex specific settings
    public string? AramexAccountNumber { get; set; }
    public string? AramexUsername { get; set; }
    public string? AramexPassword { get; set; }
    public string? AramexVersion { get; set; } = "v1.0";
    public string? AramexAccountPin { get; set; }
    public string? AramexAccountEntity { get; set; }
    public string? AramexAccountCountryCode { get; set; } = "OM";
    public string? AramexApiUrl { get; set; } = "https://ws.aramex.net/ShippingAPI.V2/";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ShippingZone> ShippingZones { get; set; } = [];
}