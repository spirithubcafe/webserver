using Microsoft.JSInterop;
using System.Text.Json;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Web.Services;

public class FavoriteService
{
    private readonly IJSRuntime _jsRuntime;
    private const string STORAGE_KEY = "spirithubcafe_favorites";

    public FavoriteService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<int>> GetFavoriteIdsAsync()
    {
        try
        {
            // Skip during prerendering
            if (_jsRuntime is not IJSInProcessRuntime)
            {
                return new List<int>();
            }
            
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            if (string.IsNullOrEmpty(json))
                return new List<int>();

            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    public async Task<bool> IsFavoriteAsync(int productId)
    {
        var favorites = await GetFavoriteIdsAsync();
        return favorites.Contains(productId);
    }

    public async Task AddFavoriteAsync(int productId)
    {
        var favorites = await GetFavoriteIdsAsync();
        if (!favorites.Contains(productId))
        {
            favorites.Add(productId);
            await SaveFavoritesAsync(favorites);
        }
    }

    public async Task RemoveFavoriteAsync(int productId)
    {
        var favorites = await GetFavoriteIdsAsync();
        if (favorites.Contains(productId))
        {
            favorites.Remove(productId);
            await SaveFavoritesAsync(favorites);
        }
    }

    public async Task<bool> ToggleFavoriteAsync(int productId)
    {
        var favorites = await GetFavoriteIdsAsync();
        bool isFavorite;
        
        if (favorites.Contains(productId))
        {
            favorites.Remove(productId);
            isFavorite = false;
        }
        else
        {
            favorites.Add(productId);
            isFavorite = true;
        }
        
        await SaveFavoritesAsync(favorites);
        return isFavorite;
    }

    public async Task<int> GetFavoriteCountAsync()
    {
        var favorites = await GetFavoriteIdsAsync();
        return favorites.Count;
    }

    private async Task SaveFavoritesAsync(List<int> favorites)
    {
        try
        {
            // Skip during prerendering
            if (_jsRuntime is not IJSInProcessRuntime)
            {
                return;
            }
            
            var json = JsonSerializer.Serialize(favorites);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch
        {
            // Handle error silently
        }
    }

    public async Task ClearFavoritesAsync()
    {
        // Skip during prerendering
        if (_jsRuntime is not IJSInProcessRuntime)
        {
            return;
        }
        
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", STORAGE_KEY);
    }
}
