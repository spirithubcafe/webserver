using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SpirithubCafe.Application.DTOs;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Infrastructure.Services;

public class SettingService : ISettingService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SettingService> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Setting_";
    private const int CacheExpirationHours = 24; // Settings rarely change

    public SettingService(IApplicationDbContext context, ILogger<SettingService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<SettingDto>> GetAllAsync()
    {
        var settings = await _context.Settings
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Key)
            .ToListAsync();

        return settings.Select(MapToDto);
    }

    public async Task<IEnumerable<SettingDto>> GetByCategoryAsync(string category)
    {
        var settings = await _context.Settings
            .Where(s => s.Category == category)
            .OrderBy(s => s.Key)
            .ToListAsync();

        return settings.Select(MapToDto);
    }

    public async Task<SettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting != null ? MapToDto(setting) : null;
    }

    public async Task<string> GetValueAsync(string key, string defaultValue = "")
    {
        var cacheKey = $"{CacheKeyPrefix}{key}";
        
        if (_cache.TryGetValue(cacheKey, out string? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        var value = setting?.Value ?? defaultValue;
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheExpirationHours),
            Size = 1 // Each cache entry counts as 1 unit
        };
        _cache.Set(cacheKey, value, cacheOptions);
        
        return value;
    }

    public async Task<SettingDto> CreateAsync(CreateSettingDto dto)
    {
        var setting = new Setting
        {
            Key = dto.Key,
            Value = dto.Value,
            Description = dto.Description,
            DescriptionAr = dto.DescriptionAr,
            Category = dto.Category,
            DataType = dto.DataType,
            IsRequired = dto.IsRequired,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Settings.Add(setting);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created setting with key: {Key}", dto.Key);
        return MapToDto(setting);
    }

    public async Task<SettingDto> UpdateAsync(string key, UpdateSettingDto dto)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            throw new InvalidOperationException($"Setting with key '{key}' not found");
        }

        setting.Value = dto.Value;
        setting.Description = dto.Description;
        setting.DescriptionAr = dto.DescriptionAr;
        setting.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        // Clear cache when updated
        var cacheKey = $"{CacheKeyPrefix}{key}";
        _cache.Remove(cacheKey);

        _logger.LogInformation("Updated setting with key: {Key}", key);
        return MapToDto(setting);
    }

    public async Task DeleteAsync(string key)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting != null)
        {
            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();
            
            // Clear cache when deleted
            var cacheKey = $"{CacheKeyPrefix}{key}";
            _cache.Remove(cacheKey);
            
            _logger.LogInformation("Deleted setting with key: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _context.Settings.AnyAsync(s => s.Key == key);
    }

    private static SettingDto MapToDto(Setting setting)
    {
        return new SettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            DescriptionAr = setting.DescriptionAr,
            Category = setting.Category,
            DataType = setting.DataType,
            IsRequired = setting.IsRequired,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}