using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Application.DTOs;

public class CheckoutDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? AddressLine2 { get; set; }

    [Required]
    public int CountryId { get; set; }

    [Required]
    public int CityId { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [Required]
    public int ShippingMethodId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class CheckoutSummaryDto
{
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CheckoutItemDto> Items { get; set; } = new();
}

public class CheckoutItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? VariantInfo { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ImagePath { get; set; }
}

public class ShippingMethodDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Cost { get; set; }
}

public class CountryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class CityDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int CountryId { get; set; }
}