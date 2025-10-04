using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using SpirithubCafe.Web.Services;
using System.Security.Claims;

namespace SpirithubCafe.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async Task JoinChatSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        _logger.LogInformation($"Connection {Context.ConnectionId} joined chat session {sessionId}");
    }

    public async Task LeaveChatSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        _logger.LogInformation($"Connection {Context.ConnectionId} left chat session {sessionId}");
    }

    [Authorize(Roles = "Admin")]
    public async Task JoinAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        _logger.LogInformation($"Admin connection {Context.ConnectionId} joined admin group");
    }

    [Authorize(Roles = "Admin")]
    public async Task LeaveAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");
        _logger.LogInformation($"Admin connection {Context.ConnectionId} left admin group");
    }

    public async Task SendMessageToChat(string sessionId, string senderName, string message)
    {
        try
        {
            var isAdmin = Context.User?.IsInRole("Admin") ?? false;
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var chatMessage = await _chatService.SendMessageAsync(
                sessionId, 
                senderName, 
                message, 
                isAdmin, 
                userId
            );

            // Send to specific chat session group
            await Clients.Group($"chat_{sessionId}").SendAsync("ReceiveMessage", new
            {
                id = chatMessage.Id,
                sessionId = chatMessage.SessionId,
                senderName = chatMessage.SenderName,
                message = chatMessage.Message,
                isFromAdmin = chatMessage.IsFromAdmin,
                createdAt = chatMessage.CreatedAt,
                isRead = chatMessage.IsRead
            });

            // If message is from visitor, notify all admins
            if (!isAdmin)
            {
                await Clients.Group("admin").SendAsync("NewVisitorMessage", new
                {
                    sessionId = sessionId,
                    senderName = senderName,
                    message = message,
                    createdAt = chatMessage.CreatedAt
                });
            }

            _logger.LogInformation($"Message sent in session {sessionId} by {senderName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message in session {sessionId}");
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
        
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}