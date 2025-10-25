using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SpirithubCafe.Web.Services;

public class CartItemDto
{
    public int ProductId { get; set; }
    public int? VariantId { get; set; }  
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = "";
    public string VariantInfo { get; set; } = "";
    public decimal Weight { get; set; } = 0;
    public string WeightUnit { get; set; } = "g";
}

public class CartService
{
    private List<CartItemDto> _cartItems = new();
    private readonly ProtectedLocalStorage _localStorage;
    private const string CART_STORAGE_KEY = "spirithub_cart";
    private bool _isInitialized = false;
    
    public event Action? OnCartChanged;
    
    public CartService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }
    
    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            await LoadCartFromStorage();
            _isInitialized = true;
        }
    }
    
    public IReadOnlyList<CartItemDto> Items => _cartItems.AsReadOnly();
    
    public int ItemCount => _cartItems.Sum(item => item.Quantity);
    
    public decimal TotalPrice => _cartItems.Sum(item => item.Price * item.Quantity);
    public decimal Subtotal => TotalPrice;
    
    public event Action? OnChange
    {
        add => OnCartChanged += value;
        remove => OnCartChanged -= value;
    }
    
    public async Task AddToCartAsync(int productId, string name, decimal price, string imageUrl = "", int quantity = 1, int? variantId = null, string variantInfo = "", decimal weight = 0, string weightUnit = "g")
    {
        var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _cartItems.Add(new CartItemDto
            {
                ProductId = productId,
                VariantId = variantId,
                Name = name,
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl,
                VariantInfo = variantInfo,
                Weight = weight,
                WeightUnit = weightUnit
            });
        }
        
        await SaveCartToStorage();
        OnCartChanged?.Invoke();
    }
    
    public async Task UpdateQuantityAsync(int productId, int quantity, int? variantId = null)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                _cartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            
            await SaveCartToStorage();
            OnCartChanged?.Invoke();
        }
    }
    
    public async Task RemoveFromCartAsync(int productId, int? variantId = null)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
        if (item != null)
        {
            _cartItems.Remove(item);
            await SaveCartToStorage();
            OnCartChanged?.Invoke();
        }
    }
    
    public async Task ClearCart()
    {
        _cartItems.Clear();
        await SaveCartToStorage();
        OnCartChanged?.Invoke();
    }
    
    public async Task ClearCartAsync()
    {
        _cartItems.Clear();
        await SaveCartToStorage();
        OnCartChanged?.Invoke();
    }
    
    public bool HasItem(int productId, int? variantId = null)
    {
        return _cartItems.Any(x => x.ProductId == productId && x.VariantId == variantId);
    }
    
    public int GetItemQuantity(int productId, int? variantId = null)
    {
        return _cartItems.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId)?.Quantity ?? 0;
    }
    
    private async Task LoadCartFromStorage()
    {
        try
        {
            var result = await _localStorage.GetAsync<List<CartItemDto>>(CART_STORAGE_KEY);
            if (result.Success && result.Value != null)
            {
                _cartItems = result.Value;
            }
            else
            {
                _cartItems = new List<CartItemDto>();
            }
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Error loading cart from storage: {ex.Message}");
            _cartItems = new List<CartItemDto>();
        }
    }
    
    private async Task SaveCartToStorage()
    {
        try
        {
            await _localStorage.SetAsync(CART_STORAGE_KEY, _cartItems);
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Error saving cart to storage: {ex.Message}");
        }
    }
}