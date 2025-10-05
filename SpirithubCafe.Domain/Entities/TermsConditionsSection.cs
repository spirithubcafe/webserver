using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities;

/// <summary>
/// Dynamic sections for the Terms & Conditions page
/// </summary>
public class TermsConditionsSection
{
    public int Id { get; set; }

    public int TermsConditionsPageId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = "";

    [StringLength(200)]
    public string TitleAr { get; set; } = "";

    public string? Content { get; set; } // Rich text content

    public string? ContentAr { get; set; } // Rich text content in Arabic

    [StringLength(500)]
    public string? ImagePath { get; set; }

    [StringLength(200)]
    public string? ImageAlt { get; set; }

    [StringLength(200)]
    public string? ImageAltAr { get; set; }

    /// <summary>
    /// Section layout type:
    /// - image-right-text-left: Image on right, text on left
    /// - image-left-text-right: Image on left, text on right  
    /// - text-only: Full width text content
    /// - image-only: Full width image
    /// </summary>
    [StringLength(50)]
    public string LayoutType { get; set; } = "image-right-text-left";

    [StringLength(50)]
    public string BgType { get; set; } = "color"; // color, image, video

    [StringLength(500)]
    public string BgValue { get; set; } = "#ffffff"; // color code, image path, or video path

    /// <summary>
    /// Display order for sorting sections
    /// </summary>
    public int DisplayOrder { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public TermsConditionsPage TermsConditionsPage { get; set; } = null!;
}
