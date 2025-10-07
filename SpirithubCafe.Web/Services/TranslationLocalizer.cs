using Microsoft.Extensions.Localization;
using SpirithubCafe.Application.Interfaces;

namespace SpirithubCafe.Web.Services;

public class TranslationLocalizer : IStringLocalizer
{
    private readonly ITranslationService _translationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TranslationLocalizer(ITranslationService translationService, IHttpContextAccessor httpContextAccessor)
    {
        _translationService = translationService;
        _httpContextAccessor = httpContextAccessor;
    }

    private string CurrentLanguage
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

    public LocalizedString this[string name]
    {
        get
        {
            var value = _translationService.GetTranslationAsync(name, CurrentLanguage).GetAwaiter().GetResult();
            return new LocalizedString(name, value, resourceNotFound: value == name);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = _translationService.GetTranslationAsync(name, CurrentLanguage).GetAwaiter().GetResult();
            var value = string.Format(format, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == name);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var translations = _translationService.GetTranslationsAsync(CurrentLanguage).GetAwaiter().GetResult();
        return translations.Select(t => new LocalizedString(t.Key, t.Value, resourceNotFound: false));
    }
}
