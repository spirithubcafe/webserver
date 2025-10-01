using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service for managing payment gateway settings
/// </summary>
public class PaymentGatewaySettingsService
{
    private readonly ApplicationDbContext _context;

    public PaymentGatewaySettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get Bank Muscat payment gateway settings
    /// </summary>
    /// <returns>Payment gateway settings or null if not found</returns>
    public async Task<PaymentGatewaySettings?> GetBankMuscatSettingsAsync()
    {
        return await _context.PaymentGatewaySettings
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Update Bank Muscat payment gateway settings
    /// </summary>
    /// <param name="settings">Settings to update</param>
    /// <returns>Updated settings</returns>
    public async Task<PaymentGatewaySettings> UpdateBankMuscatSettingsAsync(PaymentGatewaySettings settings)
    {
        var existingSettings = await _context.PaymentGatewaySettings.FirstOrDefaultAsync();

        if (existingSettings == null)
        {
            // Create new settings
            settings.CreatedAt = DateTime.UtcNow;
            settings.UpdatedAt = DateTime.UtcNow;
            _context.PaymentGatewaySettings.Add(settings);
        }
        else
        {
            // Update existing settings
            existingSettings.GatewayName = settings.GatewayName;
            existingSettings.Title = settings.Title;
            existingSettings.Description = settings.Description;
            existingSettings.IsEnabled = settings.IsEnabled;
            existingSettings.MerchantId = settings.MerchantId;
            existingSettings.AccessCode = settings.AccessCode;
            existingSettings.WorkingKey = settings.WorkingKey;
            existingSettings.IsSandboxMode = settings.IsSandboxMode;
            existingSettings.LiveGatewayUrl = settings.LiveGatewayUrl;
            existingSettings.SandboxGatewayUrl = settings.SandboxGatewayUrl;
            existingSettings.ReturnUrl = settings.ReturnUrl;
            existingSettings.CancelUrl = settings.CancelUrl;
            existingSettings.Currency = settings.Currency;
            existingSettings.UpdatedAt = DateTime.UtcNow;

            settings = existingSettings;
        }

        await _context.SaveChangesAsync();
        return settings;
    }

    /// <summary>
    /// Create default settings if none exist
    /// </summary>
    /// <returns>Default settings</returns>
    public async Task<PaymentGatewaySettings> CreateDefaultSettingsAsync()
    {
        var defaultSettings = new PaymentGatewaySettings
        {
            GatewayName = "Bank Muscat",
            Title = "Bank Muscat",
            Description = "Pay securely by Credit or Debit card or internet banking through Bank Muscat Secure Servers.",
            IsEnabled = false,
            MerchantId = "224",
            AccessCode = "AVDP00LA16BE47PDEB",
            WorkingKey = "841FEAE32609C3E892C4D0B1393A7ACC",
            IsSandboxMode = true,
            LiveGatewayUrl = "https://secure.checkout.visa.com/payment/",
            SandboxGatewayUrl = "https://secure.checkout.visa.com/payment/",
            ReturnUrl = "/api/payment/callback/success",
            CancelUrl = "/api/payment/callback/cancel",
            Currency = "OMR",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PaymentGatewaySettings.Add(defaultSettings);
        await _context.SaveChangesAsync();
        return defaultSettings;
    }

    /// <summary>
    /// Check if Bank Muscat payment gateway is enabled
    /// </summary>
    /// <returns>True if enabled, false otherwise</returns>
    public async Task<bool> IsBankMuscatEnabledAsync()
    {
        var settings = await GetBankMuscatSettingsAsync();
        return settings?.IsEnabled ?? false;
    }

    /// <summary>
    /// Test connection to Bank Muscat gateway
    /// </summary>
    /// <returns>True if connection successful</returns>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var settings = await GetBankMuscatSettingsAsync();
            if (settings == null) return false;

            // Simple validation of required fields
            if (string.IsNullOrEmpty(settings.MerchantId) ||
                string.IsNullOrEmpty(settings.AccessCode) ||
                string.IsNullOrEmpty(settings.WorkingKey))
            {
                return false;
            }

            // Here you can add actual API test call to Bank Muscat
            // For now, we'll just return true if all required fields are present
            return true;
        }
        catch
        {
            return false;
        }
    }
}