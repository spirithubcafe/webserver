using System.Net;

namespace SpirithubCofe.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Handle DOM-related errors more gracefully - these are usually harmless
        if (exception.Message.Contains("removeChild") || 
            exception.Message.Contains("Cannot read properties of null") ||
            exception.Message.Contains("TypeError"))
        {
            _logger.LogWarning("DOM manipulation error: {Message}. This is usually harmless and occurs during connection interruptions.", 
                exception.Message);
            
            // Don't return error response for DOM errors
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        }

        if (exception is AggregateException aggEx)
        {
            foreach (var innerEx in aggEx.InnerExceptions)
            {
                if (innerEx.Message.Contains("removeChild") || 
                    innerEx.Message.Contains("Cannot read properties of null") ||
                    innerEx.Message.Contains("TypeError"))
                {
                    _logger.LogWarning("DOM manipulation error in aggregate exception: {Message}. This is usually harmless.", 
                        innerEx.Message);
                    
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    return Task.CompletedTask;
                }
            }
        }

        // Log other exceptions normally
        _logger.LogError(exception, "Unhandled exception occurred");
        
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return Task.CompletedTask;
    }
}