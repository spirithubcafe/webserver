using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SpirithubCafe.Application.Services;

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

        // STEP 1: String Formation as per Official SmartPay Documentation
        // Parameters in order as per Section 7 of SmartPay documentation
        var requestParams = new Dictionary<string, string>();
        
        // Mandatory parameters (Section 7.1)
        requestParams["merchant_id"] = gateway.MerchantId;
        requestParams["order_id"] = request.PaymentReference;
        requestParams["amount"] = request.Amount.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
        requestParams["currency"] = request.Currency;
        requestParams["redirect_url"] = request.ReturnUrl;
        requestParams["cancel_url"] = request.CancelUrl;
        
        // Optional billing parameters (Section 7.2)
        if (!string.IsNullOrEmpty(request.CustomerName))
            requestParams["billing_name"] = request.CustomerName;
        if (!string.IsNullOrEmpty(request.CustomerEmail))
            requestParams["billing_email"] = request.CustomerEmail;
        
        // Additional optional parameters
        requestParams["billing_country"] = "OM"; // Default for Bank Muscat
        
        // Add merchant parameters for tracking
        requestParams["merchant_param1"] = $"Order_{request.PaymentReference}";
        requestParams["merchant_param2"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // STEP 2: Create parameter string without URL encoding (as per documentation)
        var parameterString = string.Join("&", requestParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        // STEP 3: Encrypt the parameter string using AES-256-GCM
        var encryptedRequest = EncryptPaymentData(parameterString, gateway.WorkingKey);

        // Return encrypted request - will be used in POST form or GET query
        return encryptedRequest;
    }

    public async Task<bool> ValidateCallbackAsync(Dictionary<string, string> parameters)
    {
        var gateway = await GetActiveGatewayAsync();
        if (gateway == null) return false;

        // Check for required response parameters as per Section 10 of documentation
        if (!parameters.ContainsKey("order_id") || !parameters.ContainsKey("encResponse"))
            return false;

        try
        {
            // Decrypt the response using working key
            var decryptedData = DecryptPaymentData(parameters["encResponse"], gateway.WorkingKey);
            var responseParams = ParseQueryString(decryptedData);
            
            // Validate order_id matches between encrypted and plain response
            if (!responseParams.ContainsKey("order_id") || 
                responseParams["order_id"] != parameters["order_id"])
                return false;

            // Validate required response parameters exist
            var requiredFields = new[] { "tracking_id", "order_status", "status_code", "amount" };
            foreach (var field in requiredFields)
            {
                if (!responseParams.ContainsKey(field))
                    return false;
            }

            return true;
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

        // Validate required parameters as per Section 10 of SmartPay documentation
        foreach (var key in new[] { "order_id", "encResponse" })
        {
            if (!parameters.ContainsKey(key) || string.IsNullOrEmpty(parameters[key]))
            {
                throw new ArgumentException($"Invalid response from payment gateway [{key}] missing");
            }
        }

        string decryptedString;
        try
        {
            // STEP 2: Response Decryption as per documentation
            decryptedString = DecryptPaymentData(parameters["encResponse"], gateway.WorkingKey);
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid response from payment gateway");
        }

        if (string.IsNullOrEmpty(decryptedString))
        {
            throw new ArgumentException("Invalid response from payment gateway - decryption failed");
        }

        // STEP 3: Response Validation and Business Logic as per documentation
        var responseData = ParseQueryString(decryptedString);

        // Validate required response parameters as per Section 9
        var requiredFields = new[] { "order_id", "tracking_id", "order_status", "status_code", "amount" };
        foreach (var field in requiredFields)
        {
            if (!responseData.ContainsKey(field))
            {
                throw new ArgumentException($"Invalid response from payment gateway [{field}] missing");
            }
        }

        // Validate order_id matches between encrypted response and plain parameter
        if (responseData["order_id"] != parameters["order_id"])
        {
            throw new ArgumentException("Order ID mismatch between encrypted and plain response");
        }

        // Create callback DTO based on official response parameters (Section 9)
        var callback = new PaymentCallbackDto
        {
            PaymentReference = responseData["order_id"],
            TransactionId = responseData["tracking_id"],
            PaymentMethod = responseData.GetValueOrDefault("payment_mode", ""),
            GatewayResponse = JsonSerializer.Serialize(responseData),
            ErrorMessage = responseData.GetValueOrDefault("failure_message", "")
        };

        // Map order status according to SmartPay official status codes (Section 11.2)
        var orderStatus = responseData["order_status"].ToUpper();
        callback.Status = orderStatus switch
        {
            "SUCCESS" => "Completed",
            "FAILURE" => "Failed", 
            "ABORTED" => "Cancelled",
            "INVALID" => "Failed",
            "INITIATED" => "Pending",
            "AWAITED" => "Pending",
            "TIMEOUT" => "Failed",
            _ => "Unknown"
        };

        // Set error message for failed transactions
        if (callback.Status == "Failed" || callback.Status == "Cancelled")
        {
            callback.ErrorMessage = responseData.GetValueOrDefault("failure_message", 
                responseData.GetValueOrDefault("status_message", "Payment failed"));
        }

        return callback;
    }



    public string EncryptPaymentData(string data, string workingKey)
    {
        try
        {
            // 1) derive key bytes (UTF8) and normalize to 32 bytes
            var keyBytes = NormalizeKeyBytes(Encoding.UTF8.GetBytes(workingKey));

            // 2) generate random 16-byte IV (guide says 16 bytes)
            var random = new SecureRandom();
            var iv = new byte[16];
            random.NextBytes(iv);

            // 3) plaintext bytes
            var pt = Encoding.UTF8.GetBytes(data);

            // 4) prepare GCM engine (AES)
            var gcm = new GcmBlockCipher(new AesEngine());
            // Tag length in bits (128 -> 16 bytes)
            var aeadParams = new AeadParameters(new KeyParameter(keyBytes), 128, iv, null); // null = no AAD
            gcm.Init(true, aeadParams);

            // 5) encrypt (process bytes + doFinal will append tag)
            var outBuf = new byte[gcm.GetOutputSize(pt.Length)];
            int len = gcm.ProcessBytes(pt, 0, pt.Length, outBuf, 0);
            try
            {
                gcm.DoFinal(outBuf, len);
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption failed: " + ex.Message);
            }

            // outBuf contains ciphertext || tag
            // 6) return HEX(IV) + HEX(ciphertext+tag)
            return ToHex(iv) + ToHex(outBuf);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Encryption failed.", ex);
        }
    }

    public string DecryptPaymentData(string encData, string workingKey)
    {
        try
        {
            var keyBytes = NormalizeKeyBytes(Encoding.UTF8.GetBytes(workingKey));

            // first 32 hex chars = 16 bytes IV
            if (encData.Length < 32) throw new ArgumentException("Encrypted string too short to contain IV");
            string ivHex = encData.Substring(0, 32);
            string cipherHex = encData.Substring(32);

            var iv = HexToBytes(ivHex);
            var cipherWithTag = HexToBytes(cipherHex);

            var gcm = new GcmBlockCipher(new AesEngine());
            var aeadParams = new AeadParameters(new KeyParameter(keyBytes), 128, iv, null);
            gcm.Init(false, aeadParams);

            var outBuf = new byte[gcm.GetOutputSize(cipherWithTag.Length)];
            int len = gcm.ProcessBytes(cipherWithTag, 0, cipherWithTag.Length, outBuf, 0);
            try
            {
                gcm.DoFinal(outBuf, len);
            }
            catch (Exception ex)
            {
                // authentication failed (bad tag) or other problem
                throw new Exception("Decryption failed / authentication failed: " + ex.Message);
            }

            // trim possible zero padding at the end of outBuf
            int actualLen = outBuf.Length;
            while (actualLen > 0 && outBuf[actualLen - 1] == 0) actualLen--;
            var pt = new byte[actualLen];
            Array.Copy(outBuf, 0, pt, 0, actualLen);

            return Encoding.UTF8.GetString(pt);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed.", ex);
        }
    }

    // Helper: convert byte[] -> hex
    private static string ToHex(byte[] data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (var b in data) sb.AppendFormat("{0:x2}", b);
        return sb.ToString();
    }

    // Helper: convert hex -> byte[]
    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0) throw new ArgumentException("Hex string must have even length");
        var result = new byte[hex.Length / 2];
        for (int i = 0; i < result.Length; i++)
            result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return result;
    }

    // Ensure 32-byte key for AES-256: if provided key bytes are shorter, pad with 0; if longer truncate.
    private static byte[] NormalizeKeyBytes(byte[] key)
    {
        const int KEY_LEN = 32; // 256 bits
        if (key.Length == KEY_LEN) return key;
        var k = new byte[KEY_LEN];
        Array.Copy(key, 0, k, 0, Math.Min(key.Length, KEY_LEN));
        return k;
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