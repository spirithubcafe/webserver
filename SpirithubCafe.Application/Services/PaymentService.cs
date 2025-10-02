using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SpirithubCafe.Application.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
    Task<Payment?> GetPaymentByReferenceAsync(string paymentReference);
    Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);
    Task<Payment> UpdatePaymentStatusAsync(string paymentReference, PaymentCallbackDto callbackDto);
    Task<string> GeneratePaymentReferenceAsync();
    Task<PaymentGatewayRequestDto> PreparePaymentRequestAsync(int orderId);
    Task<bool> VerifyPaymentAsync(string paymentReference, decimal expectedAmount);
}

public class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _context;

    public PaymentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
    {
        var payment = new Payment
        {
            OrderId = createPaymentDto.OrderId,
            PaymentReference = await GeneratePaymentReferenceAsync(),
            Amount = createPaymentDto.Amount,
            Currency = createPaymentDto.Currency,
            Gateway = createPaymentDto.Gateway,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment?> GetPaymentByReferenceAsync(string paymentReference)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.PaymentReference == paymentReference);
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Payment> UpdatePaymentStatusAsync(string paymentReference, PaymentCallbackDto callbackDto)
    {
        var payment = await GetPaymentByReferenceAsync(paymentReference);
        if (payment == null)
        {
            throw new InvalidOperationException($"Payment with reference {paymentReference} not found");
        }

        payment.TransactionId = callbackDto.TransactionId;
        payment.Status = callbackDto.Status;
        payment.PaymentMethod = callbackDto.PaymentMethod;
        payment.GatewayResponse = callbackDto.GatewayResponse;
        payment.ErrorMessage = callbackDto.ErrorMessage;
        payment.UpdatedAt = DateTime.UtcNow;

        if (callbackDto.Status == "Completed" || callbackDto.Status == "Paid")
        {
            payment.CompletedAt = DateTime.UtcNow;
            
            // Update order payment status
            if (payment.Order != null)
            {
                payment.Order.PaymentStatus = "Paid";
                payment.Order.Status = "Pending"; // Ready for processing/shipping
                payment.Order.UpdatedAt = DateTime.UtcNow;
            }
        }
        else if (callbackDto.Status == "Failed" || callbackDto.Status == "Cancelled")
        {
            // Update order payment status to failed
            if (payment.Order != null)
            {
                payment.Order.PaymentStatus = "Failed";
                payment.Order.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<string> GeneratePaymentReferenceAsync()
    {
        string reference;
        do
        {
            reference = $"PAY{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
        } while (await _context.Payments.AnyAsync(p => p.PaymentReference == reference));

        return reference;
    }

    public async Task<PaymentGatewayRequestDto> PreparePaymentRequestAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {orderId} not found");
        }

        var payment = await GetPaymentByOrderIdAsync(orderId);
        if (payment == null)
        {
            throw new InvalidOperationException($"No payment found for order {orderId}");
        }

        return new PaymentGatewayRequestDto
        {
            PaymentReference = payment.PaymentReference,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CustomerName = $"{order.FirstName} {order.LastName}",
            CustomerEmail = order.Email,
            CustomerPhone = order.Phone,
            ReturnUrl = $"/checkout/payment-success?ref={payment.PaymentReference}",
            CancelUrl = $"/checkout/payment-cancelled?ref={payment.PaymentReference}",
            CallbackUrl = $"/api/payment/callback"
        };
    }

    public async Task<bool> VerifyPaymentAsync(string paymentReference, decimal expectedAmount)
    {
        var payment = await GetPaymentByReferenceAsync(paymentReference);
        if (payment == null) return false;

        return payment.Status == "Completed" || payment.Status == "Paid" && 
               Math.Abs(payment.Amount - expectedAmount) < 0.01m; // Allow for small rounding differences
    }
}