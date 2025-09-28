using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;
using System.Text.Json;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service for managing shipping methods and rates
/// </summary>
public class ShippingMethodService
{
    private readonly IApplicationDbContext _context;

    public ShippingMethodService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Shipping Methods CRUD
    public async Task<List<ShippingMethod>> GetAllShippingMethodsAsync()
    {
        return await _context.ShippingMethods
            .Include(s => s.NoolRates)
                .ThenInclude(r => r.City)
                    .ThenInclude(c => c.Country)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingMethod>> GetActiveShippingMethodsAsync()
    {
        return await _context.ShippingMethods
            .Include(s => s.NoolRates)
                .ThenInclude(r => r.City)
                    .ThenInclude(c => c.Country)
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<ShippingMethod?> GetShippingMethodByIdAsync(int id)
    {
        return await _context.ShippingMethods
            .Include(s => s.NoolRates)
                .ThenInclude(r => r.City)
                    .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ShippingMethod?> GetShippingMethodByTypeAsync(string type)
    {
        return await _context.ShippingMethods
            .Include(s => s.NoolRates)
                .ThenInclude(r => r.City)
                    .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(s => s.Type == type);
    }

    public async Task<ShippingMethod> CreateShippingMethodAsync(ShippingMethod shippingMethod)
    {
        shippingMethod.CreatedAt = DateTime.UtcNow;
        shippingMethod.UpdatedAt = DateTime.UtcNow;

        _context.ShippingMethods.Add(shippingMethod);
        await _context.SaveChangesAsync();
        return shippingMethod;
    }

    public async Task<ShippingMethod> UpdateShippingMethodAsync(ShippingMethod shippingMethod)
    {
        shippingMethod.UpdatedAt = DateTime.UtcNow;

        _context.ShippingMethods.Update(shippingMethod);
        await _context.SaveChangesAsync();
        return shippingMethod;
    }

    public async Task<bool> DeleteShippingMethodAsync(int id)
    {
        var shippingMethod = await _context.ShippingMethods.FindAsync(id);
        if (shippingMethod == null) return false;

        _context.ShippingMethods.Remove(shippingMethod);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleShippingMethodStatusAsync(int id)
    {
        var shippingMethod = await _context.ShippingMethods.FindAsync(id);
        if (shippingMethod == null) return false;

        shippingMethod.IsActive = !shippingMethod.IsActive;
        shippingMethod.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Nool Shipping Rates CRUD
    public async Task<List<NoolShippingRate>> GetNoolRatesAsync(int shippingMethodId)
    {
        return await _context.NoolShippingRates
            .Include(r => r.City)
                .ThenInclude(c => c.Country)
            .Where(r => r.ShippingMethodId == shippingMethodId)
            .OrderBy(r => r.City.Country.Name)
            .ThenBy(r => r.City.Name)
            .ToListAsync();
    }

    public async Task<NoolShippingRate?> GetNoolRateAsync(int shippingMethodId, int cityId)
    {
        return await _context.NoolShippingRates
            .Include(r => r.City)
                .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(r => r.ShippingMethodId == shippingMethodId && r.CityId == cityId);
    }

    public async Task<NoolShippingRate> CreateOrUpdateNoolRateAsync(int shippingMethodId, int cityId, decimal rate)
    {
        var existingRate = await _context.NoolShippingRates
            .FirstOrDefaultAsync(r => r.ShippingMethodId == shippingMethodId && r.CityId == cityId);

        if (existingRate != null)
        {
            existingRate.Rate = rate;
            existingRate.UpdatedAt = DateTime.UtcNow;
            existingRate.IsActive = true;
            await _context.SaveChangesAsync();
            return existingRate;
        }

        var newRate = new NoolShippingRate
        {
            ShippingMethodId = shippingMethodId,
            CityId = cityId,
            Rate = rate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.NoolShippingRates.Add(newRate);
        await _context.SaveChangesAsync();
        return newRate;
    }

    public async Task<bool> DeleteNoolRateAsync(int id)
    {
        var rate = await _context.NoolShippingRates.FindAsync(id);
        if (rate == null) return false;

        _context.NoolShippingRates.Remove(rate);
        await _context.SaveChangesAsync();
        return true;
    }

    // Aramex Configuration
    public async Task<Dictionary<string, object>?> GetAramexConfigurationAsync()
    {
        var aramexMethod = await GetShippingMethodByTypeAsync("Aramex");
        if (aramexMethod?.ApiConfiguration == null) return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(aramexMethod.ApiConfiguration);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAramexConfigurationAsync(Dictionary<string, object> configuration)
    {
        var aramexMethod = await GetShippingMethodByTypeAsync("Aramex");
        if (aramexMethod == null) return false;

        aramexMethod.ApiConfiguration = JsonSerializer.Serialize(configuration);
        aramexMethod.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Calculate shipping cost
    public async Task<decimal> CalculateShippingCostAsync(string shippingType, int? countryId, int? cityId)
    {
        var shippingMethod = await GetShippingMethodByTypeAsync(shippingType);
        if (shippingMethod == null || !shippingMethod.IsActive) return 0;

        switch (shippingType.ToLower())
        {
            case "pickup":
                return 0; // Always free

            case "nooloman":
                if (cityId.HasValue)
                {
                    var rate = await GetNoolRateAsync(shippingMethod.Id, cityId.Value);
                    return rate?.Rate ?? 0;
                }
                return 0;

            case "aramex":
                // TODO: Implement Aramex API call for real-time calculation
                // For now, return 0 - this should be replaced with actual API integration
                return 0;

            default:
                return 0;
        }
    }
}