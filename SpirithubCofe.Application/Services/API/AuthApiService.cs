using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpirithubCofe.Application.DTOs.API;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpirithubCofe.Application.Services.API;

/// <summary>
/// Service for handling authentication operations for API
/// </summary>
public interface IAuthApiService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<UserInfoDto>> GetUserInfoAsync(string userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    Task<ApiResponse<UserInfoDto>> UpdateProfileAsync(string userId, UpdateProfileRequestDto request);
    Task<ApiResponse<bool>> LogoutAsync(string userId);
}

/// <summary>
/// Implementation of authentication API service
/// </summary>
public class AuthApiService<TUser> : IAuthApiService where TUser : IdentityUser
{
    private readonly UserManager<TUser> _userManager;
    private readonly SignInManager<TUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthApiService(
        UserManager<TUser> userManager,
        SignInManager<TUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            var token = await GenerateJwtTokenAsync(user);
            var userInfo = await CreateUserInfoDto(user);

            var authResponse = new AuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = 3600, // 1 hour
                User = userInfo
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during login", ex.Message);
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("User with this email already exists");
            }

            var user = Activator.CreateInstance<TUser>();
            user.UserName = request.Email;
            user.Email = request.Email;
            user.EmailConfirmed = true; // For API, we'll assume email is confirmed

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<AuthResponseDto>.ErrorResponse("Registration failed", errors);
            }

            // Add user to Customer role by default
            await _userManager.AddToRoleAsync(user, "Customer");

            var token = await GenerateJwtTokenAsync(user);
            var userInfo = await CreateUserInfoDto(user);

            var authResponse = new AuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = 3600, // 1 hour
                User = userInfo
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Registration successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during registration", ex.Message);
        }
    }

    public async Task<ApiResponse<UserInfoDto>> GetUserInfoAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            var userInfo = await CreateUserInfoDto(user);
            return ApiResponse<UserInfoDto>.SuccessResponse(userInfo);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while retrieving user info", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<bool>.ErrorResponse("Password change failed", errors);
            }

            return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse("An error occurred while changing password", ex.Message);
        }
    }

    public async Task<ApiResponse<UserInfoDto>> UpdateProfileAsync(string userId, UpdateProfileRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            // Update phone number if provided
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<UserInfoDto>.ErrorResponse("Profile update failed", errors);
            }

            var userInfo = await CreateUserInfoDto(user);
            return ApiResponse<UserInfoDto>.SuccessResponse(userInfo, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while updating profile", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string userId)
    {
        try
        {
            // For JWT tokens, logout is typically handled client-side by removing the token
            // We could implement token blacklisting here if needed
            
            await _signInManager.SignOutAsync();
            return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse("An error occurred during logout", ex.Message);
        }
    }

    private async Task<string> GenerateJwtTokenAsync(TUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong!");
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add role claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"] ?? "SpirithubCofe",
            Audience = jwtSettings["Audience"] ?? "SpirithubCofe"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<UserInfoDto> CreateUserInfoDto(TUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Roles = roles.ToList(),
            EmailConfirmed = user.EmailConfirmed
        };
    }
}