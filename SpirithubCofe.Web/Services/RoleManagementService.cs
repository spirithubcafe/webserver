using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Web.Data;
using SpirithubCofe.Domain.Entities;
using System.Security.Claims;

namespace SpirithubCofe.Web.Services;

public class RoleManagementService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleManagementService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<List<IdentityRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
    {
        return await _roleManager.FindByIdAsync(roleId);
    }

    public async Task<IdentityRole?> GetRoleByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName, string? description = null)
    {
        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded && !string.IsNullOrEmpty(description))
        {
            await _roleManager.AddClaimAsync(role, new Claim("description", description));
        }

        return result;
    }

    public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
    {
        return await _roleManager.UpdateAsync(role);
    }

    public async Task<IdentityResult> DeleteRoleAsync(IdentityRole role)
    {
        // Check if role has users
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            return IdentityResult.Failed(new IdentityError 
            { 
                Code = "RoleHasUsers", 
                Description = "Cannot delete role that has assigned users." 
            });
        }

        return await _roleManager.DeleteAsync(role);
    }

    public async Task<List<Claim>> GetRoleClaimsAsync(IdentityRole role)
    {
        var claims = await _roleManager.GetClaimsAsync(role);
        return claims.ToList();
    }

    public async Task<IdentityResult> AddClaimToRoleAsync(IdentityRole role, Claim claim)
    {
        return await _roleManager.AddClaimAsync(role, claim);
    }

    public async Task<IdentityResult> RemoveClaimFromRoleAsync(IdentityRole role, Claim claim)
    {
        return await _roleManager.RemoveClaimAsync(role, claim);
    }

    public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName)
    {
        return (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
    }

    public async Task<Dictionary<string, int>> GetRoleUserCountsAsync()
    {
        var roles = await GetAllRolesAsync();
        var counts = new Dictionary<string, int>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            counts[role.Name!] = usersInRole.Count;
        }

        return counts;
    }

    public async Task<RoleStatsDto> GetRoleStatsAsync()
    {
        var allRoles = await GetAllRolesAsync();
        var roleCounts = await GetRoleUserCountsAsync();

        var systemRoles = new[] { "Admin", "Customer", "Staff" };
        var customRoles = allRoles.Where(r => !systemRoles.Contains(r.Name!)).Count();

        return new RoleStatsDto
        {
            TotalRoles = allRoles.Count,
            CustomRoles = customRoles,
            SystemRoles = systemRoles.Length,
            TotalAssignedUsers = roleCounts.Values.Sum(),
            RoleUserCounts = roleCounts
        };
    }

    public bool IsSystemRole(string roleName)
    {
        return roleName is "Admin" or "Customer" or "Staff";
    }

    public bool CanDeleteRole(string roleName, int userCount)
    {
        return !IsSystemRole(roleName) && userCount == 0;
    }

    public List<string> GetDefaultPermissions(string roleName)
    {
        return roleName switch
        {
            "Admin" => new List<string>
            {
                "users.view", "users.create", "users.edit", "users.delete",
                "roles.view", "roles.create", "roles.edit", "roles.delete",
                "products.view", "products.create", "products.edit", "products.delete",
                "orders.view", "orders.edit", "orders.cancel",
                "reports.view", "settings.edit", "slides.manage"
            },
            "Staff" => new List<string>
            {
                "products.view", "products.create", "products.edit",
                "orders.view", "orders.edit",
                "customers.view", "reports.view"
            },
            "Customer" => new List<string>
            {
                "products.view", "orders.create", "orders.view"
            },
            _ => new List<string>()
        };
    }

    public async Task<IdentityResult> SetRolePermissionsAsync(string roleName, List<string> permissions)
    {
        var role = await GetRoleByNameAsync(roleName);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "RoleNotFound", Description = "Role not found." });
        }

        // Remove existing permission claims
        var existingClaims = await GetRoleClaimsAsync(role);
        var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();
        
        foreach (var claim in permissionClaims)
        {
            await RemoveClaimFromRoleAsync(role, claim);
        }

        // Add new permission claims
        foreach (var permission in permissions)
        {
            await AddClaimToRoleAsync(role, new Claim("permission", permission));
        }

        return IdentityResult.Success;
    }

    public async Task<List<string>> GetRolePermissionsAsync(string roleName)
    {
        var role = await GetRoleByNameAsync(roleName);
        if (role == null) return new List<string>();

        var claims = await GetRoleClaimsAsync(role);
        return claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
    }
}

public class RoleStatsDto
{
    public int TotalRoles { get; set; }
    public int CustomRoles { get; set; }
    public int SystemRoles { get; set; }
    public int TotalAssignedUsers { get; set; }
    public Dictionary<string, int> RoleUserCounts { get; set; } = new();
}