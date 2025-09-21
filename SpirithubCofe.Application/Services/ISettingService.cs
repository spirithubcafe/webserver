using SpirithubCofe.Application.DTOs;

namespace SpirithubCofe.Application.Services;

public interface ISettingService
{
    Task<IEnumerable<SettingDto>> GetAllAsync();
    Task<IEnumerable<SettingDto>> GetByCategoryAsync(string category);
    Task<SettingDto?> GetByKeyAsync(string key);
    Task<string> GetValueAsync(string key, string defaultValue = "");
    Task<SettingDto> CreateAsync(CreateSettingDto dto);
    Task<SettingDto> UpdateAsync(string key, UpdateSettingDto dto);
    Task DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
}