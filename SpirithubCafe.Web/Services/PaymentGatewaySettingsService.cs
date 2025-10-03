using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SpirithubCafe.Web.Services;

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

    /// <summary>
    /// Encrypt payment data using AES-256-GCM (Bank Muscat standard)
    /// </summary>
    /// <param name="plaintext">Plain text data to encrypt</param>
    /// <param name="workingKey">Working key for encryption (hex string)</param>
    /// <returns>Encrypted data as hex string (nonce + ciphertext)</returns>
    public string EncryptPaymentData(string plaintext, string workingKey)
    {
        try
        {
            // Convert hex working key to bytes
            var keyBytes = Convert.FromHexString(workingKey);
            
            // Generate random 12-byte nonce
            var nonce = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }

            // Create AES-GCM cipher
            using var aes = new AesGcm(keyBytes, 16); // 16-byte tag size
            
            // Prepare plaintext bytes
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            
            // Prepare output arrays
            var ciphertext = new byte[plaintextBytes.Length];
            var tag = new byte[16];
            
            // Encrypt
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            
            // Combine nonce + ciphertext + tag
            var result = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Array.Copy(nonce, 0, result, 0, nonce.Length);
            Array.Copy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
            Array.Copy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);
            
            // Return as hex string
            return Convert.ToHexString(result).ToLower();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Encryption failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Decrypt payment response data using AES-256-GCM (Bank Muscat standard)
    /// </summary>
    /// <param name="encryptedDataHex">Encrypted data as hex string (nonce + ciphertext + tag)</param>
    /// <param name="workingKey">Working key for decryption (hex string)</param>
    /// <returns>Decrypted data as dictionary</returns>
    public Dictionary<string, object> DecryptPaymentResponse(string encryptedDataHex, string workingKey)
    {
        try
        {
            // Convert hex strings to bytes
            var encryptedData = Convert.FromHexString(encryptedDataHex);
            var keyBytes = Convert.FromHexString(workingKey);
            
            // Extract components
            var nonce = encryptedData[..12]; // First 12 bytes
            var tag = encryptedData[^16..]; // Last 16 bytes
            var ciphertext = encryptedData[12..^16]; // Middle part
            
            // Create AES-GCM cipher
            using var aes = new AesGcm(keyBytes, 16);
            
            // Prepare output array
            var plaintextBytes = new byte[ciphertext.Length];
            
            // Decrypt
            aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);
            
            // Convert to string
            var plaintext = Encoding.UTF8.GetString(plaintextBytes);
            
            // Parse as query string to dictionary
            return ParseQueryStringToMap(plaintext);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Decryption failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert query string to dictionary
    /// </summary>
    /// <param name="queryString">Query string (key1=value1&key2=value2)</param>
    /// <returns>Dictionary of key-value pairs</returns>
    private Dictionary<string, object> ParseQueryStringToMap(string queryString)
    {
        var result = new Dictionary<string, object>();
        
        if (string.IsNullOrEmpty(queryString))
            return result;
            
        var pairs = queryString.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }
        
        return result;
    }

    /// <summary>
    /// Get active payment gateway settings
    /// </summary>
    /// <returns>Active gateway settings or null</returns>
    public async Task<PaymentGatewaySettings?> GetActiveGatewayAsync()
    {
        return await _context.PaymentGatewaySettings
            .FirstOrDefaultAsync(g => g.IsEnabled);
    }
}