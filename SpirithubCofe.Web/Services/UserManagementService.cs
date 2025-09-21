using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

public class UserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<IdentityResult> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd = null)
    {
        await _userManager.SetLockoutEnabledAsync(user, true);
        return await _userManager.SetLockoutEndDateAsync(user, lockoutEnd ?? DateTimeOffset.Now.AddYears(100));
    }

    public async Task<IdentityResult> UnlockUserAsync(ApplicationUser user)
    {
        return await _userManager.SetLockoutEndDateAsync(user, null);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<List<ApplicationUser>> SearchUsersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllUsersAsync();

        return await _userManager.Users
            .Where(u => u.UserName!.Contains(searchTerm) || 
                       u.Email!.Contains(searchTerm) ||
                       u.PhoneNumber!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
    {
        return (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var totalUsers = allUsers.Count;
        var activeUsers = allUsers.Count(u => !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.Now);
        
        var adminUsers = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
        var customerUsers = (await _userManager.GetUsersInRoleAsync("Customer")).Count;
        var staffUsers = (await _userManager.GetUsersInRoleAsync("Staff")).Count;

        return new UserStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            AdminUsers = adminUsers,
            CustomerUsers = customerUsers,
            StaffUsers = staffUsers,
            LockedUsers = totalUsers - activeUsers
        };
    }
}

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int AdminUsers { get; set; }
    public int CustomerUsers { get; set; }
    public int StaffUsers { get; set; }
    public int LockedUsers { get; set; }
}