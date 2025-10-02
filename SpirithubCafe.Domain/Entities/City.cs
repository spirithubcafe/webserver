namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// City entry for shipping (bilingual, code, active flag)
/// </summary>
public class City
{
    public int Id { get; set; }

    /// <summary>
    /// Optional city code used by shipping provider
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Whether this city is available for shipping
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Foreign key to Country
    /// </summary>
    public int CountryId { get; set; }
    public virtual Country? Country { get; set; }
}
