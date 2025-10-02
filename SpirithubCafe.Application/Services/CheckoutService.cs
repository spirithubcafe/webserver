using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SpirithubCafe.Application.Services;

public interface ICheckoutService
{
    Task<IEnumerable<CountryDto>> GetActiveCountriesAsync();
    Task<IEnumerable<CityDto>> GetActiveCitiesByCountryAsync(int countryId);
    Task<IEnumerable<ShippingMethodDto>> GetAvailableShippingMethodsAsync(int countryId);
    Task<decimal> CalculateShippingCostAsync(int shippingMethodId, int? cityId = null);
    Task<decimal> CalculateTaxAsync(IEnumerable<CartItem> cartItems);
    Task<Order> CreateOrderAsync(Order order);
    Task<string> GenerateOrderNumberAsync();
    Task<bool> IsOmanCountryAsync(int countryId);
    Task<(Order order, Payment payment)> CreateOrderWithPaymentAsync(Order order, string gateway);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<Order?> GetOrderByNumberAsync(string orderNumber);
}

public class CheckoutService : ICheckoutService
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;

    public CheckoutService(IApplicationDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<IEnumerable<CountryDto>> GetActiveCountriesAsync()
    {
        return await _context.Countries
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                NameAr = c.NameAr
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<CityDto>> GetActiveCitiesByCountryAsync(int countryId)
    {
        return await _context.Cities
            .Where(c => c.CountryId == countryId && c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                NameAr = c.NameAr,
                CountryId = c.CountryId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ShippingMethodDto>> GetAvailableShippingMethodsAsync(int countryId)
    {
        var country = await _context.Countries.FindAsync(countryId);
        if (country == null) return new List<ShippingMethodDto>();

        var allMethods = await _context.ShippingMethods
            .Where(sm => sm.IsActive)
            .OrderBy(sm => sm.DisplayOrder)
            .ToListAsync();

        var availableMethods = new List<ShippingMethodDto>();

        foreach (var method in allMethods)
        {
            // Always include pickup
            if (method.Type == "Pickup")
            {
                availableMethods.Add(new ShippingMethodDto
                {
                    Id = method.Id,
                    Type = method.Type,
                    Name = method.Name,
                    NameAr = method.NameAr,
                    Description = method.Description,
                    DescriptionAr = method.DescriptionAr,
                    Cost = 0 // Free pickup
                });
                continue;
            }

            // Include Nool only for Oman
            if (method.Type == "NoolOman" && country.Code == "OM")
            {
                availableMethods.Add(new ShippingMethodDto
                {
                    Id = method.Id,
                    Type = method.Type,
                    Name = method.Name,
                    NameAr = method.NameAr,
                    Description = method.Description,
                    DescriptionAr = method.DescriptionAr,
                    Cost = 0 // Will be calculated based on city selection
                });
                continue;
            }

            // Include Aramex for all countries except Oman
            if (method.Type == "Aramex" && country.Code != "OM")
            {
                availableMethods.Add(new ShippingMethodDto
                {
                    Id = method.Id,
                    Type = method.Type,
                    Name = method.Name,
                    NameAr = method.NameAr,
                    Description = method.Description,
                    DescriptionAr = method.DescriptionAr,
                    Cost = 1 // Placeholder - will use API later
                });
                continue;
            }
        }

        return availableMethods;
    }

    public async Task<decimal> CalculateShippingCostAsync(int shippingMethodId, int? cityId = null)
    {
        var shippingMethod = await _context.ShippingMethods.FindAsync(shippingMethodId);
        if (shippingMethod == null) return 0;

        switch (shippingMethod.Type)
        {
            case "Pickup":
                return 0; // Free pickup

            case "NoolOman":
                if (cityId.HasValue)
                {
                    var noolRate = await _context.NoolShippingRates
                        .FirstOrDefaultAsync(r => r.ShippingMethodId == shippingMethodId 
                                                && r.CityId == cityId.Value 
                                                && r.IsActive);
                    return noolRate?.Rate ?? 1; // Default to 1 OMR if no rate found
                }
                return 1; // Default rate

            case "Aramex":
                // TODO: Implement Aramex API integration
                // For now, return 1 OMR as requested
                return 1;

            default:
                return 0;
        }
    }

    public async Task<decimal> CalculateTaxAsync(IEnumerable<CartItem> cartItems)
    {
        decimal totalTax = 0;

        foreach (var item in cartItems)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product?.Category != null)
            {
                var itemSubtotal = item.UnitPrice * item.Quantity;
                var taxAmount = itemSubtotal * (product.Category.TaxPercentage / 100);
                totalTax += taxAmount;
            }
        }

        return totalTax;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        // Generate order number
        order.OrderNumber = await GenerateOrderNumberAsync();

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrder = await _context.Orders
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync();

        var nextNumber = (lastOrder?.Id ?? 0) + 1;
        return $"ORD-{DateTime.UtcNow:yyyyMM}-{nextNumber:D6}";
    }

    public async Task<bool> IsOmanCountryAsync(int countryId)
    {
        var country = await _context.Countries.FindAsync(countryId);
        return country?.Code == "OM";
    }

    public async Task<(Order order, Payment payment)> CreateOrderWithPaymentAsync(Order order, string gateway)
    {
        // Create order with unpaid status
        order.PaymentStatus = "Unpaid";
        order.Status = "Pending";
        var createdOrder = await CreateOrderAsync(order);

        // Create payment record
        var createPaymentDto = new CreatePaymentDto
        {
            OrderId = createdOrder.Id,
            Amount = createdOrder.TotalAmount,
            Currency = "OMR", // You can make this configurable
            Gateway = gateway
        };

        var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);

        return (createdOrder, payment);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Include(o => o.Country)
            .Include(o => o.City)
            .Include(o => o.ShippingMethod)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
}