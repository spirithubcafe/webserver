using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Terms & Conditions page header and general settings
/// </summary>
public class TermsConditionsPage
{
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = "Terms & Conditions";

    [StringLength(200)]
    public string TitleAr { get; set; } = "الشروط والأحكام";

    [StringLength(500)]
    public string? Subtitle { get; set; } = "Please read our terms carefully";

    [StringLength(500)]
    public string? SubtitleAr { get; set; } = "يرجى قراءة شروطنا بعناية";

    [StringLength(50)]
    public string BgType { get; set; } = "color"; // color, image, video

    [StringLength(500)]
    public string BgValue { get; set; } = "#ffffff"; // color code, image path, or video path

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<TermsConditionsSection> Sections { get; set; } = new List<TermsConditionsSection>();
}
