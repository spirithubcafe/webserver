using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Web.Services;

public class ShippingService(IApplicationDbContext context) : IShippingService
{
    private readonly IApplicationDbContext _context = context;

    #region Countries

    public async Task<List<Country>> GetCountriesAsync()
    {
        return await _context.Countries
            .Include(c => c.Cities)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Country>> GetActiveCountriesAsync()
    {
        return await _context.Countries
            .Where(c => c.IsActive)
            .Include(c => c.Cities.Where(city => city.IsActive))
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Country>> GetGccCountriesAsync()
    {
        return await _context.Countries
            .Where(c => c.IsActive && c.IsGccCountry)
            .Include(c => c.Cities.Where(city => city.IsActive))
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Country?> GetCountryByIdAsync(int id)
    {
        return await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Country?> GetCountryByCodeAsync(string code)
    {
        return await _context.Countries
            .Include(c => c.Cities.Where(city => city.IsActive))
            .FirstOrDefaultAsync(c => c.Code == code || c.Code2 == code);
    }

    public async Task<int> CreateCountryAsync(Country country)
    {
        country.CreatedAt = DateTime.UtcNow;
        country.UpdatedAt = DateTime.UtcNow;
        
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return country.Id;
    }

    public async Task<bool> UpdateCountryAsync(Country country)
    {
        country.UpdatedAt = DateTime.UtcNow;
        
        _context.Countries.Update(country);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCountryAsync(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null) return false;

        _context.Countries.Remove(country);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region Cities

    public async Task<List<City>> GetCitiesAsync()
    {
        return await _context.Cities
            .Include(c => c.Country)
            .OrderBy(c => c.Country!.Name)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<City>> GetActiveCitiesAsync()
    {
        return await _context.Cities
            .Where(c => c.IsActive && c.Country!.IsActive)
            .Include(c => c.Country)
            .OrderBy(c => c.Country!.Name)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<City>> GetCitiesByCountryAsync(int countryId)
    {
        return await _context.Cities
            .Where(c => c.CountryId == countryId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateCityAsync(City city)
    {
        city.CreatedAt = DateTime.UtcNow;
        city.UpdatedAt = DateTime.UtcNow;
        
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();
        return city.Id;
    }

    public async Task<bool> UpdateCityAsync(City city)
    {
        city.UpdatedAt = DateTime.UtcNow;
        
        _context.Cities.Update(city);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return false;

        _context.Cities.Remove(city);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region Shipping Methods

    public async Task<List<ShippingMethod>> GetShippingMethodsAsync()
    {
        return await _context.ShippingMethods
            .Include(sm => sm.ShippingZones)
            .ThenInclude(sz => sz.Country)
            .OrderBy(sm => sm.DisplayOrder)
            .ThenBy(sm => sm.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingMethod>> GetActiveShippingMethodsAsync()
    {
        return await _context.ShippingMethods
            .Where(sm => sm.IsActive)
            .Include(sm => sm.ShippingZones.Where(sz => sz.IsActive))
            .ThenInclude(sz => sz.Country)
            .OrderBy(sm => sm.DisplayOrder)
            .ThenBy(sm => sm.Name)
            .ToListAsync();
    }

    public async Task<ShippingMethod?> GetShippingMethodByIdAsync(int id)
    {
        return await _context.ShippingMethods
            .Include(sm => sm.ShippingZones)
            .ThenInclude(sz => sz.Country)
            .FirstOrDefaultAsync(sm => sm.Id == id);
    }

    public async Task<ShippingMethod?> GetShippingMethodByTypeAsync(ShippingMethodType type)
    {
        return await _context.ShippingMethods
            .Include(sm => sm.ShippingZones)
            .ThenInclude(sz => sz.Country)
            .FirstOrDefaultAsync(sm => sm.Type == type && sm.IsActive);
    }

    public async Task<int> CreateShippingMethodAsync(ShippingMethod method)
    {
        method.CreatedAt = DateTime.UtcNow;
        method.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingMethods.Add(method);
        await _context.SaveChangesAsync();
        return method.Id;
    }

    public async Task<bool> UpdateShippingMethodAsync(ShippingMethod method)
    {
        method.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingMethods.Update(method);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteShippingMethodAsync(int id)
    {
        var method = await _context.ShippingMethods.FindAsync(id);
        if (method == null) return false;

        _context.ShippingMethods.Remove(method);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region Shipping Zones

    public async Task<List<ShippingZone>> GetShippingZonesAsync()
    {
        return await _context.ShippingZones
            .Include(sz => sz.ShippingMethod)
            .Include(sz => sz.Country)
            .Include(sz => sz.ShippingRates)
            .ThenInclude(sr => sr.City)
            .OrderBy(sz => sz.DisplayOrder)
            .ThenBy(sz => sz.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingZone>> GetActiveShippingZonesAsync()
    {
        return await _context.ShippingZones
            .Where(sz => sz.IsActive && sz.ShippingMethod!.IsActive)
            .Include(sz => sz.ShippingMethod)
            .Include(sz => sz.Country)
            .Include(sz => sz.ShippingRates.Where(sr => sr.IsActive))
            .ThenInclude(sr => sr.City)
            .OrderBy(sz => sz.DisplayOrder)
            .ThenBy(sz => sz.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingZone>> GetShippingZonesByMethodAsync(int methodId)
    {
        return await _context.ShippingZones
            .Where(sz => sz.ShippingMethodId == methodId)
            .Include(sz => sz.Country)
            .Include(sz => sz.ShippingRates.Where(sr => sr.IsActive))
            .ThenInclude(sr => sr.City)
            .OrderBy(sz => sz.DisplayOrder)
            .ThenBy(sz => sz.Name)
            .ToListAsync();
    }

    public async Task<ShippingZone?> GetShippingZoneByIdAsync(int id)
    {
        return await _context.ShippingZones
            .Include(sz => sz.ShippingMethod)
            .Include(sz => sz.Country)
            .Include(sz => sz.ShippingRates)
            .ThenInclude(sr => sr.City)
            .FirstOrDefaultAsync(sz => sz.Id == id);
    }

    public async Task<int> CreateShippingZoneAsync(ShippingZone zone)
    {
        zone.CreatedAt = DateTime.UtcNow;
        zone.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingZones.Add(zone);
        await _context.SaveChangesAsync();
        return zone.Id;
    }

    public async Task<bool> UpdateShippingZoneAsync(ShippingZone zone)
    {
        zone.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingZones.Update(zone);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteShippingZoneAsync(int id)
    {
        var zone = await _context.ShippingZones.FindAsync(id);
        if (zone == null) return false;

        _context.ShippingZones.Remove(zone);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region Shipping Rates

    public async Task<List<ShippingRate>> GetShippingRatesAsync()
    {
        return await _context.ShippingRates
            .Include(sr => sr.ShippingZone)
            .ThenInclude(sz => sz!.ShippingMethod)
            .Include(sr => sr.City)
            .ThenInclude(c => c!.Country)
            .OrderBy(sr => sr.ShippingZone!.Name)
            .ThenBy(sr => sr.City!.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingRate>> GetActiveShippingRatesAsync()
    {
        return await _context.ShippingRates
            .Where(sr => sr.IsActive && sr.ShippingZone!.IsActive && sr.ShippingZone.ShippingMethod!.IsActive)
            .Include(sr => sr.ShippingZone)
            .ThenInclude(sz => sz!.ShippingMethod)
            .Include(sr => sr.City)
            .ThenInclude(c => c!.Country)
            .OrderBy(sr => sr.ShippingZone!.Name)
            .ThenBy(sr => sr.City!.Name)
            .ToListAsync();
    }

    public async Task<List<ShippingRate>> GetShippingRatesByZoneAsync(int zoneId)
    {
        return await _context.ShippingRates
            .Where(sr => sr.ShippingZoneId == zoneId)
            .Include(sr => sr.City)
            .ThenInclude(c => c!.Country)
            .OrderBy(sr => sr.City!.Name)
            .ToListAsync();
    }

    public async Task<ShippingRate?> GetShippingRateByIdAsync(int id)
    {
        return await _context.ShippingRates
            .Include(sr => sr.ShippingZone)
            .ThenInclude(sz => sz!.ShippingMethod)
            .Include(sr => sr.City)
            .ThenInclude(c => c!.Country)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<ShippingRate?> GetShippingRateAsync(int zoneId, int cityId)
    {
        return await _context.ShippingRates
            .Include(sr => sr.ShippingZone)
            .ThenInclude(sz => sz!.ShippingMethod)
            .Include(sr => sr.City)
            .FirstOrDefaultAsync(sr => sr.ShippingZoneId == zoneId && sr.CityId == cityId);
    }

    public async Task<int> CreateShippingRateAsync(ShippingRate rate)
    {
        rate.CreatedAt = DateTime.UtcNow;
        rate.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingRates.Add(rate);
        await _context.SaveChangesAsync();
        return rate.Id;
    }

    public async Task<bool> UpdateShippingRateAsync(ShippingRate rate)
    {
        rate.UpdatedAt = DateTime.UtcNow;
        
        _context.ShippingRates.Update(rate);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteShippingRateAsync(int id)
    {
        var rate = await _context.ShippingRates.FindAsync(id);
        if (rate == null) return false;

        _context.ShippingRates.Remove(rate);
        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region Calculate Shipping

    public async Task<List<ShippingOption>> CalculateShippingAsync(int cityId, decimal orderTotal, decimal orderWeight = 0)
    {
        var city = await GetCityByIdAsync(cityId);
        if (city?.Country == null) return [];

        var shippingOptions = new List<ShippingOption>();

        // Get all active shipping methods and their rates for this city
        var activeRates = await _context.ShippingRates
            .Where(sr => sr.CityId == cityId && sr.IsActive &&
                        sr.ShippingZone!.IsActive && 
                        sr.ShippingZone.ShippingMethod!.IsActive)
            .Include(sr => sr.ShippingZone)
            .ThenInclude(sz => sz!.ShippingMethod)
            .ToListAsync();

        foreach (var rate in activeRates)
        {
            var method = rate.ShippingZone!.ShippingMethod!;
            var cost = rate.Rate;

            // Apply free shipping threshold
            if (orderTotal >= rate.MinOrderAmount)
            {
                cost = 0;
            }

            // Special handling for pickup (always free)
            if (method.Type == ShippingMethodType.Pickup)
            {
                cost = 0;
            }

            shippingOptions.Add(new ShippingOption
            {
                ShippingMethodId = method.Id,
                Name = method.Name,
                NameAr = method.NameAr ?? "",
                Description = method.Description ?? "",
                DescriptionAr = method.DescriptionAr ?? "",
                Cost = cost,
                EstimatedDays = rate.EstimatedDays,
                Type = method.Type,
                IsAvailable = true
            });
        }

        return shippingOptions.OrderBy(so => so.Cost).ThenBy(so => so.EstimatedDays).ToList();
    }

    #endregion

    #region Seed Data

    public async Task<bool> SeedDefaultDataAsync()
    {
        var isCountriesSeeded = false;
        var isShippingMethodsSeeded = false;
        var isShippingZonesSeeded = false;
        var isShippingRatesSeeded = false;

        // Seed Countries if not exist
        if (!await _context.Countries.AnyAsync())
        {

        // GCC Countries with major cities
        var gccCountries = new List<Country>
        {
            new() { Name = "Oman", NameAr = "عُمان", Code = "OMN", Code2 = "OM", IsGccCountry = true, DisplayOrder = 1 },
            new() { Name = "United Arab Emirates", NameAr = "الإمارات العربية المتحدة", Code = "ARE", Code2 = "AE", IsGccCountry = true, DisplayOrder = 2 },
            new() { Name = "Saudi Arabia", NameAr = "المملكة العربية السعودية", Code = "SAU", Code2 = "SA", IsGccCountry = true, DisplayOrder = 3 },
            new() { Name = "Qatar", NameAr = "قطر", Code = "QAT", Code2 = "QA", IsGccCountry = true, DisplayOrder = 4 },
            new() { Name = "Kuwait", NameAr = "الكويت", Code = "KWT", Code2 = "KW", IsGccCountry = true, DisplayOrder = 5 },
            new() { Name = "Bahrain", NameAr = "البحرين", Code = "BHR", Code2 = "BH", IsGccCountry = true, DisplayOrder = 6 }
        };

        _context.Countries.AddRange(gccCountries);
        await _context.SaveChangesAsync();

        // Add major cities for each country
        var cities = new List<City>();

        // Oman cities (comprehensive list)
        var oman = gccCountries.First(c => c.Code2 == "OM");
        cities.AddRange([
            // Muscat Governorate
            new() { Name = "Muscat", NameAr = "مسقط", CountryId = oman.Id, DisplayOrder = 1, NoolCode = "MST", AramexCode = "MST" },
            new() { Name = "Mutrah", NameAr = "مطرح", CountryId = oman.Id, DisplayOrder = 2, NoolCode = "MTR", AramexCode = "MTR" },
            new() { Name = "Bausher", NameAr = "بوشر", CountryId = oman.Id, DisplayOrder = 3, NoolCode = "BSH", AramexCode = "BSH" },
            new() { Name = "Seeb", NameAr = "السيب", CountryId = oman.Id, DisplayOrder = 4, NoolCode = "SEB", AramexCode = "SEB" },
            new() { Name = "Quriyat", NameAr = "قريات", CountryId = oman.Id, DisplayOrder = 5, NoolCode = "QUR", AramexCode = "QUR" },
            new() { Name = "Amerat", NameAr = "العامرات", CountryId = oman.Id, DisplayOrder = 6, NoolCode = "AMR", AramexCode = "AMR" },
            
            // North Al Batinah Governorate
            new() { Name = "Sohar", NameAr = "صحار", CountryId = oman.Id, DisplayOrder = 10, NoolCode = "SOH", AramexCode = "SOH" },
            new() { Name = "Shinas", NameAr = "شناص", CountryId = oman.Id, DisplayOrder = 11, NoolCode = "SHN", AramexCode = "SHN" },
            new() { Name = "Liwa", NameAr = "لوى", CountryId = oman.Id, DisplayOrder = 12, NoolCode = "LIW", AramexCode = "LIW" },
            new() { Name = "Saham", NameAr = "صحم", CountryId = oman.Id, DisplayOrder = 13, NoolCode = "SAH", AramexCode = "SAH" },
            new() { Name = "Al Khaburah", NameAr = "الخابورة", CountryId = oman.Id, DisplayOrder = 14, NoolCode = "KHB", AramexCode = "KHB" },
            new() { Name = "As Suwaiq", NameAr = "السويق", CountryId = oman.Id, DisplayOrder = 15, NoolCode = "SWQ", AramexCode = "SWQ" },
            
            // South Al Batinah Governorate
            new() { Name = "Rustaq", NameAr = "الرستاق", CountryId = oman.Id, DisplayOrder = 20, NoolCode = "RST", AramexCode = "RST" },
            new() { Name = "Nakhal", NameAr = "نخل", CountryId = oman.Id, DisplayOrder = 21, NoolCode = "NKH", AramexCode = "NKH" },
            new() { Name = "Wadi Al Maawil", NameAr = "وادي المعاول", CountryId = oman.Id, DisplayOrder = 22, NoolCode = "WAM", AramexCode = "WAM" },
            new() { Name = "Barka", NameAr = "بركاء", CountryId = oman.Id, DisplayOrder = 23, NoolCode = "BRK", AramexCode = "BRK" },
            new() { Name = "Al Musanaah", NameAr = "المصنعة", CountryId = oman.Id, DisplayOrder = 24, NoolCode = "MSN", AramexCode = "MSN" },
            new() { Name = "Al Awabi", NameAr = "العوابي", CountryId = oman.Id, DisplayOrder = 25, NoolCode = "AWB", AramexCode = "AWB" },
            
            // Ad Dakhiliyah Governorate
            new() { Name = "Nizwa", NameAr = "نزوى", CountryId = oman.Id, DisplayOrder = 30, NoolCode = "NZW", AramexCode = "NZW" },
            new() { Name = "Bahla", NameAr = "بهلاء", CountryId = oman.Id, DisplayOrder = 31, NoolCode = "BHL", AramexCode = "BHL" },
            new() { Name = "Manah", NameAr = "منح", CountryId = oman.Id, DisplayOrder = 32, NoolCode = "MNH", AramexCode = "MNH" },
            new() { Name = "Adam", NameAr = "آدم", CountryId = oman.Id, DisplayOrder = 33, NoolCode = "ADM", AramexCode = "ADM" },
            new() { Name = "Al Hamra", NameAr = "الحمراء", CountryId = oman.Id, DisplayOrder = 34, NoolCode = "HMR", AramexCode = "HMR" },
            new() { Name = "Izki", NameAr = "إزكي", CountryId = oman.Id, DisplayOrder = 35, NoolCode = "IZK", AramexCode = "IZK" },
            new() { Name = "Samayil", NameAr = "سمائل", CountryId = oman.Id, DisplayOrder = 36, NoolCode = "SMY", AramexCode = "SMY" },
            new() { Name = "Bid Bid", NameAr = "بدبد", CountryId = oman.Id, DisplayOrder = 37, NoolCode = "BDB", AramexCode = "BDB" },
            
            // North Al Sharqiyah Governorate
            new() { Name = "Ibra", NameAr = "إبراء", CountryId = oman.Id, DisplayOrder = 40, NoolCode = "IBR", AramexCode = "IBR" },
            new() { Name = "Al Mudhaybi", NameAr = "المضيبي", CountryId = oman.Id, DisplayOrder = 41, NoolCode = "MDH", AramexCode = "MDH" },
            new() { Name = "Bidiyah", NameAr = "بدية", CountryId = oman.Id, DisplayOrder = 42, NoolCode = "BDY", AramexCode = "BDY" },
            new() { Name = "Dama Wa At Tayin", NameAr = "دماء والطائيين", CountryId = oman.Id, DisplayOrder = 43, NoolCode = "DWT", AramexCode = "DWT" },
            new() { Name = "Al Qabil", NameAr = "القابل", CountryId = oman.Id, DisplayOrder = 44, NoolCode = "QBL", AramexCode = "QBL" },
            new() { Name = "Wadi Bani Khalid", NameAr = "وادي بني خالد", CountryId = oman.Id, DisplayOrder = 45, NoolCode = "WBK", AramexCode = "WBK" },
            
            // South Al Sharqiyah Governorate
            new() { Name = "Sur", NameAr = "صور", CountryId = oman.Id, DisplayOrder = 50, NoolCode = "SUR", AramexCode = "SUR" },
            new() { Name = "Al Kamil Wa Al Wafi", NameAr = "الكامل والوافي", CountryId = oman.Id, DisplayOrder = 51, NoolCode = "KWW", AramexCode = "KWW" },
            new() { Name = "Jaalan Bani Bu Hassan", NameAr = "جعلان بني بو حسن", CountryId = oman.Id, DisplayOrder = 52, NoolCode = "JBH", AramexCode = "JBH" },
            new() { Name = "Jaalan Bani Bu Ali", NameAr = "جعلان بني بو علي", CountryId = oman.Id, DisplayOrder = 53, NoolCode = "JBA", AramexCode = "JBA" },
            new() { Name = "Masirah", NameAr = "مصيرة", CountryId = oman.Id, DisplayOrder = 54, NoolCode = "MSR", AramexCode = "MSR" },
            
            // Ad Dhahirah Governorate
            new() { Name = "Ibri", NameAr = "عبري", CountryId = oman.Id, DisplayOrder = 60, NoolCode = "IBR", AramexCode = "IBR" },
            new() { Name = "Yanqul", NameAr = "ينقل", CountryId = oman.Id, DisplayOrder = 61, NoolCode = "YNQ", AramexCode = "YNQ" },
            new() { Name = "Dhank", NameAr = "ضنك", CountryId = oman.Id, DisplayOrder = 62, NoolCode = "DHN", AramexCode = "DHN" },
            
            // Al Buraimi Governorate
            new() { Name = "Al Buraimi", NameAr = "البريمي", CountryId = oman.Id, DisplayOrder = 70, NoolCode = "BUR", AramexCode = "BUR" },
            new() { Name = "Mahadah", NameAr = "محضة", CountryId = oman.Id, DisplayOrder = 71, NoolCode = "MHD", AramexCode = "MHD" },
            new() { Name = "As Sunaynah", NameAr = "السنينة", CountryId = oman.Id, DisplayOrder = 72, NoolCode = "SNN", AramexCode = "SNN" },
            
            // Al Wusta Governorate
            new() { Name = "Haima", NameAr = "هيماء", CountryId = oman.Id, DisplayOrder = 80, NoolCode = "HIM", AramexCode = "HIM" },
            new() { Name = "Mahawt", NameAr = "محوت", CountryId = oman.Id, DisplayOrder = 81, NoolCode = "MHW", AramexCode = "MHW" },
            new() { Name = "Ad Duqm", NameAr = "الدقم", CountryId = oman.Id, DisplayOrder = 82, NoolCode = "DQM", AramexCode = "DQM" },
            new() { Name = "Al Jazer", NameAr = "الجازر", CountryId = oman.Id, DisplayOrder = 83, NoolCode = "JZR", AramexCode = "JZR" },
            
            // Dhofar Governorate
            new() { Name = "Salalah", NameAr = "صلالة", CountryId = oman.Id, DisplayOrder = 90, NoolCode = "SLL", AramexCode = "SLL" },
            new() { Name = "Taqah", NameAr = "طاقة", CountryId = oman.Id, DisplayOrder = 91, NoolCode = "TQH", AramexCode = "TQH" },
            new() { Name = "Mirbat", NameAr = "مرباط", CountryId = oman.Id, DisplayOrder = 92, NoolCode = "MRB", AramexCode = "MRB" },
            new() { Name = "Sadh", NameAr = "سدح", CountryId = oman.Id, DisplayOrder = 93, NoolCode = "SDH", AramexCode = "SDH" },
            new() { Name = "Rakhyut", NameAr = "رخيوت", CountryId = oman.Id, DisplayOrder = 94, NoolCode = "RKH", AramexCode = "RKH" },
            new() { Name = "Thumrait", NameAr = "ثمريت", CountryId = oman.Id, DisplayOrder = 95, NoolCode = "THM", AramexCode = "THM" },
            new() { Name = "Shalim Wa Juzor Al Hallaniyyat", NameAr = "شليم وجزر الحلانيات", CountryId = oman.Id, DisplayOrder = 96, NoolCode = "SHL", AramexCode = "SHL" },
            new() { Name = "Al Mazyunah", NameAr = "المزيونة", CountryId = oman.Id, DisplayOrder = 97, NoolCode = "MZY", AramexCode = "MZY" },
            new() { Name = "Dhalkut", NameAr = "ضلكوت", CountryId = oman.Id, DisplayOrder = 98, NoolCode = "DLK", AramexCode = "DLK" },
            new() { Name = "Muqshin", NameAr = "مقشن", CountryId = oman.Id, DisplayOrder = 99, NoolCode = "MQS", AramexCode = "MQS" },
            
            // Musandam Governorate
            new() { Name = "Khasab", NameAr = "خصب", CountryId = oman.Id, DisplayOrder = 100, NoolCode = "KHS", AramexCode = "KHS" },
            new() { Name = "Bukha", NameAr = "بخاء", CountryId = oman.Id, DisplayOrder = 101, NoolCode = "BKH", AramexCode = "BKH" },
            new() { Name = "Daba", NameAr = "دبا", CountryId = oman.Id, DisplayOrder = 102, NoolCode = "DBA", AramexCode = "DBA" },
            new() { Name = "Madha", NameAr = "مدحاء", CountryId = oman.Id, DisplayOrder = 103, NoolCode = "MDH", AramexCode = "MDH" }
        ]);

        // UAE cities (comprehensive list)
        var uae = gccCountries.First(c => c.Code2 == "AE");
        cities.AddRange([
            // Dubai Emirate
            new() { Name = "Dubai", NameAr = "دبي", CountryId = uae.Id, DisplayOrder = 1, AramexCode = "DXB" },
            new() { Name = "Deira", NameAr = "ديرة", CountryId = uae.Id, DisplayOrder = 2, AramexCode = "DEI" },
            new() { Name = "Bur Dubai", NameAr = "بر دبي", CountryId = uae.Id, DisplayOrder = 3, AramexCode = "BUR" },
            new() { Name = "Jumeirah", NameAr = "جميرا", CountryId = uae.Id, DisplayOrder = 4, AramexCode = "JUM" },
            new() { Name = "Al Barsha", NameAr = "البرشاء", CountryId = uae.Id, DisplayOrder = 5, AramexCode = "BAR" },
            new() { Name = "Dubai Marina", NameAr = "مرسى دبي", CountryId = uae.Id, DisplayOrder = 6, AramexCode = "MAR" },
            new() { Name = "Downtown Dubai", NameAr = "وسط مدينة دبي", CountryId = uae.Id, DisplayOrder = 7, AramexCode = "DOW" },
            new() { Name = "Business Bay", NameAr = "الخليج التجاري", CountryId = uae.Id, DisplayOrder = 8, AramexCode = "BB" },
            new() { Name = "Dubai Investment Park", NameAr = "مجمع دبي للاستثمار", CountryId = uae.Id, DisplayOrder = 9, AramexCode = "DIP" },
            new() { Name = "Jebel Ali", NameAr = "جبل علي", CountryId = uae.Id, DisplayOrder = 10, AramexCode = "JA" },
            new() { Name = "Al Qusais", NameAr = "القصيص", CountryId = uae.Id, DisplayOrder = 11, AramexCode = "QUS" },
            new() { Name = "Al Mizhar", NameAr = "المزهر", CountryId = uae.Id, DisplayOrder = 12, AramexCode = "MIZ" },
            new() { Name = "Al Khawaneej", NameAr = "الخوانيج", CountryId = uae.Id, DisplayOrder = 13, AramexCode = "KHW" },
            new() { Name = "Hatta", NameAr = "حتا", CountryId = uae.Id, DisplayOrder = 14, AramexCode = "HAT" },
            
            // Abu Dhabi Emirate
            new() { Name = "Abu Dhabi", NameAr = "أبو ظبي", CountryId = uae.Id, DisplayOrder = 20, AramexCode = "AUH" },
            new() { Name = "Al Ain", NameAr = "العين", CountryId = uae.Id, DisplayOrder = 21, AramexCode = "AAN" },
            new() { Name = "Al Dhafra", NameAr = "الظفرة", CountryId = uae.Id, DisplayOrder = 22, AramexCode = "DHF" },
            new() { Name = "Khalifa City", NameAr = "مدينة خليفة", CountryId = uae.Id, DisplayOrder = 23, AramexCode = "KHC" },
            new() { Name = "Al Ruwais", NameAr = "الرويس", CountryId = uae.Id, DisplayOrder = 24, AramexCode = "RUW" },
            new() { Name = "Masdar City", NameAr = "مدينة مصدر", CountryId = uae.Id, DisplayOrder = 25, AramexCode = "MAS" },
            new() { Name = "Yas Island", NameAr = "جزيرة ياس", CountryId = uae.Id, DisplayOrder = 26, AramexCode = "YAS" },
            new() { Name = "Saadiyat Island", NameAr = "جزيرة السعديات", CountryId = uae.Id, DisplayOrder = 27, AramexCode = "SAD" },
            new() { Name = "Al Bateen", NameAr = "الباطن", CountryId = uae.Id, DisplayOrder = 28, AramexCode = "BAT" },
            new() { Name = "Al Mushrif", NameAr = "المشرف", CountryId = uae.Id, DisplayOrder = 29, AramexCode = "MSH" },
            new() { Name = "Al Shamkha", NameAr = "الشمخة", CountryId = uae.Id, DisplayOrder = 30, AramexCode = "SHM" },
            new() { Name = "Baniyas", NameAr = "بني ياس", CountryId = uae.Id, DisplayOrder = 31, AramexCode = "BAN" },
            new() { Name = "Liwa", NameAr = "ليوا", CountryId = uae.Id, DisplayOrder = 32, AramexCode = "LIW" },
            new() { Name = "Mirfa", NameAr = "مرفأ", CountryId = uae.Id, DisplayOrder = 33, AramexCode = "MIR" },
            new() { Name = "Ghayathi", NameAr = "غياثي", CountryId = uae.Id, DisplayOrder = 34, AramexCode = "GHY" },
            
            // Sharjah Emirate
            new() { Name = "Sharjah", NameAr = "الشارقة", CountryId = uae.Id, DisplayOrder = 40, AramexCode = "SHJ" },
            new() { Name = "Kalba", NameAr = "كلباء", CountryId = uae.Id, DisplayOrder = 41, AramexCode = "KLB" },
            new() { Name = "Khor Fakkan", NameAr = "خور فكان", CountryId = uae.Id, DisplayOrder = 42, AramexCode = "KFK" },
            new() { Name = "Dibba Al Hisn", NameAr = "دبا الحصن", CountryId = uae.Id, DisplayOrder = 43, AramexCode = "DBH" },
            new() { Name = "Al Dhaid", NameAr = "الذيد", CountryId = uae.Id, DisplayOrder = 44, AramexCode = "DHD" },
            new() { Name = "Mleiha", NameAr = "مليحة", CountryId = uae.Id, DisplayOrder = 45, AramexCode = "MLH" },
            new() { Name = "Al Madam", NameAr = "المدام", CountryId = uae.Id, DisplayOrder = 46, AramexCode = "MDM" },
            new() { Name = "Al Hamriyah", NameAr = "الحمرية", CountryId = uae.Id, DisplayOrder = 47, AramexCode = "HAM" },
            
            // Ajman Emirate
            new() { Name = "Ajman", NameAr = "عجمان", CountryId = uae.Id, DisplayOrder = 50, AramexCode = "AJM" },
            new() { Name = "Al Manama", NameAr = "المنامة", CountryId = uae.Id, DisplayOrder = 51, AramexCode = "MAN" },
            new() { Name = "Masfout", NameAr = "مسفوت", CountryId = uae.Id, DisplayOrder = 52, AramexCode = "MSF" },
            
            // Ras Al Khaimah Emirate
            new() { Name = "Ras Al Khaimah", NameAr = "رأس الخيمة", CountryId = uae.Id, DisplayOrder = 60, AramexCode = "RAK" },
            new() { Name = "Al Jazirah Al Hamra", NameAr = "الجزيرة الحمراء", CountryId = uae.Id, DisplayOrder = 61, AramexCode = "JZH" },
            new() { Name = "Digdaga", NameAr = "دقداقة", CountryId = uae.Id, DisplayOrder = 62, AramexCode = "DIG" },
            new() { Name = "Khatt", NameAr = "خت", CountryId = uae.Id, DisplayOrder = 63, AramexCode = "KHT" },
            new() { Name = "Rams", NameAr = "رمس", CountryId = uae.Id, DisplayOrder = 64, AramexCode = "RAM" },
            
            // Fujairah Emirate
            new() { Name = "Fujairah", NameAr = "الفجيرة", CountryId = uae.Id, DisplayOrder = 70, AramexCode = "FUJ" },
            new() { Name = "Dibba Al Fujairah", NameAr = "دبا الفجيرة", CountryId = uae.Id, DisplayOrder = 71, AramexCode = "DBF" },
            new() { Name = "Al Bidyah", NameAr = "البدية", CountryId = uae.Id, DisplayOrder = 72, AramexCode = "BDY" },
            new() { Name = "Masafi", NameAr = "مسافي", CountryId = uae.Id, DisplayOrder = 73, AramexCode = "MSA" },
            new() { Name = "Al Hayl", NameAr = "الحيل", CountryId = uae.Id, DisplayOrder = 74, AramexCode = "HYL" },
            
            // Umm Al Quwain Emirate
            new() { Name = "Umm Al Quwain", NameAr = "أم القيوين", CountryId = uae.Id, DisplayOrder = 80, AramexCode = "UAQ" },
            new() { Name = "Falaj Al Mualla", NameAr = "فلج المعلا", CountryId = uae.Id, DisplayOrder = 81, AramexCode = "FAL" },
            new() { Name = "Al Dur", NameAr = "الدور", CountryId = uae.Id, DisplayOrder = 82, AramexCode = "DUR" }
        ]);

        // Saudi Arabia cities (comprehensive list)
        var saudi = gccCountries.First(c => c.Code2 == "SA");
        cities.AddRange([
            // Riyadh Province
            new() { Name = "Riyadh", NameAr = "الرياض", CountryId = saudi.Id, DisplayOrder = 1, AramexCode = "RUH" },
            new() { Name = "Al Kharj", NameAr = "الخرج", CountryId = saudi.Id, DisplayOrder = 2, AramexCode = "KHJ" },
            new() { Name = "Al Dawadmi", NameAr = "الدوادمي", CountryId = saudi.Id, DisplayOrder = 3, AramexCode = "DWD" },
            new() { Name = "Al Majmaah", NameAr = "المجمعة", CountryId = saudi.Id, DisplayOrder = 4, AramexCode = "MJM" },
            new() { Name = "Al Quwayiyah", NameAr = "القويعية", CountryId = saudi.Id, DisplayOrder = 5, AramexCode = "QWY" },
            new() { Name = "Wadi Al Dawasir", NameAr = "وادي الدواسر", CountryId = saudi.Id, DisplayOrder = 6, AramexCode = "WDD" },
            new() { Name = "Al Zulfi", NameAr = "الزلفي", CountryId = saudi.Id, DisplayOrder = 7, AramexCode = "ZLF" },
            new() { Name = "Shaqra", NameAr = "شقراء", CountryId = saudi.Id, DisplayOrder = 8, AramexCode = "SHQ" },
            new() { Name = "Howtat Bani Tamim", NameAr = "حوطة بني تميم", CountryId = saudi.Id, DisplayOrder = 9, AramexCode = "HBT" },
            new() { Name = "Afif", NameAr = "عفيف", CountryId = saudi.Id, DisplayOrder = 10, AramexCode = "AFF" },
            new() { Name = "As Sulayyil", NameAr = "السليل", CountryId = saudi.Id, DisplayOrder = 11, AramexCode = "SLY" },
            
            // Makkah Province
            new() { Name = "Mecca", NameAr = "مكة المكرمة", CountryId = saudi.Id, DisplayOrder = 20, AramexCode = "MKK" },
            new() { Name = "Jeddah", NameAr = "جدة", CountryId = saudi.Id, DisplayOrder = 21, AramexCode = "JED" },
            new() { Name = "Taif", NameAr = "الطائف", CountryId = saudi.Id, DisplayOrder = 22, AramexCode = "TIF" },
            new() { Name = "Al Qunfudhah", NameAr = "القنفذة", CountryId = saudi.Id, DisplayOrder = 23, AramexCode = "QNF" },
            new() { Name = "Rabigh", NameAr = "رابغ", CountryId = saudi.Id, DisplayOrder = 24, AramexCode = "RBG" },
            new() { Name = "Al Lith", NameAr = "الليث", CountryId = saudi.Id, DisplayOrder = 25, AramexCode = "LTH" },
            new() { Name = "Khulais", NameAr = "خليص", CountryId = saudi.Id, DisplayOrder = 26, AramexCode = "KHL" },
            new() { Name = "Al Jumum", NameAr = "الجموم", CountryId = saudi.Id, DisplayOrder = 27, AramexCode = "JMM" },
            new() { Name = "Bahra", NameAr = "بحرة", CountryId = saudi.Id, DisplayOrder = 28, AramexCode = "BHR" },
            new() { Name = "Adham", NameAr = "أضم", CountryId = saudi.Id, DisplayOrder = 29, AramexCode = "ADH" },
            new() { Name = "Kamil", NameAr = "الكامل", CountryId = saudi.Id, DisplayOrder = 30, AramexCode = "KML" },
            
            // Medina Province
            new() { Name = "Medina", NameAr = "المدينة المنورة", CountryId = saudi.Id, DisplayOrder = 40, AramexCode = "MED" },
            new() { Name = "Yanbu", NameAr = "ينبع", CountryId = saudi.Id, DisplayOrder = 41, AramexCode = "YNB" },
            new() { Name = "Al Ula", NameAr = "العلا", CountryId = saudi.Id, DisplayOrder = 42, AramexCode = "ULA" },
            new() { Name = "Badr", NameAr = "بدر", CountryId = saudi.Id, DisplayOrder = 43, AramexCode = "BDR" },
            new() { Name = "Khaybar", NameAr = "خيبر", CountryId = saudi.Id, DisplayOrder = 44, AramexCode = "KHY" },
            new() { Name = "Al Mahd", NameAr = "المهد", CountryId = saudi.Id, DisplayOrder = 45, AramexCode = "MHD" },
            new() { Name = "Wadi Al Fara", NameAr = "وادي الفرع", CountryId = saudi.Id, DisplayOrder = 46, AramexCode = "WDF" },
            new() { Name = "Al Henakiyah", NameAr = "الحناكية", CountryId = saudi.Id, DisplayOrder = 47, AramexCode = "HNK" },
            
            // Eastern Province
            new() { Name = "Dammam", NameAr = "الدمام", CountryId = saudi.Id, DisplayOrder = 50, AramexCode = "DMM" },
            new() { Name = "Khobar", NameAr = "الخبر", CountryId = saudi.Id, DisplayOrder = 51, AramexCode = "KBR" },
            new() { Name = "Dhahran", NameAr = "الظهران", CountryId = saudi.Id, DisplayOrder = 52, AramexCode = "DHR" },
            new() { Name = "Al Jubail", NameAr = "الجبيل", CountryId = saudi.Id, DisplayOrder = 53, AramexCode = "JUB" },
            new() { Name = "Al Ahsa", NameAr = "الأحساء", CountryId = saudi.Id, DisplayOrder = 54, AramexCode = "AHS" },
            new() { Name = "Hafar Al Batin", NameAr = "حفر الباطن", CountryId = saudi.Id, DisplayOrder = 55, AramexCode = "HFB" },
            new() { Name = "Qatif", NameAr = "القطيف", CountryId = saudi.Id, DisplayOrder = 56, AramexCode = "QTF" },
            new() { Name = "Ras Tanura", NameAr = "رأس تنورة", CountryId = saudi.Id, DisplayOrder = 57, AramexCode = "RTN" },
            new() { Name = "Khafji", NameAr = "الخفجي", CountryId = saudi.Id, DisplayOrder = 58, AramexCode = "KHF" },
            new() { Name = "Nairyah", NameAr = "النعيرية", CountryId = saudi.Id, DisplayOrder = 59, AramexCode = "NYR" },
            new() { Name = "Abqaiq", NameAr = "بقيق", CountryId = saudi.Id, DisplayOrder = 60, AramexCode = "ABQ" },
            
            // Asir Province
            new() { Name = "Abha", NameAr = "أبها", CountryId = saudi.Id, DisplayOrder = 70, AramexCode = "AHB" },
            new() { Name = "Khamis Mushait", NameAr = "خميس مشيط", CountryId = saudi.Id, DisplayOrder = 71, AramexCode = "KMS" },
            new() { Name = "Najran", NameAr = "نجران", CountryId = saudi.Id, DisplayOrder = 72, AramexCode = "EAM" },
            new() { Name = "Jazan", NameAr = "جازان", CountryId = saudi.Id, DisplayOrder = 73, AramexCode = "GIZ" },
            new() { Name = "Bisha", NameAr = "بيشة", CountryId = saudi.Id, DisplayOrder = 74, AramexCode = "BIS" },
            new() { Name = "Sabya", NameAr = "صبيا", CountryId = saudi.Id, DisplayOrder = 75, AramexCode = "SBY" },
            new() { Name = "Abu Arish", NameAr = "أبو عريش", CountryId = saudi.Id, DisplayOrder = 76, AramexCode = "ARS" },
            new() { Name = "Samtah", NameAr = "صامطة", CountryId = saudi.Id, DisplayOrder = 77, AramexCode = "SMT" },
            new() { Name = "Mahayil", NameAr = "محايل", CountryId = saudi.Id, DisplayOrder = 78, AramexCode = "MHY" },
            new() { Name = "Ahad Rafidah", NameAr = "أحد رفيدة", CountryId = saudi.Id, DisplayOrder = 79, AramexCode = "ARF" },
            
            // Tabuk Province
            new() { Name = "Tabuk", NameAr = "تبوك", CountryId = saudi.Id, DisplayOrder = 80, AramexCode = "TUU" },
            new() { Name = "Duba", NameAr = "ضباء", CountryId = saudi.Id, DisplayOrder = 81, AramexCode = "DUB" },
            new() { Name = "Timaa", NameAr = "تيماء", CountryId = saudi.Id, DisplayOrder = 82, AramexCode = "TIM" },
            new() { Name = "Umluj", NameAr = "أملج", CountryId = saudi.Id, DisplayOrder = 83, AramexCode = "UML" },
            new() { Name = "Al Wajh", NameAr = "الوجه", CountryId = saudi.Id, DisplayOrder = 84, AramexCode = "EJH" },
            new() { Name = "Haql", NameAr = "حقل", CountryId = saudi.Id, DisplayOrder = 85, AramexCode = "HQL" },
            new() { Name = "Al Bad", NameAr = "البدع", CountryId = saudi.Id, DisplayOrder = 86, AramexCode = "BAD" },
            
            // Hail Province
            new() { Name = "Hail", NameAr = "حائل", CountryId = saudi.Id, DisplayOrder = 90, AramexCode = "HAS" },
            new() { Name = "Baqaa", NameAr = "بقعاء", CountryId = saudi.Id, DisplayOrder = 91, AramexCode = "BQA" },
            new() { Name = "Al Ghazalah", NameAr = "الغزالة", CountryId = saudi.Id, DisplayOrder = 92, AramexCode = "GHZ" },
            new() { Name = "Ash Shinan", NameAr = "الشنان", CountryId = saudi.Id, DisplayOrder = 93, AramexCode = "SHN" },
            new() { Name = "As Sulaymi", NameAr = "السليمي", CountryId = saudi.Id, DisplayOrder = 94, AramexCode = "SLM" },
            
            // Northern Borders Province
            new() { Name = "Arar", NameAr = "عرعر", CountryId = saudi.Id, DisplayOrder = 100, AramexCode = "RAE" },
            new() { Name = "Rafha", NameAr = "رفحاء", CountryId = saudi.Id, DisplayOrder = 101, AramexCode = "RFH" },
            new() { Name = "Turaif", NameAr = "طريف", CountryId = saudi.Id, DisplayOrder = 102, AramexCode = "TUI" },
            
            // Al Jouf Province
            new() { Name = "Sakaka", NameAr = "سكاكا", CountryId = saudi.Id, DisplayOrder = 110, AramexCode = "AJF" },
            new() { Name = "Qurayyat", NameAr = "القريات", CountryId = saudi.Id, DisplayOrder = 111, AramexCode = "URY" },
            new() { Name = "Tabarjal", NameAr = "طبرجل", CountryId = saudi.Id, DisplayOrder = 112, AramexCode = "TBJ" },
            new() { Name = "Dumat Al Jandal", NameAr = "دومة الجندل", CountryId = saudi.Id, DisplayOrder = 113, AramexCode = "DMJ" },
            
            // Al Bahah Province
            new() { Name = "Al Bahah", NameAr = "الباحة", CountryId = saudi.Id, DisplayOrder = 120, AramexCode = "ABT" },
            new() { Name = "Baljurashi", NameAr = "بلجرشي", CountryId = saudi.Id, DisplayOrder = 121, AramexCode = "BLJ" },
            new() { Name = "Al Mandaq", NameAr = "المندق", CountryId = saudi.Id, DisplayOrder = 122, AramexCode = "MND" },
            new() { Name = "Al Mikhwah", NameAr = "المخواة", CountryId = saudi.Id, DisplayOrder = 123, AramexCode = "MKH" },
            new() { Name = "Qilwah", NameAr = "قلوة", CountryId = saudi.Id, DisplayOrder = 124, AramexCode = "QLW" },
            
            // Qassim Province  
            new() { Name = "Buraydah", NameAr = "بريدة", CountryId = saudi.Id, DisplayOrder = 130, AramexCode = "ELQ" },
            new() { Name = "Unaizah", NameAr = "عنيزة", CountryId = saudi.Id, DisplayOrder = 131, AramexCode = "UNZ" },
            new() { Name = "Ar Rass", NameAr = "الرس", CountryId = saudi.Id, DisplayOrder = 132, AramexCode = "RSS" },
            new() { Name = "Al Mithnab", NameAr = "المذنب", CountryId = saudi.Id, DisplayOrder = 133, AramexCode = "MTH" },
            new() { Name = "Al Bukayriyah", NameAr = "البكيرية", CountryId = saudi.Id, DisplayOrder = 134, AramexCode = "BKR" },
            new() { Name = "Riyadh Al Khabra", NameAr = "رياض الخبراء", CountryId = saudi.Id, DisplayOrder = 135, AramexCode = "RKH" },
            new() { Name = "Al Badayie", NameAr = "البدائع", CountryId = saudi.Id, DisplayOrder = 136, AramexCode = "BDY" }
        ]);

        // Qatar cities (comprehensive list)
        var qatar = gccCountries.First(c => c.Code2 == "QA");
        cities.AddRange([
            // Doha Municipality
            new() { Name = "Doha", NameAr = "الدوحة", CountryId = qatar.Id, DisplayOrder = 1, AramexCode = "DOH" },
            new() { Name = "West Bay", NameAr = "الخليج الغربي", CountryId = qatar.Id, DisplayOrder = 2, AramexCode = "WB" },
            new() { Name = "Al Sadd", NameAr = "السد", CountryId = qatar.Id, DisplayOrder = 3, AramexCode = "SAD" },
            new() { Name = "Al Nasr", NameAr = "النصر", CountryId = qatar.Id, DisplayOrder = 4, AramexCode = "NSR" },
            new() { Name = "Bin Mahmoud", NameAr = "بن محمود", CountryId = qatar.Id, DisplayOrder = 5, AramexCode = "BM" },
            new() { Name = "Al Mirqab", NameAr = "المرقاب", CountryId = qatar.Id, DisplayOrder = 6, AramexCode = "MRQ" },
            new() { Name = "Mushaireb", NameAr = "مشيرب", CountryId = qatar.Id, DisplayOrder = 7, AramexCode = "MSH" },
            new() { Name = "Al Jasra", NameAr = "الجسرة", CountryId = qatar.Id, DisplayOrder = 8, AramexCode = "JSR" },
            new() { Name = "Fereej Bin Mahmoud", NameAr = "فريج بن محمود", CountryId = qatar.Id, DisplayOrder = 9, AramexCode = "FBM" },
            new() { Name = "Al Bidda", NameAr = "البدع", CountryId = qatar.Id, DisplayOrder = 10, AramexCode = "BDD" },
            new() { Name = "Najma", NameAr = "نجمة", CountryId = qatar.Id, DisplayOrder = 11, AramexCode = "NJM" },
            new() { Name = "Mansoura", NameAr = "المنصورة", CountryId = qatar.Id, DisplayOrder = 12, AramexCode = "MNS" },
            new() { Name = "Fereej Al Nasr", NameAr = "فريج النصر", CountryId = qatar.Id, DisplayOrder = 13, AramexCode = "FNS" },
            new() { Name = "Al Hilal", NameAr = "الهلال", CountryId = qatar.Id, DisplayOrder = 14, AramexCode = "HLL" },
            new() { Name = "Old Airport", NameAr = "المطار القديم", CountryId = qatar.Id, DisplayOrder = 15, AramexCode = "OA" },
            
            // Al Rayyan Municipality
            new() { Name = "Al Rayyan", NameAr = "الريان", CountryId = qatar.Id, DisplayOrder = 20, AramexCode = "RAY" },
            new() { Name = "Education City", NameAr = "المدينة التعليمية", CountryId = qatar.Id, DisplayOrder = 21, AramexCode = "EC" },
            new() { Name = "Al Aziziyah", NameAr = "العزيزية", CountryId = qatar.Id, DisplayOrder = 22, AramexCode = "AZZ" },
            new() { Name = "Abu Hamour", NameAr = "أبو هامور", CountryId = qatar.Id, DisplayOrder = 23, AramexCode = "AH" },
            new() { Name = "Al Gharrafa", NameAr = "الغرافة", CountryId = qatar.Id, DisplayOrder = 24, AramexCode = "GHR" },
            new() { Name = "Al Waab", NameAr = "الوعب", CountryId = qatar.Id, DisplayOrder = 25, AramexCode = "WAB" },
            new() { Name = "Ain Khaled", NameAr = "عين خالد", CountryId = qatar.Id, DisplayOrder = 26, AramexCode = "AK" },
            new() { Name = "Al Sailiya", NameAr = "السيلية", CountryId = qatar.Id, DisplayOrder = 27, AramexCode = "SLY" },
            new() { Name = "Umm Salal Mohammed", NameAr = "أم صلال محمد", CountryId = qatar.Id, DisplayOrder = 28, AramexCode = "USM" },
            new() { Name = "Al Shahaniya", NameAr = "الشحانية", CountryId = qatar.Id, DisplayOrder = 29, AramexCode = "SHH" },
            new() { Name = "Dukhan", NameAr = "دخان", CountryId = qatar.Id, DisplayOrder = 30, AramexCode = "DKH" },
            
            // Al Wakrah Municipality
            new() { Name = "Al Wakrah", NameAr = "الوكرة", CountryId = qatar.Id, DisplayOrder = 40, AramexCode = "WKR" },
            new() { Name = "Mesaieed", NameAr = "مسيعيد", CountryId = qatar.Id, DisplayOrder = 41, AramexCode = "MSA" },
            new() { Name = "Al Wukair", NameAr = "الوكير", CountryId = qatar.Id, DisplayOrder = 42, AramexCode = "WUK" },
            new() { Name = "Al Kheesa", NameAr = "الخيسة", CountryId = qatar.Id, DisplayOrder = 43, AramexCode = "KHS" },
            new() { Name = "Lusail", NameAr = "لوسيل", CountryId = qatar.Id, DisplayOrder = 44, AramexCode = "LSL" },
            new() { Name = "Al Egla", NameAr = "العقلة", CountryId = qatar.Id, DisplayOrder = 45, AramexCode = "EGL" },
            
            // Al Khor Municipality  
            new() { Name = "Al Khor", NameAr = "الخور", CountryId = qatar.Id, DisplayOrder = 50, AramexCode = "KHR" },
            new() { Name = "Al Thakhira", NameAr = "الذخيرة", CountryId = qatar.Id, DisplayOrder = 51, AramexCode = "THK" },
            new() { Name = "Ras Laffan", NameAr = "رأس لفان", CountryId = qatar.Id, DisplayOrder = 52, AramexCode = "RLF" },
            new() { Name = "Simaisma", NameAr = "سميسمة", CountryId = qatar.Id, DisplayOrder = 53, AramexCode = "SMS" },
            
            // Umm Salal Municipality
            new() { Name = "Umm Salal Ali", NameAr = "أم صلال علي", CountryId = qatar.Id, DisplayOrder = 60, AramexCode = "USA" },
            new() { Name = "Umm Salal", NameAr = "أم صلال", CountryId = qatar.Id, DisplayOrder = 61, AramexCode = "UMS" },
            
            // Al Daayen Municipality
            new() { Name = "Al Daayen", NameAr = "الضعاين", CountryId = qatar.Id, DisplayOrder = 70, AramexCode = "DDY" },
            new() { Name = "Al Kharrara", NameAr = "الخرارة", CountryId = qatar.Id, DisplayOrder = 71, AramexCode = "KHR" },
            
            // Al Shamal Municipality
            new() { Name = "Madinat Ash Shamal", NameAr = "مدينة الشمال", CountryId = qatar.Id, DisplayOrder = 80, AramexCode = "MSM" },
            new() { Name = "Ar Ruwais", NameAr = "الرويس", CountryId = qatar.Id, DisplayOrder = 81, AramexCode = "RWS" },
            new() { Name = "Fuwairit", NameAr = "فويرط", CountryId = qatar.Id, DisplayOrder = 82, AramexCode = "FWR" }
        ]);

        // Kuwait cities (comprehensive list)
        var kuwait = gccCountries.First(c => c.Code2 == "KW");
        cities.AddRange([
            // Capital Governorate
            new() { Name = "Kuwait City", NameAr = "مدينة الكويت", CountryId = kuwait.Id, DisplayOrder = 1, AramexCode = "KWI" },
            new() { Name = "Dasman", NameAr = "دسمان", CountryId = kuwait.Id, DisplayOrder = 2, AramexCode = "DSM" },
            new() { Name = "Sharq", NameAr = "شرق", CountryId = kuwait.Id, DisplayOrder = 3, AramexCode = "SHR" },
            new() { Name = "Mirqab", NameAr = "مرقاب", CountryId = kuwait.Id, DisplayOrder = 4, AramexCode = "MRQ" },
            new() { Name = "Jibla", NameAr = "جبلة", CountryId = kuwait.Id, DisplayOrder = 5, AramexCode = "JBL" },
            new() { Name = "Bneid Al Qar", NameAr = "بنيد القار", CountryId = kuwait.Id, DisplayOrder = 6, AramexCode = "BQ" },
            new() { Name = "Khaldiya", NameAr = "خالدية", CountryId = kuwait.Id, DisplayOrder = 7, AramexCode = "KHL" },
            new() { Name = "Qadsia", NameAr = "قادسية", CountryId = kuwait.Id, DisplayOrder = 8, AramexCode = "QDS" },
            new() { Name = "Faiha", NameAr = "الفيحاء", CountryId = kuwait.Id, DisplayOrder = 9, AramexCode = "FIH" },
            new() { Name = "Shamiya", NameAr = "الشامية", CountryId = kuwait.Id, DisplayOrder = 10, AramexCode = "SHM" },
            new() { Name = "Shuwaikh", NameAr = "الشويخ", CountryId = kuwait.Id, DisplayOrder = 11, AramexCode = "SHW" },
            new() { Name = "Doha", NameAr = "الدوحة", CountryId = kuwait.Id, DisplayOrder = 12, AramexCode = "DOH" },
            new() { Name = "Nahda", NameAr = "النهضة", CountryId = kuwait.Id, DisplayOrder = 13, AramexCode = "NHD" },
            new() { Name = "Ferdous", NameAr = "الفردوس", CountryId = kuwait.Id, DisplayOrder = 14, AramexCode = "FRD" },
            new() { Name = "Dasma", NameAr = "الدسمة", CountryId = kuwait.Id, DisplayOrder = 15, AramexCode = "DSM" },
            new() { Name = "Yarmouk", NameAr = "اليرموك", CountryId = kuwait.Id, DisplayOrder = 16, AramexCode = "YRM" },
            new() { Name = "Surra", NameAr = "الصرة", CountryId = kuwait.Id, DisplayOrder = 17, AramexCode = "SRR" },
            new() { Name = "Sulaibikhat", NameAr = "الصليبيخات", CountryId = kuwait.Id, DisplayOrder = 18, AramexCode = "SLB" },
            new() { Name = "Qortuba", NameAr = "قرطبة", CountryId = kuwait.Id, DisplayOrder = 19, AramexCode = "QRT" },
            new() { Name = "Adailiya", NameAr = "العديلية", CountryId = kuwait.Id, DisplayOrder = 20, AramexCode = "ADL" },
            new() { Name = "Kaifan", NameAr = "كيفان", CountryId = kuwait.Id, DisplayOrder = 21, AramexCode = "KIF" },
            new() { Name = "Rawda", NameAr = "الروضة", CountryId = kuwait.Id, DisplayOrder = 22, AramexCode = "RWD" },
            new() { Name = "Salam", NameAr = "السلام", CountryId = kuwait.Id, DisplayOrder = 23, AramexCode = "SLM" },
            
            // Hawalli Governorate
            new() { Name = "Hawalli", NameAr = "حولي", CountryId = kuwait.Id, DisplayOrder = 30, AramexCode = "HWL" },
            new() { Name = "Salmiya", NameAr = "السالمية", CountryId = kuwait.Id, DisplayOrder = 31, AramexCode = "SLM" },
            new() { Name = "Jabriya", NameAr = "الجابرية", CountryId = kuwait.Id, DisplayOrder = 32, AramexCode = "JBR" },
            new() { Name = "Maidan Hawalli", NameAr = "ميدان حولي", CountryId = kuwait.Id, DisplayOrder = 33, AramexCode = "MH" },
            new() { Name = "Bayan", NameAr = "بيان", CountryId = kuwait.Id, DisplayOrder = 34, AramexCode = "BYN" },
            new() { Name = "Mishref", NameAr = "مشرف", CountryId = kuwait.Id, DisplayOrder = 35, AramexCode = "MSH" },
            new() { Name = "Salwa", NameAr = "سلوى", CountryId = kuwait.Id, DisplayOrder = 36, AramexCode = "SLW" },
            new() { Name = "Rumaithiya", NameAr = "الرميثية", CountryId = kuwait.Id, DisplayOrder = 37, AramexCode = "RMT" },
            new() { Name = "Shaab", NameAr = "الشعب", CountryId = kuwait.Id, DisplayOrder = 38, AramexCode = "SHB" },
            new() { Name = "Shuhada", NameAr = "الشهداء", CountryId = kuwait.Id, DisplayOrder = 39, AramexCode = "SHD" },
            
            // Farwaniya Governorate
            new() { Name = "Farwaniya", NameAr = "الفروانية", CountryId = kuwait.Id, DisplayOrder = 40, AramexCode = "FRW" },
            new() { Name = "Jeleeb Al Shuyoukh", NameAr = "جليب الشيوخ", CountryId = kuwait.Id, DisplayOrder = 41, AramexCode = "JLS" },
            new() { Name = "Khaitan", NameAr = "خيطان", CountryId = kuwait.Id, DisplayOrder = 42, AramexCode = "KHT" },
            new() { Name = "Ardiya", NameAr = "العارضية", CountryId = kuwait.Id, DisplayOrder = 43, AramexCode = "ARD" },
            new() { Name = "Firdous", NameAr = "الفردوس", CountryId = kuwait.Id, DisplayOrder = 44, AramexCode = "FRD" },
            new() { Name = "Andalous", NameAr = "الأندلس", CountryId = kuwait.Id, DisplayOrder = 45, AramexCode = "AND" },
            new() { Name = "Rehab", NameAr = "الرحاب", CountryId = kuwait.Id, DisplayOrder = 46, AramexCode = "RHB" },
            new() { Name = "Dhajeej", NameAr = "الضجيج", CountryId = kuwait.Id, DisplayOrder = 47, AramexCode = "DHJ" },
            new() { Name = "Omariya", NameAr = "العمرية", CountryId = kuwait.Id, DisplayOrder = 48, AramexCode = "OMR" },
            new() { Name = "Rai", NameAr = "الري", CountryId = kuwait.Id, DisplayOrder = 49, AramexCode = "RAI" },
            new() { Name = "Rabiya", NameAr = "الرابية", CountryId = kuwait.Id, DisplayOrder = 50, AramexCode = "RBY" },
            new() { Name = "Sabahiya", NameAr = "الصباحية", CountryId = kuwait.Id, DisplayOrder = 51, AramexCode = "SBH" },
            new() { Name = "Ishbiliya", NameAr = "إشبيلية", CountryId = kuwait.Id, DisplayOrder = 52, AramexCode = "ISH" },
            
            // Ahmadi Governorate
            new() { Name = "Ahmadi", NameAr = "الأحمدي", CountryId = kuwait.Id, DisplayOrder = 60, AramexCode = "AHM" },
            new() { Name = "Fahaheel", NameAr = "الفحيحيل", CountryId = kuwait.Id, DisplayOrder = 61, AramexCode = "FAH" },
            new() { Name = "Mangaf", NameAr = "المنقف", CountryId = kuwait.Id, DisplayOrder = 62, AramexCode = "MNG" },
            new() { Name = "Abu Halifa", NameAr = "أبو حليفة", CountryId = kuwait.Id, DisplayOrder = 63, AramexCode = "AH" },
            new() { Name = "Fintas", NameAr = "الفنطاس", CountryId = kuwait.Id, DisplayOrder = 64, AramexCode = "FNT" },
            new() { Name = "Mahboula", NameAr = "المهبولة", CountryId = kuwait.Id, DisplayOrder = 65, AramexCode = "MHB" },
            new() { Name = "Sabah Al Salem", NameAr = "صباح السالم", CountryId = kuwait.Id, DisplayOrder = 66, AramexCode = "SAS" },
            new() { Name = "Ali Sabah Al Salem", NameAr = "علي صباح السالم", CountryId = kuwait.Id, DisplayOrder = 67, AramexCode = "ASS" },
            new() { Name = "Riqqa", NameAr = "الرقة", CountryId = kuwait.Id, DisplayOrder = 68, AramexCode = "RQQ" },
            new() { Name = "Mina Abdullah", NameAr = "ميناء عبد الله", CountryId = kuwait.Id, DisplayOrder = 69, AramexCode = "MAB" },
            new() { Name = "Zour", NameAr = "الزور", CountryId = kuwait.Id, DisplayOrder = 70, AramexCode = "ZOR" },
            new() { Name = "Wafra", NameAr = "الوفرة", CountryId = kuwait.Id, DisplayOrder = 71, AramexCode = "WFR" },
            new() { Name = "Khiran", NameAr = "الخيران", CountryId = kuwait.Id, DisplayOrder = 72, AramexCode = "KHR" },
            
            // Jahra Governorate
            new() { Name = "Jahra", NameAr = "الجهراء", CountryId = kuwait.Id, DisplayOrder = 80, AramexCode = "JHR" },
            new() { Name = "Sulaibiya", NameAr = "الصليبية", CountryId = kuwait.Id, DisplayOrder = 81, AramexCode = "SLB" },
            new() { Name = "Naseem", NameAr = "النسيم", CountryId = kuwait.Id, DisplayOrder = 82, AramexCode = "NSM" },
            new() { Name = "Qasr", NameAr = "القصر", CountryId = kuwait.Id, DisplayOrder = 83, AramexCode = "QSR" },
            new() { Name = "Taima", NameAr = "تيماء", CountryId = kuwait.Id, DisplayOrder = 84, AramexCode = "TIM" },
            new() { Name = "Warah", NameAr = "الوارة", CountryId = kuwait.Id, DisplayOrder = 85, AramexCode = "WRH" },
            new() { Name = "Abdaly", NameAr = "العبدلي", CountryId = kuwait.Id, DisplayOrder = 86, AramexCode = "ABD" },
            new() { Name = "Kabd", NameAr = "كبد", CountryId = kuwait.Id, DisplayOrder = 87, AramexCode = "KBD" },
            new() { Name = "Saad Al Abdullah", NameAr = "سعد العبد الله", CountryId = kuwait.Id, DisplayOrder = 88, AramexCode = "SAA" },
            
            // Mubarak Al Kabeer Governorate
            new() { Name = "Mubarak Al Kabeer", NameAr = "مبارك الكبير", CountryId = kuwait.Id, DisplayOrder = 90, AramexCode = "MBK" },
            new() { Name = "Qurain", NameAr = "القرين", CountryId = kuwait.Id, DisplayOrder = 91, AramexCode = "QRN" },
            new() { Name = "Adan", NameAr = "العدان", CountryId = kuwait.Id, DisplayOrder = 92, AramexCode = "ADN" },
            new() { Name = "Qusour", NameAr = "القصور", CountryId = kuwait.Id, DisplayOrder = 93, AramexCode = "QSR" },
            new() { Name = "Sabhan", NameAr = "صبحان", CountryId = kuwait.Id, DisplayOrder = 94, AramexCode = "SBH" },
            new() { Name = "Fnaitees", NameAr = "الفنيطيس", CountryId = kuwait.Id, DisplayOrder = 95, AramexCode = "FNT" },
            new() { Name = "Messila", NameAr = "المسيلة", CountryId = kuwait.Id, DisplayOrder = 96, AramexCode = "MSL" },
            new() { Name = "Abu Ftaira", NameAr = "أبو فطيرة", CountryId = kuwait.Id, DisplayOrder = 97, AramexCode = "AF" }
        ]);

        // Bahrain cities (comprehensive list)
        var bahrain = gccCountries.First(c => c.Code2 == "BH");
        cities.AddRange([
            // Capital Governorate
            new() { Name = "Manama", NameAr = "المنامة", CountryId = bahrain.Id, DisplayOrder = 1, AramexCode = "BAH" },
            new() { Name = "Diplomatic Area", NameAr = "المنطقة الدبلوماسية", CountryId = bahrain.Id, DisplayOrder = 2, AramexCode = "DA" },
            new() { Name = "Juffair", NameAr = "الجفير", CountryId = bahrain.Id, DisplayOrder = 3, AramexCode = "JUF" },
            new() { Name = "Adliya", NameAr = "العدلية", CountryId = bahrain.Id, DisplayOrder = 4, AramexCode = "ADL" },
            new() { Name = "Hoora", NameAr = "الحورة", CountryId = bahrain.Id, DisplayOrder = 5, AramexCode = "HOR" },
            new() { Name = "Gudaibiya", NameAr = "القضيبية", CountryId = bahrain.Id, DisplayOrder = 6, AramexCode = "GUD" },
            new() { Name = "Mahooz", NameAr = "الماحوز", CountryId = bahrain.Id, DisplayOrder = 7, AramexCode = "MAH" },
            new() { Name = "Salmaniya", NameAr = "السلمانية", CountryId = bahrain.Id, DisplayOrder = 8, AramexCode = "SLM" },
            new() { Name = "Zinj", NameAr = "الزنج", CountryId = bahrain.Id, DisplayOrder = 9, AramexCode = "ZNJ" },
            new() { Name = "Sanabis", NameAr = "السنابس", CountryId = bahrain.Id, DisplayOrder = 10, AramexCode = "SNB" },
            new() { Name = "Tubli", NameAr = "توبلي", CountryId = bahrain.Id, DisplayOrder = 11, AramexCode = "TUB" },
            new() { Name = "Umm Al Hassam", NameAr = "أم الحصم", CountryId = bahrain.Id, DisplayOrder = 12, AramexCode = "UMH" },
            new() { Name = "Seef", NameAr = "السيف", CountryId = bahrain.Id, DisplayOrder = 13, AramexCode = "SEF" },
            new() { Name = "Janabiya", NameAr = "الجنبية", CountryId = bahrain.Id, DisplayOrder = 14, AramexCode = "JAN" },
            new() { Name = "Busaiteen", NameAr = "البسيتين", CountryId = bahrain.Id, DisplayOrder = 15, AramexCode = "BUS" },
            new() { Name = "Qudaibiya", NameAr = "القديبية", CountryId = bahrain.Id, DisplayOrder = 16, AramexCode = "QUD" },
            
            // Muharraq Governorate
            new() { Name = "Muharraq", NameAr = "المحرق", CountryId = bahrain.Id, DisplayOrder = 20, AramexCode = "MHQ" },
            new() { Name = "Hidd", NameAr = "الحد", CountryId = bahrain.Id, DisplayOrder = 21, AramexCode = "HID" },
            new() { Name = "Arad", NameAr = "عراد", CountryId = bahrain.Id, DisplayOrder = 22, AramexCode = "ARD" },
            new() { Name = "Dair", NameAr = "الدير", CountryId = bahrain.Id, DisplayOrder = 23, AramexCode = "DIR" },
            new() { Name = "Galali", NameAr = "القلالي", CountryId = bahrain.Id, DisplayOrder = 24, AramexCode = "GAL" },
            new() { Name = "Halat Bu Maher", NameAr = "حالة بو ماهر", CountryId = bahrain.Id, DisplayOrder = 25, AramexCode = "HBM" },
            new() { Name = "Samaheej", NameAr = "سماهيج", CountryId = bahrain.Id, DisplayOrder = 26, AramexCode = "SMH" },
            new() { Name = "Busaiteen", NameAr = "البسيتين", CountryId = bahrain.Id, DisplayOrder = 27, AramexCode = "BUS" },
            
            // Northern Governorate
            new() { Name = "Hamala", NameAr = "الحمالة", CountryId = bahrain.Id, DisplayOrder = 30, AramexCode = "HAM" },
            new() { Name = "Janussan", NameAr = "جنوسان", CountryId = bahrain.Id, DisplayOrder = 31, AramexCode = "JAN" },
            new() { Name = "Budaiya", NameAr = "البديع", CountryId = bahrain.Id, DisplayOrder = 32, AramexCode = "BUD" },
            new() { Name = "Barbar", NameAr = "باربار", CountryId = bahrain.Id, DisplayOrder = 33, AramexCode = "BAR" },
            new() { Name = "Sar", NameAr = "سار", CountryId = bahrain.Id, DisplayOrder = 34, AramexCode = "SAR" },
            new() { Name = "Shakhurah", NameAr = "الشاخورة", CountryId = bahrain.Id, DisplayOrder = 35, AramexCode = "SHK" },
            new() { Name = "Karzakan", NameAr = "كرزكان", CountryId = bahrain.Id, DisplayOrder = 36, AramexCode = "KAR" },
            new() { Name = "Diraz", NameAr = "الدراز", CountryId = bahrain.Id, DisplayOrder = 37, AramexCode = "DIR" },
            new() { Name = "Bani Jamrah", NameAr = "بني جمرة", CountryId = bahrain.Id, DisplayOrder = 38, AramexCode = "BJM" },
            new() { Name = "Abu Saiba", NameAr = "أبو صيبع", CountryId = bahrain.Id, DisplayOrder = 39, AramexCode = "ABS" },
            new() { Name = "Dumistan", NameAr = "دميستان", CountryId = bahrain.Id, DisplayOrder = 40, AramexCode = "DUM" },
            new() { Name = "Jid Ali", NameAr = "جد علي", CountryId = bahrain.Id, DisplayOrder = 41, AramexCode = "JDA" },
            new() { Name = "Al Jasra", NameAr = "الجسرة", CountryId = bahrain.Id, DisplayOrder = 42, AramexCode = "JAS" },
            
            // Southern Governorate
            new() { Name = "Riffa", NameAr = "الرفاع", CountryId = bahrain.Id, DisplayOrder = 50, AramexCode = "RIF" },
            new() { Name = "East Riffa", NameAr = "الرفاع الشرقي", CountryId = bahrain.Id, DisplayOrder = 51, AramexCode = "ERF" },
            new() { Name = "West Riffa", NameAr = "الرفاع الغربي", CountryId = bahrain.Id, DisplayOrder = 52, AramexCode = "WRF" },
            new() { Name = "Hamad Town", NameAr = "مدينة حمد", CountryId = bahrain.Id, DisplayOrder = 53, AramexCode = "HAM" },
            new() { Name = "Isa Town", NameAr = "مدينة عيسى", CountryId = bahrain.Id, DisplayOrder = 54, AramexCode = "ISA" },
            new() { Name = "Sitra", NameAr = "سترة", CountryId = bahrain.Id, DisplayOrder = 55, AramexCode = "SIT" },
            new() { Name = "Awali", NameAr = "العوالي", CountryId = bahrain.Id, DisplayOrder = 56, AramexCode = "AWL" },
            new() { Name = "Zallaq", NameAr = "الزلاق", CountryId = bahrain.Id, DisplayOrder = 57, AramexCode = "ZAL" },
            new() { Name = "Askar", NameAr = "عسكر", CountryId = bahrain.Id, DisplayOrder = 58, AramexCode = "ASK" },
            new() { Name = "Jaw", NameAr = "جو", CountryId = bahrain.Id, DisplayOrder = 59, AramexCode = "JAW" },
            new() { Name = "Alba", NameAr = "ألبا", CountryId = bahrain.Id, DisplayOrder = 60, AramexCode = "ALB" },
            new() { Name = "Dur", NameAr = "الدور", CountryId = bahrain.Id, DisplayOrder = 61, AramexCode = "DUR" },
            new() { Name = "Manama", NameAr = "المنامة", CountryId = bahrain.Id, DisplayOrder = 62, AramexCode = "MAN" },
            new() { Name = "Jidd Hafs", NameAr = "جد حفص", CountryId = bahrain.Id, DisplayOrder = 63, AramexCode = "JHF" },
            new() { Name = "Sanad", NameAr = "سند", CountryId = bahrain.Id, DisplayOrder = 64, AramexCode = "SND" },
            new() { Name = "Malkiya", NameAr = "المالكية", CountryId = bahrain.Id, DisplayOrder = 65, AramexCode = "MLK" },
            new() { Name = "Hawar Islands", NameAr = "جزر حوار", CountryId = bahrain.Id, DisplayOrder = 66, AramexCode = "HAW" }
        ]);

        _context.Cities.AddRange(cities);
        await _context.SaveChangesAsync();
        isCountriesSeeded = true;
        }

        // Seed Shipping Methods if not exist
        if (!await _context.ShippingMethods.AnyAsync())
        {
        // Create default shipping methods
        var shippingMethods = new List<ShippingMethod>
        {
            new()
            {
                Name = "Pickup from our Cafe",
                NameAr = "استلام من المقهى",
                CarrierCode = "PICKUP",
                Description = "Free pickup from our store in Muscat. Orders ready within 2 hours.",
                DescriptionAr = "استلام مجاني من متجرنا في مسقط. الطلبات جاهزة خلال ساعتين.",
                Type = ShippingMethodType.Pickup,
                IsFreePickup = true,
                DisplayOrder = 1
            },
            new()
            {
                Name = "NOOL Oman Delivery",
                NameAr = "توصيل نول عمان",
                CarrierCode = "NOOL",
                Description = "Fast delivery within Oman using NOOL delivery service. Same day delivery in Muscat.",
                DescriptionAr = "توصيل سريع داخل عمان باستخدام خدمة توصيل نول. توصيل في نفس اليوم في مسقط.",
                Type = ShippingMethodType.NoolOman,
                DeliveryDays = 1,
                DisplayOrder = 2
            },
            new()
            {
                Name = "Aramex International",
                NameAr = "أرامكس الدولية",
                CarrierCode = "ARAMEX",
                Description = "International shipping to GCC countries via Aramex. Reliable and tracked delivery.",
                DescriptionAr = "شحن دولي إلى دول مجلس التعاون الخليجي عبر أرامكس. توصيل موثوق ومتتبع.",
                Type = ShippingMethodType.Aramex,
                AramexVersion = "v1.0",
                AramexAccountCountryCode = "OM",
                AramexApiUrl = "https://ws.aramex.net/ShippingAPI.V2/",
                DisplayOrder = 3
            }
        };

        _context.ShippingMethods.AddRange(shippingMethods);
        await _context.SaveChangesAsync();
        isShippingMethodsSeeded = true;
        }

        // Seed Shipping Zones if not exist
        if (!await _context.ShippingZones.AnyAsync())
        {
            var methods = await _context.ShippingMethods.ToListAsync();
            var countries = await _context.Countries.ToListAsync();
            var zones = new List<ShippingZone>();

            foreach (var method in methods)
            {
                foreach (var country in countries)
                {
                    zones.Add(new ShippingZone
                    {
                        Name = $"{method.Name} - {country.Name}",
                        NameAr = $"{method.NameAr} - {country.NameAr}",
                        ShippingMethodId = method.Id,
                        CountryId = country.Id,
                        IsActive = true
                    });
                }
            }

            _context.ShippingZones.AddRange(zones);
            await _context.SaveChangesAsync();
            isShippingZonesSeeded = true;
        }

        // Seed Shipping Rates if not exist
        if (!await _context.ShippingRates.AnyAsync())
        {
            var zones = await _context.ShippingZones.Include(z => z.Country).Include(z => z.ShippingMethod).ToListAsync();
            var cities = await _context.Cities.ToListAsync();
            var rates = new List<ShippingRate>();

            foreach (var zone in zones)
            {
                var citiesInCountry = cities.Where(c => c.CountryId == zone.CountryId).ToList();
                
                foreach (var city in citiesInCountry)
                {
                    decimal rate;
                    
                    // Set different rates based on method and country
                    if (zone.ShippingMethod?.Type == ShippingMethodType.Pickup)
                    {
                        rate = zone.Country?.Code == "OMN" ? 2.5m : 5.0m; // OMR
                    }
                    else if (zone.ShippingMethod?.Type == ShippingMethodType.NoolOman)
                    {
                        rate = zone.Country?.Code == "OMN" ? 3.0m : 7.0m; // OMR
                    }
                    else // Aramex
                    {
                        rate = zone.Country?.Code == "OMN" ? 4.5m : 8.5m; // OMR
                    }

                    rates.Add(new ShippingRate
                    {
                        ShippingZoneId = zone.Id,
                        CityId = city.Id,
                        Rate = rate,
                        MinOrderAmount = 0.0m,
                        IsActive = true
                    });
                }
            }

            _context.ShippingRates.AddRange(rates);
            await _context.SaveChangesAsync();
            isShippingRatesSeeded = true;
        }

        return isCountriesSeeded || isShippingMethodsSeeded || isShippingZonesSeeded || isShippingRatesSeeded;
    }

    #endregion
}