using Microsoft.AspNetCore.Components;
using SpirithubCafe.Application.Interfaces;

namespace SpirithubCafe.Web.Services;

public class LocalizationHelper
{
    private readonly ITranslationService _translationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalizationHelper(ITranslationService translationService, IHttpContextAccessor httpContextAccessor)
    {
        _translationService = translationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public string CurrentLanguage
    {
        get
        {
            // Try to get culture from cookie
            var cultureCookie = _httpContextAccessor.HttpContext?.Request.Cookies["SpirithubCafe.Culture"];
            
            if (!string.IsNullOrEmpty(cultureCookie))
            {
                // Cookie format is "c=ar|uic=ar" or just "ar"
                if (cultureCookie.Contains("c="))
                {
                    var parts = cultureCookie.Split('|');
                    var culturePart = parts[0].Replace("c=", "");
                    return culturePart;
                }
                return cultureCookie;
            }
            
            // Fallback to default
            return "en";
        }
    }

    public async Task<string> T(string key, string? category = null)
    {
        return await _translationService.GetTranslationAsync(key, CurrentLanguage, category);
    }

    public async Task<Dictionary<string, string>> GetAll(string? category = null)
    {
        return await _translationService.GetTranslationsAsync(CurrentLanguage, category);
    }
}
