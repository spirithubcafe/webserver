using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Aramex shipping system settings
/// </summary>
public class AramexSettings
{
    public int Id { get; set; }

    // API Credentials
    /// <summary>
    /// Enable test mode to use Aramex sandbox environment
    /// </summary>
    public bool TestMode { get; set; } = false;

    /// <summary>
    /// Aramex API username
    /// </summary>
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Aramex API password
    /// </summary>
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Aramex account number
    /// </summary>
    [MaxLength(100)]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Aramex account PIN
    /// </summary>
    [MaxLength(100)]
    public string AccountPin { get; set; } = string.Empty;

    /// <summary>
    /// Aramex account entity (e.g., AMM, MCT)
    /// </summary>
    [MaxLength(10)]
    public string AccountEntity { get; set; } = string.Empty;

    /// <summary>
    /// Aramex account country code (e.g., OM)
    /// </summary>
    [MaxLength(5)]
    public string AccountCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Aramex API version (default: v1.0)
    /// </summary>
    [MaxLength(10)]
    public string ApiVersion { get; set; } = "v1.0";

    /// <summary>
    /// Aramex source identifier (default: 24)
    /// </summary>
    [MaxLength(10)]
    public string Source { get; set; } = "24";

    // Shipper Information
    /// <summary>
    /// Company name for shipping labels
    /// </summary>
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Contact person name
    /// </summary>
    [MaxLength(255)]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number
    /// </summary>
    [MaxLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// First line of shipping address
    /// </summary>
    [MaxLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Second line of shipping address (optional)
    /// </summary>
    [MaxLength(255)]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State or province
    /// </summary>
    [MaxLength(100)]
    public string? StateProvince { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code (e.g., OM for Oman)
    /// </summary>
    [MaxLength(5)]
    public string CountryCode { get; set; } = string.Empty;

    // Services Configuration
    /// <summary>
    /// Domestic services enabled (comma-separated: OND,CDS)
    /// </summary>
    public string DomesticServices { get; set; } = string.Empty;

    /// <summary>
    /// International services enabled (comma-separated: EPX,PPX,GRD)
    /// </summary>
    public string InternationalServices { get; set; } = string.Empty;

    // Service Labels
    /// <summary>
    /// Custom label for OND service
    /// </summary>
    [MaxLength(255)]
    public string OndLabel { get; set; } = "Aramex Domestic (OND)";

    /// <summary>
    /// Custom label for CDS service
    /// </summary>
    [MaxLength(255)]
    public string CdsLabel { get; set; } = "Cash on Delivery (CDS)";

    /// <summary>
    /// Custom label for EPX service
    /// </summary>
    [MaxLength(255)]
    public string EpxLabel { get; set; } = "Aramex Express (EPX)";

    /// <summary>
    /// Custom label for PPX service
    /// </summary>
    [MaxLength(255)]
    public string PpxLabel { get; set; } = "Aramex Priority (PPX)";

    /// <summary>
    /// Custom label for GRD service
    /// </summary>
    [MaxLength(255)]
    public string GrdLabel { get; set; } = "Aramex Ground (GRD)";

    // Arabic Labels
    /// <summary>
    /// Custom label for OND service in Arabic
    /// </summary>
    [MaxLength(255)]
    public string? OndLabelAr { get; set; }

    /// <summary>
    /// Custom label for CDS service in Arabic
    /// </summary>
    [MaxLength(255)]
    public string? CdsLabelAr { get; set; }

    /// <summary>
    /// Custom label for EPX service in Arabic
    /// </summary>
    [MaxLength(255)]
    public string? EpxLabelAr { get; set; }

    /// <summary>
    /// Custom label for PPX service in Arabic
    /// </summary>
    [MaxLength(255)]
    public string? PpxLabelAr { get; set; }

    /// <summary>
    /// Custom label for GRD service in Arabic
    /// </summary>
    [MaxLength(255)]
    public string? GrdLabelAr { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}