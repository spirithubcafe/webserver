using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string SenderName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? UserId { get; set; } // For logged-in users or admin
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public bool IsFromAdmin { get; set; } = false;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
}