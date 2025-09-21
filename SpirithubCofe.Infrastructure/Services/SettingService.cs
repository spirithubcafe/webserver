using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpirithubCofe.Application.DTOs;
using SpirithubCofe.Application.Services;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Infrastructure.Services;

public class SettingService : ISettingService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SettingService> _logger;

    public SettingService(IApplicationDbContext context, ILogger<SettingService> logger)
    {
        _context = context;
        _logger = logger;
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
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting?.Value ?? defaultValue;
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