namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Country entry for shipping (bilingual, code, active flag)
/// </summary>
public class Country
{
    public int Id { get; set; }

    /// <summary>
    /// ISO or custom code for Aramex shipping (e.g. AE, SA)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Whether this country is available for shipping
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Cities in this country
    /// </summary>
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
