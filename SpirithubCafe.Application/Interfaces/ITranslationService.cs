namespace SpirithubCafe.Application.Interfaces;

public interface ITranslationService
{
    Task<string> GetTranslationAsync(string key, string language, string? category = null);
    Task<Dictionary<string, string>> GetTranslationsAsync(string language, string? category = null);
    Task<bool> SetTranslationAsync(string key, string language, string value, string? category = null);
    Task<bool> UpdateTranslationAsync(int id, string valueEn, string valueAr);
    Task<List<(int Id, string Key, string ValueEn, string ValueAr, string? Category)>> GetAllTranslationsAsync();
    Task<List<(int Id, string Key, string ValueEn, string ValueAr, string? Category)>> GetIncompleteTranslationsAsync();
    Task<bool> DeleteTranslationAsync(int id);
}
