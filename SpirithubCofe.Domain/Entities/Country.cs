using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    [MaxLength(100)]
    public string? NameAr { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Code { get; set; } = ""; // ISO 3166-1 alpha-3 (OMN, ARE, SAU, etc.)
    
    [Required]
    [MaxLength(2)]
    public string Code2 { get; set; } = ""; // ISO 3166-1 alpha-2 (OM, AE, SA, etc.)
    
    public bool IsActive { get; set; } = true;
    public bool IsGccCountry { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<City> Cities { get; set; } = [];
    public ICollection<ShippingZone> ShippingZones { get; set; } = [];
}