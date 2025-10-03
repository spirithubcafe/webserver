using Microsoft.AspNetCore.Mvc;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Application.DTOs;

namespace SpirithubCafe.Web.Controllers;

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
            
            // Get parameters from form data - SmartPay sends POST parameters as per Section 10
            foreach (var key in Request.Form.Keys)
            {
                parameters[key] = Request.Form[key].ToString();
            }

            _logger.LogInformation("SmartPay callback received with parameters: {Parameters}",
                string.Join(", ", parameters.Where(kv => kv.Key != "encResponse").Select(kv => $"{kv.Key}={kv.Value}")));

            // Validate required parameters as per Section 10: order_id and encResponse
            if (!parameters.ContainsKey("order_id") || !parameters.ContainsKey("encResponse"))
            {
                _logger.LogWarning("Invalid SmartPay callback - missing required parameters (order_id or encResponse)");
                return BadRequest("Invalid callback - missing required parameters");
            }

            // Validate callback format
            if (!await _gatewayService.ValidateCallbackAsync(parameters))
            {
                _logger.LogWarning("SmartPay callback validation failed for order {OrderId}", parameters.GetValueOrDefault("order_id"));
                return BadRequest("Invalid callback signature");
            }

            // Process callback according to SmartPay documentation
            var callbackDto = await _gatewayService.ProcessCallbackAsync(parameters);
            
            // Update payment status
            var payment = await _paymentService.UpdatePaymentStatusAsync(callbackDto.PaymentReference, callbackDto);

            _logger.LogInformation("SmartPay payment {PaymentReference} processed successfully with status {Status}", 
                callbackDto.PaymentReference, callbackDto.Status);

            return Ok("Callback processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SmartPay callback");
            return StatusCode(500, "An error occurred while processing callback");
        }
    }

    [HttpGet("callback/success")]
    [HttpPost("callback/success")]
    public async Task<IActionResult> PaymentSuccess()
    {
        try
        {
            var parameters = new Dictionary<string, string>();
            
            // Get parameters from query string or form data
            foreach (var key in Request.Query.Keys)
            {
                parameters[key] = Request.Query[key].ToString();
            }
            
            foreach (var key in Request.Form.Keys)
            {
                parameters[key] = Request.Form[key].ToString();
            }

            _logger.LogInformation("SmartPay success callback received with parameters: {Parameters}", 
                string.Join(", ", parameters.Where(kv => kv.Key != "encResponse").Select(kv => $"{kv.Key}={kv.Value}")));

            // Process the callback if we have encrypted response as per Section 10
            if (parameters.ContainsKey("encResponse") && parameters.ContainsKey("order_id"))
            {
                var callbackDto = await _gatewayService.ProcessCallbackAsync(parameters);
                await _paymentService.UpdatePaymentStatusAsync(callbackDto.PaymentReference, callbackDto);
                
                _logger.LogInformation("SmartPay success: Payment {OrderId} status updated to {Status}", 
                    parameters["order_id"], callbackDto.Status);
            }

            // Redirect to success page with parameters
            var queryString = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            return Redirect($"/checkout/payment-success?{queryString}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SmartPay success callback");
            return Redirect("/payment/cancel");
        }
    }

    [HttpGet("callback/cancel")]
    [HttpPost("callback/cancel")]
    public async Task<IActionResult> PaymentCancel()
    {
        try
        {
            var parameters = new Dictionary<string, string>();
            
            // Get parameters from query string or form data
            foreach (var key in Request.Query.Keys)
            {
                parameters[key] = Request.Query[key].ToString();
            }
            
            foreach (var key in Request.Form.Keys)
            {
                parameters[key] = Request.Form[key].ToString();
            }

            _logger.LogInformation("SmartPay cancel callback received with parameters: {Parameters}", 
                string.Join(", ", parameters.Where(kv => kv.Key != "encResponse").Select(kv => $"{kv.Key}={kv.Value}")));

            // Process the callback if we have encrypted response as per Section 10
            if (parameters.ContainsKey("encResponse") && parameters.ContainsKey("order_id"))
            {
                var callbackDto = await _gatewayService.ProcessCallbackAsync(parameters);
                await _paymentService.UpdatePaymentStatusAsync(callbackDto.PaymentReference, callbackDto);
                
                _logger.LogInformation("SmartPay cancel: Payment {OrderId} status updated to {Status}", 
                    parameters["order_id"], callbackDto.Status);
            }

            // Redirect to cancel page with parameters
            var queryString = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            return Redirect($"/payment/cancel?{queryString}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SmartPay cancel callback");
            return Redirect("/payment/cancel");
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