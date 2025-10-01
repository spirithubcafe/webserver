namespace SpirithubCofe.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "OMR";
    public string Status { get; set; } = "Pending";
    public string? PaymentMethod { get; set; }
    public string Gateway { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreatePaymentDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "OMR";
    public string Gateway { get; set; } = string.Empty;
}

public class PaymentCallbackDto
{
    public string PaymentReference { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public string? GatewayResponse { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentGatewayRequestDto
{
    public string PaymentReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "OMR";
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

public class PaymentInitiationResponse
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
}