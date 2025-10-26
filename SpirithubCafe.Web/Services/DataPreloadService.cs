using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Application.Services;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Service to preload frequently accessed data into cache on startup
/// </summary>
public class DataPreloadService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataPreloadService> _logger;

    public DataPreloadService(IServiceProvider serviceProvider, ILogger<DataPreloadService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Preload critical data into cache
    /// </summary>
    public async Task PreloadCriticalDataAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // Preload translations for both languages in parallel
            var translationService = scope.ServiceProvider.GetRequiredService<ITranslationService>();
            var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();
            
            var tasks = new List<Task>
            {
                // Preload all translations for English
                translationService.GetTranslationsAsync("en"),
                
                // Preload all translations for Arabic
                translationService.GetTranslationsAsync("ar"),
                
                // Preload active categories
                categoryService.GetActiveCategoriesAsync(),
                
                // Preload critical settings
                PreloadCriticalSettings(settingService)
            };
            
            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Critical data preloaded successfully into cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to preload critical data into cache");
        }
    }

    private async Task PreloadCriticalSettings(ISettingService settingService)
    {
        // Preload frequently accessed settings
        var settingKeys = new[]
        {
            "logo_type",
            "logo_text", 
            "logo_image",
            "home_logo_type",
            "home_logo_text",
            "home_logo_image",
            "site_title",
            "site_description"
        };

        var tasks = settingKeys.Select(key => settingService.GetValueAsync(key, ""));
        await Task.WhenAll(tasks);
    }
}
