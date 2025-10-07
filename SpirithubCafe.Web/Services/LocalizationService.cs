namespace SpirithubCafe.Web.Services;

public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] arguments);
}

public class LocalizationService : ILocalizationService
{
    private readonly TranslationLocalizer _localizer;

    public LocalizationService(TranslationLocalizer localizer)
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