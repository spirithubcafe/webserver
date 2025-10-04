using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// About Us page header and general settings
/// </summary>
public class AboutUsPage
{
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = "About Us";

    [StringLength(200)]
    public string TitleAr { get; set; } = "من نحن";

    [StringLength(500)]
    public string? Subtitle { get; set; } = "Learn more about our story";

    [StringLength(500)]
    public string? SubtitleAr { get; set; } = "تعرف على قصتنا";

    [StringLength(50)]
    public string BgType { get; set; } = "color"; // color, image, video

    [StringLength(500)]
    public string BgValue { get; set; } = "#ffffff"; // color code, image path, or video path

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<AboutUsSection> Sections { get; set; } = new List<AboutUsSection>();
}