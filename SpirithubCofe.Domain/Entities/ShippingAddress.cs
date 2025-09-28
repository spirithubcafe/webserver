using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

public class ShippingAddress
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Line1 { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Line2 { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string State { get; set; } = string.Empty; // Governorate
    
    [Required]
    [MaxLength(10)]
    public string Country { get; set; } = "OM"; // Oman
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}