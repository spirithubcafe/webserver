using Microsoft.AspNetCore.Mvc;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Web.Controllers.API;

/// <summary>
/// API Controller for handling newsletter subscriptions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NewsletterController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(
        ApplicationDbContext context, 
        IEmailService emailService, 
        ILogger<NewsletterController> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to newsletter
    /// </summary>
    /// <param name="request">Newsletter subscription data</param>
    /// <returns>Success or error response</returns>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] NewsletterSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { Success = false, Message = "Validation failed", Errors = errors });
        }

        try
        {
            // Check if email is already subscribed
            var existingSubscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(n => n.Email.ToLower() == request.Email.ToLower());

            if (existingSubscription != null)
            {
                if (existingSubscription.IsActive)
                {
                    return Ok(new { Success = true, Message = "You are already subscribed to our newsletter!" });
                }
                else
                {
                    // Reactivate subscription
                    existingSubscription.IsActive = true;
                    existingSubscription.SubscribedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Create new subscription
                var subscription = new NewsletterSubscription
                {
                    Email = request.Email.ToLower(),
                    Name = request.Name,
                    IsActive = true,
                    SubscribedAt = DateTime.UtcNow
                };

                _context.NewsletterSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }

            // Send welcome email
            try
            {
                await _emailService.SendNewsletterEmailAsync(
                    request.Email,
                    $@"
                    <div style='text-align: center; margin: 20px 0;'>
                        <h3 style='color: #8B4513;'>Welcome to SpirithubCafe Newsletter!</h3>
                        <p>Thank you for subscribing to our newsletter. You'll be the first to know about:</p>
                        <ul style='text-align: left; display: inline-block; margin: 20px 0;'>
                            <li>New coffee arrivals and special blends</li>
                            <li>Exclusive offers and discounts</li>
                            <li>Brewing tips and coffee knowledge</li>
                            <li>Upcoming events and promotions</li>
                        </ul>
                        <p style='direction: rtl; margin-top: 20px;'>
                            مرحباً بك في النشرة الإخبارية لـ SpirithubCafe! شكراً لاشتراكك معنا.
                        </p>
                    </div>
                    ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to newsletter subscriber {Email}", request.Email);
                // Don't fail the subscription if email fails
            }

            _logger.LogInformation("New newsletter subscription: {Email}", request.Email);

            return Ok(new { Success = true, Message = "Thank you for subscribing to our newsletter!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process newsletter subscription for {Email}", request.Email);
            
            return StatusCode(500, new { Success = false, Message = "Sorry, there was an error processing your subscription. Please try again later." });
        }
    }

    /// <summary>
    /// Unsubscribe from newsletter
    /// </summary>
    /// <param name="request">Unsubscribe request containing email</param>
    /// <returns>Success or error response</returns>
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Success = false, Message = "Invalid email address" });
        }

        try
        {
            var subscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(n => n.Email.ToLower() == request.Email.ToLower());

            if (subscription != null && subscription.IsActive)
            {
                subscription.IsActive = false;
                subscription.UnsubscribedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Newsletter unsubscription: {Email}", request.Email);
            }

            return Ok(new { Success = true, Message = "You have been successfully unsubscribed from our newsletter." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process newsletter unsubscription for {Email}", request.Email);
            
            return StatusCode(500, new { Success = false, Message = "Sorry, there was an error processing your unsubscription. Please try again later." });
        }
    }
}

/// <summary>
/// Newsletter subscription request
/// </summary>
public class NewsletterSubscriptionRequest
{
    /// <summary>
    /// Subscriber's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Subscriber's name (optional)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }
}

/// <summary>
/// Newsletter unsubscribe request
/// </summary>
public class UnsubscribeRequest
{
    /// <summary>
    /// Email to unsubscribe
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;
}