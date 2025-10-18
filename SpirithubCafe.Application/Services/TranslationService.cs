using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Application.Services;

public class TranslationService : ITranslationService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Translation_";
    private const int CacheExpirationMinutes = 1440; // 24 hours - translations rarely change

    public TranslationService(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<string> GetTranslationAsync(string key, string language, string? category = null)
    {
        var cacheKey = $"{CacheKeyPrefix}{language}_{key}";
        
        if (_cache.TryGetValue(cacheKey, out string? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var translation = await _context.Translations
            .FirstOrDefaultAsync(t => t.Key == key);

        if (translation != null)
        {
            var value = language == "ar" ? translation.ValueAr : translation.ValueEn;
            _cache.Set(cacheKey, value, TimeSpan.FromMinutes(CacheExpirationMinutes));
            return value;
        }

        // If translation doesn't exist, create a placeholder with the key as value
        var newTranslation = new Translation
        {
            Key = key,
            ValueEn = key, // Use key as default value for English
            ValueAr = key, // Use key as default value for Arabic
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        _context.Translations.Add(newTranslation);
        await _context.SaveChangesAsync();

        _cache.Set(cacheKey, key, TimeSpan.FromMinutes(CacheExpirationMinutes));
        return key;
    }

    public async Task<Dictionary<string, string>> GetTranslationsAsync(string language, string? category = null)
    {
        var cacheKey = $"{CacheKeyPrefix}All_{language}_{category ?? "all"}";
        
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, string>? cachedTranslations) && cachedTranslations != null)
        {
            return cachedTranslations;
        }

        var query = _context.Translations.AsQueryable();
        
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(t => t.Category == category);
        }

        var translations = await query.ToListAsync();
        
        var result = translations.ToDictionary(
            t => t.Key,
            t => language == "ar" ? t.ValueAr : t.ValueEn
        );

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(CacheExpirationMinutes));
        return result;
    }

    public async Task<bool> SetTranslationAsync(string key, string language, string value, string? category = null)
    {
        var translation = await _context.Translations
            .FirstOrDefaultAsync(t => t.Key == key);

        if (translation != null)
        {
            if (language == "ar")
                translation.ValueAr = value;
            else
                translation.ValueEn = value;
                
            translation.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(category))
            {
                translation.Category = category;
            }
        }
        else
        {
            translation = new Translation
            {
                Key = key,
                ValueEn = language == "en" ? value : key,
                ValueAr = language == "ar" ? value : key,
                Category = category,
                CreatedAt = DateTime.UtcNow
            };
            _context.Translations.Add(translation);
        }

        await _context.SaveChangesAsync();
        
        // Clear cache
        ClearTranslationCache(key);
        
        return true;
    }

    public async Task<bool> UpdateTranslationAsync(int id, string valueEn, string valueAr)
    {
        var translation = await _context.Translations.FindAsync(id);
        
        if (translation == null)
            return false;

        translation.ValueEn = valueEn;
        translation.ValueAr = valueAr;
        translation.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Clear cache
        ClearTranslationCache(translation.Key);
        
        return true;
    }

    public async Task<List<(int Id, string Key, string ValueEn, string ValueAr, string? Category)>> GetAllTranslationsAsync()
    {
        var translations = await _context.Translations
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Key)
            .Select(t => new
            {
                t.Id,
                t.Key,
                t.ValueEn,
                t.ValueAr,
                t.Category
            })
            .ToListAsync();

        return translations.Select(t => (t.Id, t.Key, t.ValueEn, t.ValueAr, t.Category)).ToList();
    }

    public async Task<List<(int Id, string Key, string ValueEn, string ValueAr, string? Category)>> GetIncompleteTranslationsAsync()
    {
        var translations = await _context.Translations
            .Where(t => t.ValueEn == t.Key || t.ValueAr == t.Key) // Where translation equals the key (not translated)
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Key)
            .Select(t => new
            {
                t.Id,
                t.Key,
                t.ValueEn,
                t.ValueAr,
                t.Category
            })
            .ToListAsync();

        return translations.Select(t => (t.Id, t.Key, t.ValueEn, t.ValueAr, t.Category)).ToList();
    }

    public async Task<bool> DeleteTranslationAsync(int id)
    {
        var translation = await _context.Translations.FindAsync(id);
        
        if (translation == null)
            return false;

        _context.Translations.Remove(translation);
        await _context.SaveChangesAsync();
        
        // Clear cache
        ClearTranslationCache(translation.Key);
        
        return true;
    }

    private void ClearTranslationCache(string key)
    {
        // Clear both language caches
        _cache.Remove($"{CacheKeyPrefix}en_{key}");
        _cache.Remove($"{CacheKeyPrefix}ar_{key}");
        
        // Also clear "all translations" cache for both languages
        _cache.Remove($"{CacheKeyPrefix}All_en_all");
        _cache.Remove($"{CacheKeyPrefix}All_ar_all");
    }
}
