using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Application.DTOs;

namespace SpirithubCafe.Web.Services;

/// <summary>
/// Service for managing orders
/// </summary>
public class OrderService
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly ProductService _productService;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentGatewayService _paymentGatewayService;

    public OrderService(
        ApplicationDbContext context, 
        CartService cartService, 
        ProductService productService,
        IPaymentService paymentService,
        IPaymentGatewayService paymentGatewayService)
    {
        _context = context;
        _cartService = cartService;
        _productService = productService;
        _paymentService = paymentService;
        _paymentGatewayService = paymentGatewayService;
    }

    /// <summary>
    /// Create a new order from current cart
    /// </summary>
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // Get current cart items
        var cartItems = _cartService.Items;
        if (!cartItems.Any())
        {
            throw new InvalidOperationException("Cart is empty");
        }

        // Generate unique order number
        var orderNumber = await GenerateOrderNumberAsync();

        // Create order
        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = request.UserId ?? "guest",
            Status = "Pending",
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            CountryId = request.CountryId,
            CityId = request.CityId,
            PostalCode = request.PostalCode,
            ShippingMethodId = request.ShippingMethodId,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Calculate amounts
        decimal subtotal = 0;
        decimal totalTax = 0;

        // Add order items
        foreach (var cartItem in cartItems)
        {
            var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
            if (product == null) continue;

            // Get product variant if specified
            ProductVariant? variant = null;
            if (cartItem.VariantId.HasValue)
            {
                variant = product.Variants?.FirstOrDefault(v => v.Id == cartItem.VariantId.Value);
            }

            // Determine price and variant info
            var unitPrice = variant?.Price ?? product.Variants?.FirstOrDefault()?.Price ?? 0;
            var variantInfo = variant != null ? $"Variant: {variant.VariantSku}" : null;

            // Calculate tax for this item
            var taxPercentage = product.Category?.TaxPercentage ?? 0;
            var itemSubtotal = unitPrice * cartItem.Quantity;
            var itemTaxAmount = itemSubtotal * (taxPercentage / 100);

            var orderItem = new OrderItem
            {
                ProductId = cartItem.ProductId,
                ProductVariantId = cartItem.VariantId,
                ProductName = product.Name,
                VariantInfo = variantInfo,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
                TaxPercentage = taxPercentage,
                TaxAmount = itemTaxAmount,
                TotalAmount = itemSubtotal + itemTaxAmount
            };

            order.OrderItems.Add(orderItem);
            subtotal += itemSubtotal;
            totalTax += itemTaxAmount;
        }

        // Set order totals
        order.SubTotal = subtotal;
        order.TaxAmount = totalTax;
        order.ShippingCost = request.ShippingCost;
        order.TotalAmount = subtotal + totalTax + request.ShippingCost;

        // Save order to database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    /// <summary>
    /// Create a new order with payment integration
    /// </summary>
    public async Task<(Order order, string paymentUrl)> CreateOrderWithPaymentAsync(CreateOrderRequest request)
    {
        // Create the order first with unpaid status
        var order = await CreateOrderAsync(request);
        
        // Update order to unpaid status
        order.PaymentStatus = "Unpaid";
        await _context.SaveChangesAsync();
        
        // Create payment record
        var createPaymentDto = new CreatePaymentDto
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            Currency = "OMR",
            Gateway = "Bank Muscat"
        };
        
        var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
        
        // Prepare payment request
        var paymentRequest = new PaymentGatewayRequestDto
        {
            PaymentReference = payment.PaymentReference,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CustomerName = $"{order.FirstName} {order.LastName}",
            CustomerEmail = order.Email,
            CustomerPhone = order.Phone,
            ReturnUrl = $"/checkout/payment-success?PaymentRef={payment.PaymentReference}",
            CancelUrl = $"/checkout/payment-cancelled?PaymentRef={payment.PaymentReference}",
            CallbackUrl = "/api/payment/callback"
        };
        
        // Generate payment URL
        var paymentUrl = await _paymentGatewayService.GeneratePaymentUrlAsync(paymentRequest);
        
        return (order, paymentUrl);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// Get order by order number
    /// </summary>
    public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.MainImage)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    /// <summary>
    /// Get user orders with pagination
    /// </summary>
    public async Task<(List<Order> Orders, int TotalCount)> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.MainImage)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    /// <summary>
    /// Get all orders with pagination (for admin)
    /// </summary>
    public async Task<(List<Order> Orders, int TotalCount)> GetAllOrdersAsync(int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.MainImage)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    /// <summary>
    /// Get count of pending orders
    /// </summary>
    public async Task<int> GetPendingOrdersCountAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == "Pending")
            .CountAsync();
    }

    /// <summary>
    /// Update order status
    /// </summary>
    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return false;

        order.Status = status;
        if (!string.IsNullOrEmpty(trackingNumber))
        {
            order.TrackingNumber = trackingNumber;
        }
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get orders for a specific user
    /// </summary>
    public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Update order status
    /// </summary>
    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Generate unique order number
    /// </summary>
    private async Task<string> GenerateOrderNumberAsync()
    {
        string orderNumber;
        bool exists;

        do
        {
            // Format: SH-YYYYMMDD-XXXX (SH = SpiritHub)
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var randomPart = new Random().Next(1000, 9999);
            orderNumber = $"SH-{datePart}-{randomPart}";

            exists = await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
        }
        while (exists);

        return orderNumber;
    }
}

/// <summary>
/// Request model for creating an order
/// </summary>
public class CreateOrderRequest
{
    public string? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public int CountryId { get; set; }
    public int CityId { get; set; }
    public string? PostalCode { get; set; }
    public int ShippingMethodId { get; set; }
    public decimal ShippingCost { get; set; }
    public string? Notes { get; set; }
}