using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

public class ChatMessage
{
    [Required]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ChatSessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string SenderName { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public ChatSenderType SenderType { get; set; } = ChatSenderType.Visitor;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ChatSession? ChatSession { get; set; }
}