using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class ChatSession
{
    [Required]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string VisitorName { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? VisitorEmail { get; set; }
    
    [MaxLength(50)]
    public string IpAddress { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public bool HasUnreadAdminMessages { get; set; } = false;
    
    public bool HasUnreadVisitorMessages { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastMessageAt { get; set; }
    
    // Navigation property
    public List<ChatMessage> Messages { get; set; } = new();
}