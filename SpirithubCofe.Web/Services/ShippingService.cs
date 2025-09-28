using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service to manage shipping countries and cities (admin + frontend)
/// </summary>
public class ShippingService
{
    private readonly ApplicationDbContext _context;

    public ShippingService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Countries
    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return await _context.Countries
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Country>> GetActiveCountriesAsync()
    {
        return await _context.Countries
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Country?> GetCountryByIdAsync(int id)
    {
        return await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Country> CreateCountryAsync(Country country)
    {
        // ensure unique code
        if (!string.IsNullOrWhiteSpace(country.Code))
        {
            var exists = await _context.Countries.AnyAsync(c => c.Code == country.Code);
            if (exists)
                throw new InvalidOperationException($"Country with code '{country.Code}' already exists.");
        }

        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return country;
    }

    public async Task<Country> UpdateCountryAsync(Country country)
    {
        var existing = await _context.Countries.FindAsync(country.Id);
        if (existing == null) throw new InvalidOperationException("Country not found");

        if (!string.IsNullOrWhiteSpace(country.Code))
        {
            var exists = await _context.Countries.AnyAsync(c => c.Code == country.Code && c.Id != country.Id);
            if (exists) throw new InvalidOperationException($"Country with code '{country.Code}' already exists.");
        }

        existing.Name = country.Name;
        existing.NameAr = country.NameAr;
        existing.Code = country.Code;
        existing.IsActive = country.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteCountryAsync(int id)
    {
        var country = await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (country == null) return false;

        if (country.Cities.Any())
            throw new InvalidOperationException($"Cannot delete country '{country.Name}' because it has {country.Cities.Count} cities. Remove or reassign cities first.");

        _context.Countries.Remove(country);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ToggleCountryStatusAsync(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null) return false;
        country.IsActive = !country.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    // Cities
    public async Task<List<City>> GetCitiesByCountryAsync(int countryId)
    {
        return await _context.Cities
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<City> CreateCityAsync(City city)
    {
        // optional: ensure same-named city doesn't exist in country
        var exists = await _context.Cities.AnyAsync(c => c.CountryId == city.CountryId && c.Name == city.Name);
        if (exists) throw new InvalidOperationException($"City '{city.Name}' already exists for this country.");

        _context.Cities.Add(city);
        await _context.SaveChangesAsync();
        return city;
    }

    public async Task<City> UpdateCityAsync(City city)
    {
        var existing = await _context.Cities.FindAsync(city.Id);
        if (existing == null) throw new InvalidOperationException("City not found");

        existing.Name = city.Name;
        existing.NameAr = city.NameAr;
        existing.Code = city.Code;
        existing.IsActive = city.IsActive;
        existing.CountryId = city.CountryId;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return false;

        _context.Cities.Remove(city);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ToggleCityStatusAsync(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return false;
        city.IsActive = !city.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }
}
