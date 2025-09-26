using Microsoft.AspNetCore.Identity;
using SpirithubCofe.Web.Data;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service to handle user login events and update last login date
/// </summary>
public class UserLoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserLoginService> _logger;

    public UserLoginService(UserManager<ApplicationUser> userManager, ILogger<UserLoginService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Update user's last login date
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns></returns>
    public async Task UpdateLastLoginDateAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Updated last login date for user {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login date for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Update user's last login date by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns></returns>
    public async Task UpdateLastLoginDateByEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Updated last login date for user with email {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login date for user with email {Email}", email);
        }
    }

    /// <summary>
    /// Test method to manually update a user's last login date - for debugging
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns></returns>
    public async Task<bool> TestUpdateLastLoginAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                _logger.LogWarning("TESTING: Found user {Email} with current LastLoginDate: {CurrentDate}", email, user.LastLoginDate);
                user.LastLoginDate = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);
                _logger.LogWarning("TESTING: Update result for {Email}: {Success}, New LastLoginDate: {NewDate}", 
                    email, result.Succeeded, user.LastLoginDate);
                return result.Succeeded;
            }
            else
            {
                _logger.LogWarning("TESTING: User not found for email {Email}", email);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TESTING: Error updating last login date for user with email {Email}", email);
            return false;
        }
    }
}