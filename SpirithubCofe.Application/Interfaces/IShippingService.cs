using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.Interfaces;

public interface IShippingService
{
    // Countries
    Task<List<Country>> GetCountriesAsync();
    Task<List<Country>> GetActiveCountriesAsync();
    Task<List<Country>> GetGccCountriesAsync();
    Task<Country?> GetCountryByIdAsync(int id);
    Task<Country?> GetCountryByCodeAsync(string code);
    Task<int> CreateCountryAsync(Country country);
    Task<bool> UpdateCountryAsync(Country country);
    Task<bool> DeleteCountryAsync(int id);

    // Cities
    Task<List<City>> GetCitiesAsync();
    Task<List<City>> GetActiveCitiesAsync();
    Task<List<City>> GetCitiesByCountryAsync(int countryId);
    Task<City?> GetCityByIdAsync(int id);
    Task<int> CreateCityAsync(City city);
    Task<bool> UpdateCityAsync(City city);
    Task<bool> DeleteCityAsync(int id);

    // Shipping Methods
    Task<List<ShippingMethod>> GetShippingMethodsAsync();
    Task<List<ShippingMethod>> GetActiveShippingMethodsAsync();
    Task<ShippingMethod?> GetShippingMethodByIdAsync(int id);
    Task<ShippingMethod?> GetShippingMethodByTypeAsync(ShippingMethodType type);
    Task<int> CreateShippingMethodAsync(ShippingMethod method);
    Task<bool> UpdateShippingMethodAsync(ShippingMethod method);
    Task<bool> DeleteShippingMethodAsync(int id);

    // Shipping Zones
    Task<List<ShippingZone>> GetShippingZonesAsync();
    Task<List<ShippingZone>> GetActiveShippingZonesAsync();
    Task<List<ShippingZone>> GetShippingZonesByMethodAsync(int methodId);
    Task<ShippingZone?> GetShippingZoneByIdAsync(int id);
    Task<int> CreateShippingZoneAsync(ShippingZone zone);
    Task<bool> UpdateShippingZoneAsync(ShippingZone zone);
    Task<bool> DeleteShippingZoneAsync(int id);

    // Shipping Rates
    Task<List<ShippingRate>> GetShippingRatesAsync();
    Task<List<ShippingRate>> GetActiveShippingRatesAsync();
    Task<List<ShippingRate>> GetShippingRatesByZoneAsync(int zoneId);
    Task<ShippingRate?> GetShippingRateByIdAsync(int id);
    Task<ShippingRate?> GetShippingRateAsync(int zoneId, int cityId);
    Task<int> CreateShippingRateAsync(ShippingRate rate);
    Task<bool> UpdateShippingRateAsync(ShippingRate rate);
    Task<bool> DeleteShippingRateAsync(int id);

    // Calculate shipping
    Task<List<ShippingOption>> CalculateShippingAsync(int cityId, decimal orderTotal, decimal orderWeight = 0);
    Task<bool> SeedDefaultDataAsync();
}

public class ShippingOption
{
    public int ShippingMethodId { get; set; }
    public string Name { get; set; } = "";
    public string NameAr { get; set; } = "";
    public string Description { get; set; } = "";
    public string DescriptionAr { get; set; } = "";
    public decimal Cost { get; set; }
    public int EstimatedDays { get; set; }
    public ShippingMethodType Type { get; set; }
    public bool IsAvailable { get; set; } = true;
}