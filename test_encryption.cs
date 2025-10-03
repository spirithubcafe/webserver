using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class TestEncryption
{
    public static void Main()
    {
        // Test encryption with Bank Muscat sample data
        var workingKey = "841FEAE32609C3E892C4D0B1393A7ACC";
        
        // Sample payment data like in production
        var paymentParameters = new Dictionary<string, string>
        {
            {"tid", "123456"},
            {"merchant_id", "224"},
            {"order_id", "ORDER123"},
            {"amount", "10.00"},
            {"currency", "OMR"},
            {"redirect_url", "http://localhost:5212/api/payment/callback/success"},
            {"cancel_url", "http://localhost:5212/api/payment/callback/cancel"},
            {"language", "EN"},
            {"billing_name", "Test User"},
            {"billing_email", "test@example.com"},
            {"billing_tel", ""},
            {"merchant_param1", "ORDER123"},
            {"merchant_param2", "SpirithubCafe"}
        };

        // Sort keys alphabetically like in Go mapToString
        var sortedParams = paymentParameters.OrderBy(kvp => kvp.Key);
        var queryString = string.Join("&", sortedParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        
        Console.WriteLine("Query string to encrypt:");
        Console.WriteLine(queryString);
        Console.WriteLine();
        
        // Encrypt
        var encrypted = EncryptPaymentData(queryString, workingKey);
        Console.WriteLine("Encrypted result:");
        Console.WriteLine(encrypted);
        Console.WriteLine($"Length: {encrypted.Length}");
        Console.WriteLine();
        
        // Try decrypt to verify
        try
        {
            var decrypted = DecryptPaymentData(encrypted, workingKey);
            Console.WriteLine("Decrypted result:");
            Console.WriteLine(decrypted);
            Console.WriteLine($"Match: {decrypted == queryString}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decryption failed: {ex.Message}");
        }
    }
    
    public static string EncryptPaymentData(string plaintext, string workingKey)
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
    
    public static string DecryptPaymentData(string encryptedDataHex, string workingKey)
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
            return Encoding.UTF8.GetString(plaintextBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Decryption failed: {ex.Message}", ex);
        }
    }
}