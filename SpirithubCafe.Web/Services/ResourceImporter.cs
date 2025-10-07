using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public class ResourceImporter
{
    private readonly ApplicationDbContext _context;

    public ResourceImporter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ImportResourcesAsync()
    {
        var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "SpirithubCafe.Langs");
        
        // Load both English and Arabic resources
        var enResources = LoadResxFile(Path.Combine(basePath, "Resources.resx"));
        var arResources = LoadResxFile(Path.Combine(basePath, "Resources.ar.resx"));
        
        // Merge resources by key
        var allKeys = enResources.Keys.Union(arResources.Keys).Distinct();
        
        foreach (var key in allKeys)
        {
            var valueEn = enResources.ContainsKey(key) ? enResources[key] : key;
            var valueAr = arResources.ContainsKey(key) ? arResources[key] : key;
            
            // Check if translation already exists
            var existing = await _context.Translations
                .FirstOrDefaultAsync(t => t.Key == key);

            if (existing == null)
            {
                var translation = new Translation
                {
                    Key = key,
                    ValueEn = valueEn,
                    ValueAr = valueAr,
                    Category = DetermineCategory(key),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Translations.Add(translation);
            }
            else
            {
                // Update existing translation
                existing.ValueEn = valueEn;
                existing.ValueAr = valueAr;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        await _context.SaveChangesAsync();
    }

    private Dictionary<string, string> LoadResxFile(string filePath)
    {
        var result = new Dictionary<string, string>();
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return result;
        }

        var doc = XDocument.Load(filePath);
        var dataElements = doc.Descendants("data");

        foreach (var element in dataElements)
        {
            var key = element.Attribute("name")?.Value;
            var value = element.Element("value")?.Value;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                result[key] = value;
            }
        }
        
        return result;
    }

    private string DetermineCategory(string key)
    {
        if (key.StartsWith("Hero") || key.StartsWith("Home"))
            return "HomePage";
        if (key.StartsWith("Product"))
            return "Product";
        if (key.StartsWith("Cart") || key.StartsWith("Checkout"))
            return "Cart";
        if (key.StartsWith("Order"))
            return "Order";
        if (key.StartsWith("User") || key.StartsWith("Account"))
            return "Account";
        if (key.StartsWith("Admin"))
            return "Admin";
        if (key.StartsWith("Email"))
            return "Email";
        if (key.StartsWith("Payment"))
            return "Payment";
        if (key.StartsWith("Shipping"))
            return "Shipping";
        if (key.StartsWith("Footer"))
            return "Footer";
        if (key.StartsWith("Contact"))
            return "Contact";
        if (key.StartsWith("About"))
            return "About";
        if (key.StartsWith("FAQ"))
            return "FAQ";
        if (key.StartsWith("Review"))
            return "Review";
        if (key.StartsWith("Category") || key.StartsWith("Categories"))
            return "Category";
        
        return "UI";
    }
}
