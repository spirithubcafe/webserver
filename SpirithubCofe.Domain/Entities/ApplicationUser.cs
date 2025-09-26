using Microsoft.AspNetCore.Identity;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Application user with additional profile information
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Last login date and time
    /// </summary>
    public DateTime? LastLoginDate { get; set; }
}