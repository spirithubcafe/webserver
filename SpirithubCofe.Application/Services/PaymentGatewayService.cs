using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SpirithubCofe.Application.Services;

public interface IPaymentGatewayService
{
    Task<PaymentGatewaySettings?> GetActiveGatewayAsync();
    Task<string> GeneratePaymentUrlAsync(PaymentGatewayRequestDto request);
    Task<bool> ValidateCallbackAsync(Dictionary<string, string> parameters);
    Task<PaymentCallbackDto> ProcessCallbackAsync(Dictionary<string, string> parameters);
    string EncryptPaymentData(string data, string workingKey);
    string DecryptPaymentData(string encData, string workingKey);
}

public class PaymentGatewayService : IPaymentGatewayService
{
    private readonly IApplicationDbContext _context;

    public PaymentGatewayService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentGatewaySettings?> GetActiveGatewayAsync()
    {
        return await _context.PaymentGatewaySettings
            .Where(g => g.IsEnabled)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GeneratePaymentUrlAsync(PaymentGatewayRequestDto request)
    {
        var gateway = await GetActiveGatewayAsync();
        if (gateway == null)
        {
            throw new InvalidOperationException("No active payment gateway found");
        }

        // Prepare payment data
        var paymentData = new Dictionary<string, string>
        {
            {"merchant_id", gateway.MerchantId},
            {"order_id", request.PaymentReference},
            {"amount", request.Amount.ToString("F2")},
            {"currency", request.Currency},
            {"redirect_url", request.ReturnUrl},
            {"cancel_url", request.CancelUrl},
            {"billing_name", request.CustomerName},
            {"billing_email", request.CustomerEmail},
            {"billing_tel", request.CustomerPhone},
            {"merchant_param1", request.PaymentReference},
            {"merchant_param2", "SpirithubCafe"}
        };

        // Convert to query string
        var queryString = string.Join("&", paymentData.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        
        // Encrypt the data
        var encryptedData = EncryptPaymentData(queryString, gateway.WorkingKey);
        
        // Get gateway URL
        var gatewayUrl = gateway.IsSandboxMode ? gateway.SandboxGatewayUrl : gateway.LiveGatewayUrl;
        
        // Create the final payment URL
        return $"{gatewayUrl}?encRequest={Uri.EscapeDataString(encryptedData)}&access_code={gateway.AccessCode}";
    }

    public async Task<bool> ValidateCallbackAsync(Dictionary<string, string> parameters)
    {
        var gateway = await GetActiveGatewayAsync();
        if (gateway == null) return false;

        if (!parameters.ContainsKey("encResp"))
            return false;

        try
        {
            var decryptedData = DecryptPaymentData(parameters["encResp"], gateway.WorkingKey);
            var callbackParams = ParseQueryString(decryptedData);
            
            // Validate merchant ID
            if (!callbackParams.ContainsKey("merchant_id") || 
                callbackParams["merchant_id"] != gateway.MerchantId)
                return false;

            // Validate order status
            return callbackParams.ContainsKey("order_status");
        }
        catch
        {
            return false;
        }
    }

    public async Task<PaymentCallbackDto> ProcessCallbackAsync(Dictionary<string, string> parameters)
    {
        var gateway = await GetActiveGatewayAsync();
        if (gateway == null)
        {
            throw new InvalidOperationException("No active payment gateway found");
        }

        if (!parameters.ContainsKey("encResp"))
        {
            throw new ArgumentException("Missing encrypted response");
        }

        var decryptedData = DecryptPaymentData(parameters["encResp"], gateway.WorkingKey);
        var callbackParams = ParseQueryString(decryptedData);

        var callback = new PaymentCallbackDto
        {
            PaymentReference = callbackParams.GetValueOrDefault("merchant_param1", ""),
            TransactionId = callbackParams.GetValueOrDefault("tracking_id", ""),
            PaymentMethod = callbackParams.GetValueOrDefault("payment_mode", ""),
            GatewayResponse = JsonSerializer.Serialize(callbackParams)
        };

        // Map order status to our payment status
        var orderStatus = callbackParams.GetValueOrDefault("order_status", "").ToLower();
        callback.Status = orderStatus switch
        {
            "success" => "Completed",
            "shipped" => "Completed",
            "delivered" => "Completed",
            "failure" => "Failed",
            "aborted" => "Cancelled",
            "invalid" => "Failed",
            _ => "Pending"
        };

        if (callback.Status == "Failed" || callback.Status == "Cancelled")
        {
            callback.ErrorMessage = callbackParams.GetValueOrDefault("failure_message", "Payment failed");
        }

        return callback;
    }

    public string EncryptPaymentData(string data, string workingKey)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(workingKey);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = MD5.HashData(key);
            aes.IV = new byte[16]; // Use zero IV for consistency

            using var encryptor = aes.CreateEncryptor();
            var encrypted = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Convert.ToHexString(encrypted).ToLower();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to encrypt payment data", ex);
        }
    }

    public string DecryptPaymentData(string encData, string workingKey)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(workingKey);
            var encryptedBytes = Convert.FromHexString(encData);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = MD5.HashData(key);
            aes.IV = new byte[16]; // Use zero IV for consistency

            using var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to decrypt payment data", ex);
        }
    }

    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var result = new Dictionary<string, string>();
        var pairs = queryString.Split('&');
        
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                result[keyValue[0]] = Uri.UnescapeDataString(keyValue[1]);
            }
        }
        
        return result;
    }
}