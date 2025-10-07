using SpirithubCafe.Application.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Service for handling contact form submissions
/// </summary>
public class ContactService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactService> _logger;

    public ContactService(IEmailService emailService, ILogger<ContactService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a contact form
    /// </summary>
    public Task<ContactSubmissionResult> SubmitContactFormAsync(ContactFormSubmission submission)
    {
        try
        {
            // Validate the submission
            var validationResults = ValidateSubmission(submission);
            if (validationResults.Any())
            {
                return Task.FromResult(ContactSubmissionResult.Error("Please correct the validation errors.", validationResults));
            }

            // Log the contact form submission (always works)
            _logger.LogInformation("Contact form submitted by {Name} ({Email}) - Subject: {Subject}", 
                submission.Name, submission.Email, submission.Subject);

            // Try to send email in background (don't wait for it)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendContactFormEmailAsync(
                        submission.Email,
                        submission.Name,
                        submission.Subject,
                        submission.Message
                    );
                    _logger.LogInformation("Email sent successfully for contact from {Name}", submission.Name);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send email for contact from {Name} ({Email})", 
                        submission.Name, submission.Email);
                }
            });

            // Always return success immediately
            return Task.FromResult(ContactSubmissionResult.Success("Thank you! Your message has been received and we'll get back to you soon."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process contact form for {Email}", submission.Email ?? "unknown");
            return Task.FromResult(ContactSubmissionResult.Error("An error occurred while processing your message. Please try again later."));
        }
    }

    private List<string> ValidateSubmission(ContactFormSubmission submission)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(submission.Name))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(submission.Email))
            errors.Add("Email is required.");
        else if (!IsValidEmail(submission.Email))
            errors.Add("Please enter a valid email address.");

        if (string.IsNullOrWhiteSpace(submission.Subject))
            errors.Add("Subject is required.");

        if (string.IsNullOrWhiteSpace(submission.Message))
            errors.Add("Message is required.");

        if (submission.Message?.Length > 2000)
            errors.Add("Message must be less than 2000 characters.");

        return errors;
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
/// Contact form submission model
/// </summary>
public class ContactFormSubmission
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Contact form submission result
/// </summary>
public class ContactSubmissionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ContactSubmissionResult Success(string message)
    {
        return new ContactSubmissionResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static ContactSubmissionResult Error(string message, List<string>? errors = null)
    {
        return new ContactSubmissionResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}