using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpirithubCofe.Web.Data;
using SpirithubCofe.Web.Services;
using SpirithubCofe.Domain.Entities;
using System.Security.Claims;

namespace SpirithubCofe.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManagementService _userService;
    private readonly RoleManagementService _roleService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManagementService userService,
        RoleManagementService roleService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userService = userService;
        _roleService = roleService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    #region Users Management

    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] string? search = null)
    {
        var users = string.IsNullOrEmpty(search) 
            ? await _userService.GetAllUsersAsync()
            : await _userService.SearchUsersAsync(search);

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userService.GetUserRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles
            });
        }

        return Ok(userDtos);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var roles = await _userService.GetUserRolesAsync(user);
        
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            Roles = roles
        });
    }

    [HttpPost("users/{id}/lock")]
    public async Task<ActionResult> LockUser(string id, [FromBody] LockUserRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userService.LockUserAsync(user, request.LockoutEnd);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost("users/{id}/unlock")]
    public async Task<ActionResult> UnlockUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userService.UnlockUserAsync(user);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost("users/{id}/roles")]
    public async Task<ActionResult> AssignRole(string id, [FromBody] AssignRoleRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userService.AddUserToRoleAsync(user, request.RoleName);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpDelete("users/{id}/roles/{roleName}")]
    public async Task<ActionResult> RemoveRole(string id, string roleName)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userService.RemoveUserFromRoleAsync(user, roleName);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost("users/{id}/reset-password")]
    public async Task<ActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userService.ResetPasswordAsync(user, request.NewPassword);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpGet("users/stats")]
    public async Task<ActionResult<UserStatsDto>> GetUserStats()
    {
        var stats = await _userService.GetUserStatsAsync();
        return Ok(stats);
    }

    #endregion

    #region Roles Management

    [HttpGet("roles")]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        var roleCounts = await _roleService.GetRoleUserCountsAsync();

        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name!,
            UserCount = roleCounts.GetValueOrDefault(r.Name!, 0),
            IsSystemRole = _roleService.IsSystemRole(r.Name!),
            CanDelete = _roleService.CanDeleteRole(r.Name!, roleCounts.GetValueOrDefault(r.Name!, 0))
        }).ToList();

        return Ok(roleDtos);
    }

    [HttpGet("roles/{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        var userCount = (await _roleService.GetUsersInRoleAsync(role.Name!)).Count;
        var permissions = await _roleService.GetRolePermissionsAsync(role.Name!);

        return Ok(new RoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            UserCount = userCount,
            IsSystemRole = _roleService.IsSystemRole(role.Name!),
            CanDelete = _roleService.CanDeleteRole(role.Name!, userCount),
            Permissions = permissions
        });
    }

    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
    {
        var result = await _roleService.CreateRoleAsync(request.Name, request.Description);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var role = await _roleService.GetRoleByNameAsync(request.Name);
        return Ok(new RoleDto
        {
            Id = role!.Id,
            Name = role.Name!,
            UserCount = 0,
            IsSystemRole = false,
            CanDelete = true
        });
    }

    [HttpPut("roles/{id}")]
    public async Task<ActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        role.Name = request.Name;
        var result = await _roleService.UpdateRoleAsync(role);
        
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpDelete("roles/{id}")]
    public async Task<ActionResult> DeleteRole(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        if (_roleService.IsSystemRole(role.Name!))
            return BadRequest("Cannot delete system roles");

        var result = await _roleService.DeleteRoleAsync(role);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpGet("roles/{id}/users")]
    public async Task<ActionResult<List<UserDto>>> GetRoleUsers(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        var users = await _roleService.GetUsersInRoleAsync(role.Name!);
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName!,
            Email = u.Email!,
            EmailConfirmed = u.EmailConfirmed
        }).ToList();

        return Ok(userDtos);
    }

    [HttpPost("roles/{id}/permissions")]
    public async Task<ActionResult> SetRolePermissions(string id, [FromBody] SetPermissionsRequest request)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        var result = await _roleService.SetRolePermissionsAsync(role.Name!, request.Permissions);
        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpGet("roles/{id}/permissions")]
    public async Task<ActionResult<List<string>>> GetRolePermissions(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        var permissions = await _roleService.GetRolePermissionsAsync(role.Name!);
        return Ok(permissions);
    }

    [HttpGet("roles/stats")]
    public async Task<ActionResult<RoleStatsDto>> GetRoleStats()
    {
        var stats = await _roleService.GetRoleStatsAsync();
        return Ok(stats);
    }

    #endregion

    #region Available Permissions

    [HttpGet("permissions")]
    public ActionResult<List<PermissionDto>> GetAvailablePermissions()
    {
        var permissions = new List<PermissionDto>
        {
            // User Management
            new() { Name = "users.view", Category = "User Management", Description = "View users" },
            new() { Name = "users.create", Category = "User Management", Description = "Create users" },
            new() { Name = "users.edit", Category = "User Management", Description = "Edit users" },
            new() { Name = "users.delete", Category = "User Management", Description = "Delete users" },
            
            // Role Management
            new() { Name = "roles.view", Category = "Role Management", Description = "View roles" },
            new() { Name = "roles.create", Category = "Role Management", Description = "Create roles" },
            new() { Name = "roles.edit", Category = "Role Management", Description = "Edit roles" },
            new() { Name = "roles.delete", Category = "Role Management", Description = "Delete roles" },
            
            // Product Management
            new() { Name = "products.view", Category = "Product Management", Description = "View products" },
            new() { Name = "products.create", Category = "Product Management", Description = "Create products" },
            new() { Name = "products.edit", Category = "Product Management", Description = "Edit products" },
            new() { Name = "products.delete", Category = "Product Management", Description = "Delete products" },
            
            // Order Management
            new() { Name = "orders.view", Category = "Order Management", Description = "View orders" },
            new() { Name = "orders.edit", Category = "Order Management", Description = "Edit orders" },
            new() { Name = "orders.cancel", Category = "Order Management", Description = "Cancel orders" },
            new() { Name = "orders.create", Category = "Order Management", Description = "Create orders" },
            
            // Reports
            new() { Name = "reports.view", Category = "Reports", Description = "View reports" },
            new() { Name = "reports.export", Category = "Reports", Description = "Export reports" },
            
            // Settings
            new() { Name = "settings.edit", Category = "Settings", Description = "Edit system settings" },
            new() { Name = "slides.manage", Category = "Content", Description = "Manage slideshow" },
            
            // Customer Support
            new() { Name = "customers.view", Category = "Customer Support", Description = "View customer information" },
            new() { Name = "customers.support", Category = "Customer Support", Description = "Provide customer support" }
        };

        return Ok(permissions);
    }

    #endregion
}

#region DTOs

public class UserDto
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class RoleDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int UserCount { get; set; }
    public bool IsSystemRole { get; set; }
    public bool CanDelete { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
}

public class LockUserRequest
{
    public DateTimeOffset? LockoutEnd { get; set; }
}

public class AssignRoleRequest
{
    public string RoleName { get; set; } = "";
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = "";
}

public class CreateRoleRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class SetPermissionsRequest
{
    public List<string> Permissions { get; set; } = new();
}

#endregion