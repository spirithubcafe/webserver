using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class City
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
    
    [MaxLength(100)]
    public string? NameAr { get; set; }
    
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    
    // For NOOL and Aramex specific settings
    public string? NoolCode { get; set; }
    public string? AramexCode { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ShippingRate> ShippingRates { get; set; } = [];
}