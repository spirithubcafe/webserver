using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Privacy Policy page header and general settings
/// </summary>
public class PrivacyPolicyPage
{
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = "Privacy Policy";

    [StringLength(200)]
    public string TitleAr { get; set; } = "سياسة الخصوصية";

    [StringLength(500)]
    public string? Subtitle { get; set; } = "Your privacy is important to us";

    [StringLength(500)]
    public string? SubtitleAr { get; set; } = "خصوصيتك مهمة بالنسبة لنا";

    [StringLength(50)]
    public string BgType { get; set; } = "color"; // color, image, video

    [StringLength(500)]
    public string BgValue { get; set; } = "#ffffff"; // color code, image path, or video path

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<PrivacyPolicySection> Sections { get; set; } = new List<PrivacyPolicySection>();
}
