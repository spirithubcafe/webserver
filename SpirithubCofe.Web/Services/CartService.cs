using Microsoft.JSInterop;
using System.Text.Json;

namespace SpirithubCofe.Web.Services;

public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = "";
}

public class CartService
{
    private readonly IJSRuntime _jsRuntime;
    private List<CartItem> _cartItems = new();
    
    public event Action? OnCartChanged;
    
    public CartService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        await LoadCartFromLocalStorage();
    }
    
    public IReadOnlyList<CartItem> Items => _cartItems.AsReadOnly();
    
    public int ItemCount => _cartItems.Sum(item => item.Quantity);
    
    public decimal TotalPrice => _cartItems.Sum(item => item.Price * item.Quantity);
    
    public async Task AddToCartAsync(int productId, string name, decimal price, string imageUrl = "", int quantity = 1)
    {
        var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == productId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _cartItems.Add(new CartItem
            {
                ProductId = productId,
                Name = name,
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl
            });
        }
        
        await SaveCartToLocalStorage();
        OnCartChanged?.Invoke();
    }
    
    public async Task RemoveFromCartAsync(int productId)
    {
        _cartItems.RemoveAll(x => x.ProductId == productId);
        await SaveCartToLocalStorage();
        OnCartChanged?.Invoke();
    }
    
    public async Task UpdateQuantityAsync(int productId, int quantity)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                await RemoveFromCartAsync(productId);
            }
            else
            {
                item.Quantity = quantity;
                await SaveCartToLocalStorage();
                OnCartChanged?.Invoke();
            }
        }
    }
    
    public async Task ClearCartAsync()
    {
        _cartItems.Clear();
        await SaveCartToLocalStorage();
        OnCartChanged?.Invoke();
    }
    
    private async Task LoadCartFromLocalStorage()
    {
        try
        {
            var cartJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "spirithub-cart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                _cartItems = items ?? new List<CartItem>();
            }
        }
        catch (Exception)
        {
            _cartItems = new List<CartItem>();
        }
    }
    
    private async Task SaveCartToLocalStorage()
    {
        try
        {
            var cartJson = JsonSerializer.Serialize(_cartItems);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "spirithub-cart", cartJson);
        }
        catch (Exception)
        {
            // Handle error silently
        }
    }
}