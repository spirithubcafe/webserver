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
    private List<CartItem> _cartItems = new();
    private readonly string _cartFilePath;
    
    public event Action? OnCartChanged;
    
    public CartService()
    {
        // Store cart data in temp directory
        _cartFilePath = Path.Combine(Path.GetTempPath(), "spirithub-cart.json");
    }
    
    public async Task InitializeAsync()
    {
        await LoadCartFromFile();
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
        
        await SaveCartToFile();
        OnCartChanged?.Invoke();
    }
    
    public async Task UpdateQuantityAsync(int productId, int quantity)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
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
            
            await SaveCartToFile();
            OnCartChanged?.Invoke();
        }
    }
    
    public async Task RemoveFromCartAsync(int productId)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _cartItems.Remove(item);
            await SaveCartToFile();
            OnCartChanged?.Invoke();
        }
    }
    
    public async Task ClearCartAsync()
    {
        _cartItems.Clear();
        await SaveCartToFile();
        OnCartChanged?.Invoke();
    }
    
    public bool HasItem(int productId)
    {
        return _cartItems.Any(x => x.ProductId == productId);
    }
    
    public int GetItemQuantity(int productId)
    {
        return _cartItems.FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;
    }
    
    private async Task LoadCartFromFile()
    {
        try
        {
            if (File.Exists(_cartFilePath))
            {
                var cartJson = await File.ReadAllTextAsync(_cartFilePath);
                if (!string.IsNullOrEmpty(cartJson))
                {
                    var items = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                    _cartItems = items ?? new List<CartItem>();
                }
            }
        }
        catch (Exception)
        {
            _cartItems = new List<CartItem>();
        }
    }
    
    private async Task SaveCartToFile()
    {
        try
        {
            var cartJson = JsonSerializer.Serialize(_cartItems, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(_cartFilePath, cartJson);
        }
        catch (Exception)
        {
            // Handle error silently
        }
    }
}