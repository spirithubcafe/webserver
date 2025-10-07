using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Application.Services
{
    public interface INewsletterService
    {
        Task<(bool Success, string Message)> SubscribeAsync(string email, string? name = null);
        Task<bool> UnsubscribeAsync(string email);
        Task<bool> IsSubscribedAsync(string email);
    }

    public class NewsletterService : INewsletterService
    {
        private readonly IApplicationDbContext _context;

        public NewsletterService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> SubscribeAsync(string email, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, "EmailRequired");
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                return (false, "InvalidEmailFormat");
            }

            // Check if already subscribed
            var existingSubscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());

            if (existingSubscription != null)
            {
                if (existingSubscription.IsActive)
                {
                    return (false, "AlreadySubscribed");
                }
                else
                {
                    // Reactivate subscription
                    existingSubscription.IsActive = true;
                    existingSubscription.UnsubscribedAt = null;
                    existingSubscription.SubscribedAt = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        existingSubscription.Name = name;
                    }
                    await _context.SaveChangesAsync(CancellationToken.None);
                    return (true, "ResubscribedSuccessfully");
                }
            }

            // Create new subscription
            var subscription = new NewsletterSubscription
            {
                Email = email.ToLower(),
                Name = name,
                IsActive = true,
                SubscribedAt = DateTime.UtcNow
            };

            _context.NewsletterSubscriptions.Add(subscription);
            await _context.SaveChangesAsync(CancellationToken.None);

            return (true, "SubscribedSuccessfully");
        }

        public async Task<bool> UnsubscribeAsync(string email)
        {
            var subscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower() && s.IsActive);

            if (subscription == null)
            {
                return false;
            }

            subscription.IsActive = false;
            subscription.UnsubscribedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(CancellationToken.None);

            return true;
        }

        public async Task<bool> IsSubscribedAsync(string email)
        {
            return await _context.NewsletterSubscriptions
                .AnyAsync(s => s.Email.ToLower() == email.ToLower() && s.IsActive);
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
}
