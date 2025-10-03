using Microsoft.AspNetCore.Identity;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Identity email sender implementation using SMTP service
/// </summary>
public class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailService _emailService;

    public IdentityEmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        await _emailService.SendConfirmationEmailAsync(email, confirmationLink);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        await _emailService.SendPasswordResetEmailAsync(email, resetLink);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        await _emailService.SendPasswordResetCodeEmailAsync(email, resetCode);
    }
}