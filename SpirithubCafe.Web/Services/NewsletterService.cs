using SpirithubCafe.Application.Services;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Service for handling newsletter subscriptions
/// </summary>
public class NewsletterService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<NewsletterService> _logger;

    public NewsletterService(
        ApplicationDbContext context,
        IEmailService emailService, 
        ILogger<NewsletterService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to newsletter
    /// </summary>
    public async Task<NewsletterSubscriptionResult> SubscribeAsync(string email)
    {
        try
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return NewsletterSubscriptionResult.Error("Please enter a valid email address.");
            }

            // Check if already subscribed
            var existingSubscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(ns => ns.Email == email);

            if (existingSubscription != null)
            {
                if (existingSubscription.IsActive)
                {
                    return NewsletterSubscriptionResult.Error("You are already subscribed to our newsletter.");
                }
                else
                {
                    // Reactivate subscription
                    existingSubscription.IsActive = true;
                    existingSubscription.SubscribedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Newsletter subscription reactivated for {Email}", email);
                    return NewsletterSubscriptionResult.Success("Welcome back! Your newsletter subscription has been reactivated.");
                }
            }

            // Create new subscription
            var subscription = new NewsletterSubscription
            {
                Email = email,
                IsActive = true,
                SubscribedAt = DateTime.UtcNow
            };

            _context.NewsletterSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New newsletter subscription created for {Email}", email);

            // Send welcome email (optional)
            try
            {
                await SendWelcomeEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
                // Don't fail the subscription if welcome email fails
            }

            return NewsletterSubscriptionResult.Success("Thank you for subscribing! You'll receive our latest updates and exclusive offers.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process newsletter subscription for {Email}", email);
            return NewsletterSubscriptionResult.Error("An error occurred while processing your subscription. Please try again later.");
        }
    }

    /// <summary>
    /// Unsubscribe from newsletter
    /// </summary>
    public async Task<NewsletterSubscriptionResult> UnsubscribeAsync(string email)
    {
        try
        {
            var subscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(ns => ns.Email == email && ns.IsActive);

            if (subscription == null)
            {
                return NewsletterSubscriptionResult.Error("No active subscription found for this email address.");
            }

            subscription.IsActive = false;
            subscription.UnsubscribedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Newsletter subscription cancelled for {Email}", email);
            return NewsletterSubscriptionResult.Success("You have been successfully unsubscribed from our newsletter.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe {Email} from newsletter", email);
            return NewsletterSubscriptionResult.Error("An error occurred while processing your unsubscription. Please try again later.");
        }
    }

    /// <summary>
    /// Send newsletter to all active subscribers
    /// </summary>
    public async Task<int> SendNewsletterAsync(string subject, string content)
    {
        try
        {
            var activeSubscribers = await _context.NewsletterSubscriptions
                .Where(ns => ns.IsActive)
                .Select(ns => ns.Email)
                .ToListAsync();

            var successCount = 0;
            var batchSize = 50; // Send in batches to avoid overwhelming the SMTP server

            for (int i = 0; i < activeSubscribers.Count; i += batchSize)
            {
                var batch = activeSubscribers.Skip(i).Take(batchSize);
                
                foreach (var email in batch)
                {
                    try
                    {
                        await _emailService.SendNewsletterEmailAsync(email, content);
                        successCount++;
                        
                        // Small delay between emails to be respectful to SMTP server
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send newsletter to {Email}", email);
                    }
                }

                // Larger delay between batches
                if (i + batchSize < activeSubscribers.Count)
                {
                    await Task.Delay(2000);
                }
            }

            _logger.LogInformation("Newsletter sent to {SuccessCount} out of {TotalCount} subscribers", 
                successCount, activeSubscribers.Count);

            return successCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send newsletter");
            throw;
        }
    }

    /// <summary>
    /// Get newsletter statistics
    /// </summary>
    public async Task<NewsletterStats> GetStatsAsync()
    {
        var totalSubscribers = await _context.NewsletterSubscriptions.CountAsync(ns => ns.IsActive);
        var totalUnsubscribed = await _context.NewsletterSubscriptions.CountAsync(ns => !ns.IsActive);
        var recentSubscriptions = await _context.NewsletterSubscriptions
            .CountAsync(ns => ns.IsActive && ns.SubscribedAt >= DateTime.UtcNow.AddDays(-30));

        return new NewsletterStats
        {
            TotalActiveSubscribers = totalSubscribers,
            TotalUnsubscribed = totalUnsubscribed,
            RecentSubscriptions = recentSubscriptions
        };
    }

    private async Task SendWelcomeEmail(string email)
    {
        var content = @"
<div style='text-align: center; margin-bottom: 30px;'>
    <h2 style='color: #8B4513; margin-bottom: 20px;'>Welcome to SpirithubCafe Newsletter!</h2>
    <p style='font-size: 16px; line-height: 1.6; color: #333;'>
        Thank you for subscribing to our newsletter! You'll be the first to know about:
    </p>
    <ul style='text-align: left; max-width: 400px; margin: 0 auto; color: #666;'>
        <li>New coffee arrivals and seasonal blends</li>
        <li>Exclusive subscriber discounts and offers</li>
        <li>Coffee brewing tips and techniques</li>
        <li>Behind-the-scenes content from our roastery</li>
        <li>Special events and tastings</li>
    </ul>
    <p style='margin-top: 30px; font-size: 16px; color: #8B4513;'>
        <strong>Welcome to the SpirithubCafe family!</strong>
    </p>
</div>";

        await _emailService.SendNewsletterEmailAsync(email, content);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Newsletter subscription result
/// </summary>
public class NewsletterSubscriptionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static NewsletterSubscriptionResult Success(string message)
    {
        return new NewsletterSubscriptionResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static NewsletterSubscriptionResult Error(string message)
    {
        return new NewsletterSubscriptionResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

/// <summary>
/// Newsletter statistics
/// </summary>
public class NewsletterStats
{
    public int TotalActiveSubscribers { get; set; }
    public int TotalUnsubscribed { get; set; }
    public int RecentSubscriptions { get; set; }
}