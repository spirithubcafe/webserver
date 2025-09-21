using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.DTOs;

/// <summary>
/// DTO for creating a new slide
/// </summary>
public class CreateSlideDto
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    public string? SubtitleAr { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string? ButtonTextAr { get; set; }
    public string ButtonUrl { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; } = "text-white";
}

/// <summary>
/// DTO for updating an existing slide
/// </summary>
public class UpdateSlideDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    public string? SubtitleAr { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string? ButtonTextAr { get; set; }
    public string ButtonUrl { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; } = "text-white";
}

/// <summary>
/// DTO for slide display
/// </summary>
public class SlideDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    public string? SubtitleAr { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string? ButtonTextAr { get; set; }
    public string ButtonUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    
    /// <summary>
    /// Gets localized title based on current culture
    /// </summary>
    public string GetLocalizedTitle(string culture) => 
        culture.StartsWith("ar") && !string.IsNullOrEmpty(TitleAr) ? TitleAr : Title;
    
    /// <summary>
    /// Gets localized subtitle based on current culture
    /// </summary>
    public string GetLocalizedSubtitle(string culture) => 
        culture.StartsWith("ar") && !string.IsNullOrEmpty(SubtitleAr) ? SubtitleAr : Subtitle;
    
    /// <summary>
    /// Gets localized button text based on current culture
    /// </summary>
    public string GetLocalizedButtonText(string culture) => 
        culture.StartsWith("ar") && !string.IsNullOrEmpty(ButtonTextAr) ? ButtonTextAr : ButtonText;
}

/// <summary>
/// Result wrapper for slide operations
/// </summary>
public class SlideResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public SlideDto? Slide { get; set; }
    
    public static SlideResult SuccessResult(SlideDto slide, string message = "") => 
        new() { Success = true, Slide = slide, Message = message };
    
    public static SlideResult FailureResult(string message) => 
        new() { Success = false, Message = message };
}

/// <summary>
/// Result wrapper for multiple slides operations
/// </summary>
public class SlidesResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<SlideDto> Slides { get; set; } = Enumerable.Empty<SlideDto>();
    
    public static SlidesResult SuccessResult(IEnumerable<SlideDto> slides, string message = "") => 
        new() { Success = true, Slides = slides, Message = message };
    
    public static SlidesResult FailureResult(string message) => 
        new() { Success = false, Message = message };
}