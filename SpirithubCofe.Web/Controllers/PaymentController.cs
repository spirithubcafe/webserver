using Microsoft.AspNetCore.Mvc;
using SpirithubCofe.Application.Services;
using SpirithubCofe.Application.DTOs;

namespace SpirithubCofe.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentGatewayService _gatewayService;
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService,
        IPaymentGatewayService gatewayService,
        ICheckoutService checkoutService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _gatewayService = gatewayService;
        _checkoutService = checkoutService;
        _logger = logger;
    }

    [HttpGet("initiate/{orderId:int}")]
    public async Task<IActionResult> InitiatePayment(int orderId)
    {
        try
        {
            var order = await _checkoutService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            if (order.PaymentStatus == "Paid")
            {
                return BadRequest("Order is already paid");
            }

            // Create or get existing payment
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment == null)
            {
                var createPaymentDto = new CreatePaymentDto
                {
                    OrderId = orderId,
                    Amount = order.TotalAmount,
                    Currency = "OMR",
                    Gateway = "Bank Muscat"
                };
                payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
            }

            // Prepare payment request
            var paymentRequest = await _paymentService.PreparePaymentRequestAsync(orderId);
            
            // Generate payment URL
            var paymentUrl = await _gatewayService.GeneratePaymentUrlAsync(paymentRequest);

            return Ok(new { paymentUrl, paymentReference = payment.PaymentReference });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment for order {OrderId}", orderId);
            return StatusCode(500, "An error occurred while initiating payment");
        }
    }

    [HttpPost("callback")]
    public async Task<IActionResult> PaymentCallback()
    {
        try
        {
            var parameters = new Dictionary<string, string>();
            
            // Get parameters from form data
            foreach (var key in Request.Form.Keys)
            {
                parameters[key] = Request.Form[key].ToString();
            }

            // Validate callback
            if (!await _gatewayService.ValidateCallbackAsync(parameters))
            {
                _logger.LogWarning("Invalid payment callback received");
                return BadRequest("Invalid callback");
            }

            // Process callback
            var callbackDto = await _gatewayService.ProcessCallbackAsync(parameters);
            
            // Update payment status
            var payment = await _paymentService.UpdatePaymentStatusAsync(callbackDto.PaymentReference, callbackDto);

            _logger.LogInformation("Payment {PaymentReference} updated to status {Status}", 
                callbackDto.PaymentReference, callbackDto.Status);

            return Ok("Callback processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment callback");
            return StatusCode(500, "An error occurred while processing callback");
        }
    }

    [HttpGet("status/{paymentReference}")]
    public async Task<IActionResult> GetPaymentStatus(string paymentReference)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByReferenceAsync(paymentReference);
            if (payment == null)
            {
                return NotFound($"Payment with reference {paymentReference} not found");
            }

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                PaymentReference = payment.PaymentReference,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                Gateway = payment.Gateway,
                ErrorMessage = payment.ErrorMessage,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt
            };

            return Ok(paymentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status for {PaymentReference}", paymentReference);
            return StatusCode(500, "An error occurred while getting payment status");
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        try
        {
            var isValid = await _paymentService.VerifyPaymentAsync(request.PaymentReference, request.Amount);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment {PaymentReference}", request.PaymentReference);
            return StatusCode(500, "An error occurred while verifying payment");
        }
    }
}

public class VerifyPaymentRequest
{
    public string PaymentReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}