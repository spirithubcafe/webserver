using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public interface IChatService
{
    Task<ChatSession> CreateChatSessionAsync(string visitorName, string? visitorEmail, string ipAddress);
    Task<ChatSession?> GetChatSessionAsync(string sessionId);
    Task<IEnumerable<ChatSession>> GetActiveSessionsAsync();
    Task<ChatMessage> SendMessageAsync(string sessionId, ChatSenderType senderType, string senderName, string message);
    Task<IEnumerable<ChatMessage>> GetMessagesAsync(string sessionId);
    Task<IEnumerable<ChatMessage>> GetMessagesAsync(string sessionId, DateTime since);
    Task MarkMessagesAsReadAsync(string sessionId, ChatSenderType senderType);
    Task CloseChatSessionAsync(string sessionId);
}

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;

    public ChatService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession> CreateChatSessionAsync(string visitorName, string? visitorEmail, string ipAddress)
    {
        var session = new ChatSession
        {
            Id = Guid.NewGuid().ToString(),
            VisitorName = visitorName,
            VisitorEmail = visitorEmail,
            IpAddress = ipAddress,
            IsActive = true,
            CreatedAt = DateTime.Now,
            LastMessageAt = DateTime.Now
        };

        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<ChatSession?> GetChatSessionAsync(string sessionId)
    {
        return await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.Id == sessionId);
    }

    public async Task<IEnumerable<ChatSession>> GetActiveSessionsAsync()
    {
        return await _context.ChatSessions
            .Where(cs => cs.IsActive)
            .OrderByDescending(cs => cs.LastMessageAt ?? cs.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatMessage> SendMessageAsync(string sessionId, ChatSenderType senderType, string senderName, string message)
    {
        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ChatSessionId = sessionId,
            SenderName = senderName,
            Content = message,
            SenderType = senderType,
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        _context.ChatMessages.Add(chatMessage);

        // Update session's last message time and unread status
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.Id == sessionId);
        
        if (session != null)
        {
            session.LastMessageAt = DateTime.Now;
            
            // Update unread message flags based on sender
            if (senderType == ChatSenderType.Visitor)
            {
                session.HasUnreadVisitorMessages = true;
            }
            else if (senderType == ChatSenderType.Admin)
            {
                session.HasUnreadAdminMessages = true;
            }
            
            _context.ChatSessions.Update(session);
        }

        await _context.SaveChangesAsync();
        return chatMessage;
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(string sessionId)
    {
        return await _context.ChatMessages
            .Where(cm => cm.ChatSessionId == sessionId)
            .OrderBy(cm => cm.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(string sessionId, DateTime since)
    {
        return await _context.ChatMessages
            .Where(cm => cm.ChatSessionId == sessionId && cm.CreatedAt > since)
            .OrderBy(cm => cm.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkMessagesAsReadAsync(string sessionId, ChatSenderType senderType)
    {
        // Mark messages from the specified sender type as read
        var messages = await _context.ChatMessages
            .Where(cm => cm.ChatSessionId == sessionId && cm.SenderType == senderType && !cm.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        // Update session unread flags
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.Id == sessionId);
        
        if (session != null)
        {
            if (senderType == ChatSenderType.Visitor)
            {
                session.HasUnreadVisitorMessages = false;
            }
            else if (senderType == ChatSenderType.Admin)
            {
                session.HasUnreadAdminMessages = false;
            }
            
            _context.ChatSessions.Update(session);
        }

        await _context.SaveChangesAsync();
    }

    public async Task CloseChatSessionAsync(string sessionId)
    {
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.Id == sessionId);
        
        if (session != null)
        {
            session.IsActive = false;
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}