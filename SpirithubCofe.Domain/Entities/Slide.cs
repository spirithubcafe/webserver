namespace SpirithubCofe.Domain.Entities;

/// <summary>
/// Represents a slide in the homepage slideshow
/// </summary>
public class Slide
{
    public int Id { get; set; }
    
    /// <summary>
    /// Main title of the slide
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Arabic translation of the title
    /// </summary>
    public string? TitleAr { get; set; }
    
    /// <summary>
    /// Subtitle or description
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;
    
    /// <summary>
    /// Arabic translation of the subtitle
    /// </summary>
    public string? SubtitleAr { get; set; }
    
    /// <summary>
    /// Path to the slide image relative to wwwroot
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Text for the call-to-action button
    /// </summary>
    public string ButtonText { get; set; } = string.Empty;
    
    /// <summary>
    /// Arabic translation of the button text
    /// </summary>
    public string? ButtonTextAr { get; set; }
    
    /// <summary>
    /// URL for the call-to-action button
    /// </summary>
    public string ButtonUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Display order of the slide (lower numbers appear first)
    /// </summary>
    public int Order { get; set; } = 0;
    
    /// <summary>
    /// Whether the slide is active and should be displayed
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date when the slide was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the slide was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Background color or CSS class for the slide
    /// </summary>
    public string? BackgroundColor { get; set; }
    
    /// <summary>
    /// Text color for the slide content
    /// </summary>
    public string? TextColor { get; set; } = "text-white";
}