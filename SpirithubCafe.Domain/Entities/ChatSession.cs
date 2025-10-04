using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class ChatSession
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string VisitorName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? VisitorEmail { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? LastMessageAt { get; set; }
    
    // Navigation property
    public List<ChatMessage> Messages { get; set; } = new();
}