using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public interface IChatService
{
    Task<string> CreateChatSessionAsync(string visitorName, string? visitorEmail, string ipAddress);
    Task<ChatSession?> GetChatSessionAsync(string sessionId);
    Task<List<ChatSession>> GetActiveChatSessionsAsync();
    Task<ChatMessage> SendMessageAsync(string sessionId, string senderName, string message, bool isFromAdmin = false, string? userId = null);
    Task<List<ChatMessage>> GetMessagesAsync(string sessionId, int skip = 0, int take = 50);
    Task MarkMessagesAsReadAsync(string sessionId, bool fromAdmin = false);
    Task CloseChatSessionAsync(string sessionId);
    Task<int> GetUnreadMessageCountAsync(string sessionId, bool forAdmin = false);
    Task<int> GetTotalUnreadMessagesForAdminAsync();
    Task<List<ChatSession>> GetRecentChatSessionsAsync(int count = 10);
}

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;

    public ChatService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateChatSessionAsync(string visitorName, string? visitorEmail, string ipAddress)
    {
        var sessionId = Guid.NewGuid().ToString();
        
        var chatSession = new ChatSession
        {
            SessionId = sessionId,
            VisitorName = visitorName,
            VisitorEmail = visitorEmail,
            IpAddress = ipAddress,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.ChatSessions.Add(chatSession);
        await _context.SaveChangesAsync();

        return sessionId;
    }

    public async Task<ChatSession?> GetChatSessionAsync(string sessionId)
    {
        return await _context.ChatSessions
            .Include(cs => cs.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(cs => cs.SessionId == sessionId);
    }

    public async Task<List<ChatSession>> GetActiveChatSessionsAsync()
    {
        return await _context.ChatSessions
            .Where(cs => cs.IsActive)
            .Include(cs => cs.Messages)
            .OrderByDescending(cs => cs.LastMessageAt ?? cs.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatMessage> SendMessageAsync(string sessionId, string senderName, string message, bool isFromAdmin = false, string? userId = null)
    {
        var chatMessage = new ChatMessage
        {
            SessionId = sessionId,
            SenderName = senderName,
            UserId = userId,
            Message = message,
            IsFromAdmin = isFromAdmin,
            CreatedAt = DateTime.Now
        };

        _context.ChatMessages.Add(chatMessage);

        // Update session's last message time
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.SessionId == sessionId);
        
        if (session != null)
        {
            session.LastMessageAt = DateTime.Now;
            _context.ChatSessions.Update(session);
        }

        await _context.SaveChangesAsync();
        return chatMessage;
    }

    public async Task<List<ChatMessage>> GetMessagesAsync(string sessionId, int skip = 0, int take = 50)
    {
        return await _context.ChatMessages
            .Where(cm => cm.SessionId == sessionId)
            .OrderBy(cm => cm.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task MarkMessagesAsReadAsync(string sessionId, bool fromAdmin = false)
    {
        var messages = await _context.ChatMessages
            .Where(cm => cm.SessionId == sessionId && 
                        cm.IsFromAdmin != fromAdmin && 
                        !cm.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        if (messages.Any())
        {
            _context.ChatMessages.UpdateRange(messages);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CloseChatSessionAsync(string sessionId)
    {
        var session = await _context.ChatSessions
            .FirstOrDefaultAsync(cs => cs.SessionId == sessionId);

        if (session != null)
        {
            session.IsActive = false;
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadMessageCountAsync(string sessionId, bool forAdmin = false)
    {
        return await _context.ChatMessages
            .CountAsync(cm => cm.SessionId == sessionId && 
                             cm.IsFromAdmin != forAdmin && 
                             !cm.IsRead);
    }

    public async Task<int> GetTotalUnreadMessagesForAdminAsync()
    {
        return await _context.ChatMessages
            .CountAsync(cm => !cm.IsFromAdmin && !cm.IsRead);
    }

    public async Task<List<ChatSession>> GetRecentChatSessionsAsync(int count = 10)
    {
        return await _context.ChatSessions
            .Include(cs => cs.Messages)
            .OrderByDescending(cs => cs.LastMessageAt ?? cs.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}