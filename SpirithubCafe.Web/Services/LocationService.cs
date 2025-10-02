using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Service for managing countries and cities for shipping
/// </summary>
public class LocationService
{
    private readonly IApplicationDbContext _context;

    public LocationService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Countries
    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return await _context.Countries
            .Include(c => c.Cities.Where(city => city.IsActive))
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Country?> GetCountryByIdAsync(int id)
    {
        return await _context.Countries
            .Include(c => c.Cities.Where(city => city.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }

    public async Task<Country?> GetCountryByCodeAsync(string code)
    {
        return await _context.Countries
            .Include(c => c.Cities.Where(city => city.IsActive))
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
    }

    // Cities
    public async Task<List<City>> GetAllCitiesAsync()
    {
        return await _context.Cities
            .Include(c => c.Country)
            .Where(c => c.IsActive && c.Country!.IsActive)
            .OrderBy(c => c.Country!.Name)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<City>> GetCitiesByCountryIdAsync(int countryId)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .Where(c => c.CountryId == countryId && c.IsActive && c.Country!.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }
}