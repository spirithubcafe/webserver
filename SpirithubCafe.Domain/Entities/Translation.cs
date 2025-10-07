using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class Translation
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(5000)]
    public string ValueEn { get; set; } = string.Empty; // English translation
    
    [Required]
    [MaxLength(5000)]
    public string ValueAr { get; set; } = string.Empty; // Arabic translation
    
    [MaxLength(100)]
    public string? Category { get; set; } // e.g., "UI", "Product", "Page", etc.
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
