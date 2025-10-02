using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel;

namespace SpirithubCafe.Application.Services;

/// <summary>
/// Service for calculating shipping rates via Aramex API
/// </summary>
public interface IAramexRateService
{
    Task<AramexRateResult> CalculateRateAsync(AramexRateRequest request);
    Task<AramexSettings?> GetAramexSettingsAsync();
    Task<List<string>> GetAvailableServicesAsync(bool isDomestic);
}

public class AramexRateService : IAramexRateService
{
    private readonly IApplicationDbContext _context;

    public AramexRateService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AramexRateResult> CalculateRateAsync(AramexRateRequest request)
    {
        try
        {
            var settings = await GetAramexSettingsAsync();
            if (settings == null)
            {
                return new AramexRateResult
                {
                    Success = false,
                    ErrorMessage = "Aramex settings not configured"
                };
            }

            // TODO: Implement actual Aramex Rate Calculator API integration
            // For now, return mock data to test the UI
            await Task.Delay(1000); // Simulate API call delay

            var mockRate = CalculateMockRate(request, settings);

            return new AramexRateResult
            {
                Success = true,
                TotalAmount = mockRate,
                Currency = request.Currency ?? "OMR",
                TransactionId = $"TEST_{DateTime.UtcNow:yyyyMMddHHmmss}",
                RateBreakdown = new List<AramexRateBreakdown>
                {
                    new AramexRateBreakdown
                    {
                        Description = "Base Rate",
                        Amount = mockRate * 0.8m,
                        Currency = request.Currency ?? "OMR"
                    },
                    new AramexRateBreakdown
                    {
                        Description = "Fuel Surcharge",
                        Amount = mockRate * 0.15m,
                        Currency = request.Currency ?? "OMR"
                    },
                    new AramexRateBreakdown
                    {
                        Description = "Service Fee",
                        Amount = mockRate * 0.05m,
                        Currency = request.Currency ?? "OMR"
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new AramexRateResult
            {
                Success = false,
                ErrorMessage = $"Error calculating rate: {ex.Message}"
            };
        }
    }

    private decimal CalculateMockRate(AramexRateRequest request, AramexSettings settings)
    {
        // Simple mock calculation based on weight and destination
        decimal baseRate = 2.000m; // Base rate in OMR
        decimal weightMultiplier = request.Weight * 0.5m;
        decimal distanceMultiplier = 1.0m;

        // Adjust based on destination country
        switch (request.DestinationCountryCode?.ToUpper())
        {
            case "AE": // UAE
                distanceMultiplier = 1.2m;
                break;
            case "SA": // Saudi Arabia
                distanceMultiplier = 1.5m;
                break;
            case "US": // USA
                distanceMultiplier = 3.0m;
                break;
            case "GB": // UK
                distanceMultiplier = 2.8m;
                break;
            default:
                distanceMultiplier = 2.0m;
                break;
        }

        // Adjust based on product type
        switch (request.ProductType?.ToUpper())
        {
            case "EPX": // Express
                distanceMultiplier *= 1.5m;
                break;
            case "PPX": // Priority
                distanceMultiplier *= 1.3m;
                break;
            case "GRD": // Ground
                distanceMultiplier *= 1.0m;
                break;
            case "OND": // Domestic
                distanceMultiplier *= 0.8m;
                break;
        }

        return Math.Round(baseRate + weightMultiplier + distanceMultiplier, 3);
    }

    public async Task<AramexSettings?> GetAramexSettingsAsync()
    {
        return await _context.AramexSettings.FirstOrDefaultAsync();
    }

    public async Task<List<string>> GetAvailableServicesAsync(bool isDomestic)
    {
        var settings = await GetAramexSettingsAsync();
        if (settings == null) return new List<string>();

        var services = isDomestic 
            ? settings.DomesticServices.Split(',', StringSplitOptions.RemoveEmptyEntries)
            : settings.InternationalServices.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return services.Select(s => s.Trim()).ToList();
    }
}

/// <summary>
/// Aramex rate calculation request
/// </summary>
public class AramexRateRequest
{
    public string DestinationAddress { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string? DestinationState { get; set; }
    public string? DestinationPostalCode { get; set; }
    public string DestinationCountryCode { get; set; } = string.Empty;
    
    public decimal Weight { get; set; } = 1.0m;
    public decimal Length { get; set; } = 10;
    public decimal Width { get; set; } = 10;
    public decimal Height { get; set; } = 10;
    public int NumberOfPieces { get; set; } = 1;
    
    public string Description { get; set; } = "Test Package";
    public string? ProductGroup { get; set; } = "EXP";
    public string? ProductType { get; set; } = "EPX";
    public string? Services { get; set; }
    public string? Currency { get; set; } = "OMR";
    
    public decimal CodAmount { get; set; } = 0;
    public decimal InsuranceAmount { get; set; } = 0;
}

/// <summary>
/// Aramex rate calculation result
/// </summary>
public class AramexRateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "OMR";
    public string? TransactionId { get; set; }
    public List<AramexRateBreakdown> RateBreakdown { get; set; } = new();
}

/// <summary>
/// Aramex rate breakdown item
/// </summary>
public class AramexRateBreakdown
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "OMR";
}