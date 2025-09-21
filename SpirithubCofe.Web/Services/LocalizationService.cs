using Microsoft.Extensions.Localization;
using SpirithubCofe.Langs;

namespace SpirithubCofe.Web.Services;

public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] arguments);
}

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<Resources> _localizer;

    public LocalizationService(IStringLocalizer<Resources> localizer)
    {
        _localizer = localizer;
    }

    public string GetString(string key)
    {
        return _localizer[key];
    }

    public string GetString(string key, params object[] arguments)
    {
        return _localizer[key, arguments];
    }
}