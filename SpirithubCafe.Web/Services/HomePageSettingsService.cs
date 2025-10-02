using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace SpirithubCafe.Web.Services
{
    public class HomePageSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomePageSettingsService> _logger;

        public HomePageSettingsService(ApplicationDbContext context, ILogger<HomePageSettingsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HomePageSettings> GetSettingsAsync()
        {
            try
            {
                var settings = await _context.HomePageSettings
                    .OrderBy(h => h.Id)
                    .FirstOrDefaultAsync();
                
                if (settings == null)
                {
                    // Create default settings if none exist
                    settings = new HomePageSettings();
                    _context.HomePageSettings.Add(settings);
                    await _context.SaveChangesAsync();
                }
                
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting homepage settings");
                throw;
            }
        }

        public async Task<HomePageSettings> UpdateSettingsAsync(HomePageSettings settings)
        {
            try
            {
                var existingSettings = await _context.HomePageSettings
                    .OrderBy(h => h.Id)
                    .FirstOrDefaultAsync();
                
                if (existingSettings == null)
                {
                    settings.CreatedAt = DateTime.UtcNow;
                    settings.UpdatedAt = DateTime.UtcNow;
                    _context.HomePageSettings.Add(settings);
                }
                else
                {
                    existingSettings.ShowSlideshow = settings.ShowSlideshow;
                    
                    // Categories
                    existingSettings.ShowCategories = settings.ShowCategories;
                    existingSettings.CategoriesTitle = settings.CategoriesTitle;
                    existingSettings.CategoriesTitleAr = settings.CategoriesTitleAr;
                    existingSettings.CategoriesSubtitle = settings.CategoriesSubtitle;
                    existingSettings.CategoriesSubtitleAr = settings.CategoriesSubtitleAr;
                    existingSettings.CategoriesDisplayCount = settings.CategoriesDisplayCount;
                    existingSettings.CategoriesBgType = settings.CategoriesBgType;
                    existingSettings.CategoriesBgValue = settings.CategoriesBgValue;
                    
                    // Mission
                    existingSettings.ShowMission = settings.ShowMission;
                    existingSettings.MissionTitle = settings.MissionTitle;
                    existingSettings.MissionTitleAr = settings.MissionTitleAr;
                    existingSettings.MissionSubtitle = settings.MissionSubtitle;
                    existingSettings.MissionSubtitleAr = settings.MissionSubtitleAr;
                    existingSettings.MissionText = settings.MissionText;
                    existingSettings.MissionTextAr = settings.MissionTextAr;
                    existingSettings.MissionBgType = settings.MissionBgType;
                    existingSettings.MissionBgValue = settings.MissionBgValue;
                    
                    // Latest Products
                    existingSettings.ShowLatestProducts = settings.ShowLatestProducts;
                    existingSettings.LatestProductsTitle = settings.LatestProductsTitle;
                    existingSettings.LatestProductsTitleAr = settings.LatestProductsTitleAr;
                    existingSettings.LatestProductsSubtitle = settings.LatestProductsSubtitle;
                    existingSettings.LatestProductsSubtitleAr = settings.LatestProductsSubtitleAr;
                    existingSettings.LatestProductsCount = settings.LatestProductsCount;
                    existingSettings.LatestProductsBgType = settings.LatestProductsBgType;
                    existingSettings.LatestProductsBgValue = settings.LatestProductsBgValue;
                    
                    // Newsletter
                    existingSettings.ShowNewsletter = settings.ShowNewsletter;
                    existingSettings.NewsletterTitle = settings.NewsletterTitle;
                    existingSettings.NewsletterTitleAr = settings.NewsletterTitleAr;
                    existingSettings.NewsletterSubtitle = settings.NewsletterSubtitle;
                    existingSettings.NewsletterSubtitleAr = settings.NewsletterSubtitleAr;
                    
                    existingSettings.UpdatedAt = DateTime.UtcNow;
                    settings = existingSettings;
                }
                
                await _context.SaveChangesAsync();
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homepage settings");
                throw;
            }
        }
    }
}