using Microsoft.AspNetCore.Identity;
using System;

class Program
{
    static void Main()
    {
        var hasher = new PasswordHasher<object>();
        var hash = hasher.HashPassword(new object(), "Test123!");
        
        Console.WriteLine("Password Hash for 'Test123!':");
        Console.WriteLine(hash);
        
        // Test verification
        var verificationResult = hasher.VerifyHashedPassword(new object(), hash, "Test123!");
        Console.WriteLine($"Verification result: {verificationResult}");
    }
}