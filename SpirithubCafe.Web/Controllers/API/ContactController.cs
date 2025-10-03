using Microsoft.AspNetCore.Mvc;
using SpirithubCafe.Application.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Web.Controllers.API;

/// <summary>
/// API Controller for handling contact form submissions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContactController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IEmailService emailService, ILogger<ContactController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Submit contact form
    /// </summary>
    /// <param name="request">Contact form data</param>
    /// <returns>Success or error response</returns>
    [HttpPost]
    public async Task<IActionResult> SubmitContactForm([FromBody] ContactFormRequest request)
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
            // Send email to admin
            await _emailService.SendContactFormEmailAsync(
                request.Email,
                request.Name,
                request.Subject,
                request.Message);

            _logger.LogInformation("Contact form submission received from {Name} ({Email}) with subject: {Subject}", 
                request.Name, request.Email, request.Subject);

            return Ok(new { Success = true, Message = "Your message has been sent successfully. We'll get back to you soon!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process contact form submission from {Name} ({Email})", 
                request.Name, request.Email);
            
            return StatusCode(500, new { Success = false, Message = "Sorry, there was an error sending your message. Please try again later." });
        }
    }
}

/// <summary>
/// Contact form submission request
/// </summary>
public class ContactFormRequest
{
    /// <summary>
    /// Sender's name
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Sender's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    /// <summary>
    /// Message subject
    /// </summary>
    [Required(ErrorMessage = "Subject is required")]
    [MaxLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Message content
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters long")]
    [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;
}