using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Refund Policy page header and general settings
/// </summary>
public class RefundPolicyPage
{
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = "Refund Policy";

    [StringLength(200)]
    public string TitleAr { get; set; } = "سياسة الاسترجاع";

    [StringLength(500)]
    public string? Subtitle { get; set; } = "Learn about our refund procedures";

    [StringLength(500)]
    public string? SubtitleAr { get; set; } = "تعرف على إجراءات الاسترجاع لدينا";

    [StringLength(50)]
    public string BgType { get; set; } = "color"; // color, image, video

    [StringLength(500)]
    public string BgValue { get; set; } = "#ffffff"; // color code, image path, or video path

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<RefundPolicySection> Sections { get; set; } = new List<RefundPolicySection>();
}
