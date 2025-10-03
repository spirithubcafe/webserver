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
    public async Task<ContactSubmissionResult> SubmitContactFormAsync(ContactFormSubmission submission)
    {
        try
        {
            // Validate the submission
            var validationResults = ValidateSubmission(submission);
            if (validationResults.Any())
            {
                return ContactSubmissionResult.Error("Please correct the validation errors.", validationResults);
            }

            // Send email to admin
            await _emailService.SendContactFormEmailAsync(
                submission.Email,
                submission.Name,
                submission.Subject,
                submission.Message
            );

            _logger.LogInformation("Contact form submitted successfully by {Name} ({Email})", 
                submission.Name, submission.Email);

            return ContactSubmissionResult.Success("Thank you for your message! We'll get back to you soon.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit contact form for {Email}", submission.Email);
            return ContactSubmissionResult.Error("An error occurred while sending your message. Please try again later.");
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