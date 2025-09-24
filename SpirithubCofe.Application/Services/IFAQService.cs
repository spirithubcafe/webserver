using SpirithubCofe.Application.DTOs;

namespace SpirithubCofe.Application.Services;

public interface IFAQService
{
    // FAQ Management
    Task<IEnumerable<FAQDto>> GetAllFAQsAsync();
    Task<IEnumerable<FAQDto>> GetActiveFAQsAsync();
    Task<IEnumerable<FAQDto>> GetFAQsByCategoryAsync(int categoryId);
    Task<FAQDto?> GetFAQByIdAsync(int id);
    Task<FAQDto> CreateFAQAsync(CreateFAQDto createDto);
    Task<FAQDto> UpdateFAQAsync(UpdateFAQDto updateDto);
    Task<bool> DeleteFAQAsync(int id);
    Task<bool> ReorderFAQsAsync(Dictionary<int, int> orderUpdates);

    // FAQ Categories
    Task<IEnumerable<FAQCategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<FAQCategoryDto>> GetActiveCategoriesAsync();
    Task<FAQCategoryDto?> GetCategoryByIdAsync(int id);
    Task<FAQCategoryDto> CreateCategoryAsync(CreateFAQCategoryDto createDto);
    Task<FAQCategoryDto> UpdateCategoryAsync(UpdateFAQCategoryDto updateDto);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> ReorderCategoriesAsync(Dictionary<int, int> orderUpdates);

    // FAQ Page Settings
    Task<FAQPageDto> GetFAQPageSettingsAsync();
    Task<FAQPageDto> UpdateFAQPageSettingsAsync(FAQPageDto updateDto);

    // Public Methods
    Task<IEnumerable<FAQDto>> GetPublicFAQsAsync(string? categorySlug = null);
    Task<IEnumerable<FAQCategoryDto>> GetPublicCategoriesAsync();
}