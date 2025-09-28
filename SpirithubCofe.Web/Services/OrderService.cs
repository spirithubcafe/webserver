using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<OrderStats> GetOrderStatsAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        
        return new OrderStats
        {
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing),
            CompletedOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
            TotalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Total)
        };
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return;

        if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
        {
            order.Status = orderStatus;
        }

        if (!string.IsNullOrEmpty(trackingNumber))
        {
            order.TrackingNumber = trackingNumber;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Order> CreateOrderAsync(string userId, List<CartItem> cartItems, ShippingAddress shippingAddress, 
        string shippingMethod, decimal shippingCost)
    {
        var subtotal = cartItems.Sum(item => item.UnitPrice * item.Quantity);
        var total = subtotal + shippingCost;

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = total,
            ShippingProvider = shippingMethod,
            ShippingAddress = shippingAddress,
            CreatedAt = DateTime.UtcNow,
            Items = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? "Unknown Product",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }
}

public class OrderStats
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}