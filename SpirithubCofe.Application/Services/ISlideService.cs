using SpirithubCofe.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace SpirithubCofe.Application.Services;

/// <summary>
/// Service interface for managing slides
/// </summary>
public interface ISlideService
{
    /// <summary>
    /// Get all slides ordered by Order property
    /// </summary>
    Task<SlidesResult> GetAllSlidesAsync();
    
    /// <summary>
    /// Get only active slides ordered by Order property
    /// </summary>
    Task<SlidesResult> GetActiveSlidesAsync();
    
    /// <summary>
    /// Get a slide by ID
    /// </summary>
    Task<SlideResult> GetSlideByIdAsync(int id);
    
    /// <summary>
    /// Create a new slide
    /// </summary>
    Task<SlideResult> CreateSlideAsync(CreateSlideDto createSlideDto);
    
    /// <summary>
    /// Update an existing slide
    /// </summary>
    Task<SlideResult> UpdateSlideAsync(UpdateSlideDto updateSlideDto);
    
    /// <summary>
    /// Delete a slide by ID
    /// </summary>
    Task<bool> DeleteSlideAsync(int id);
    
    /// <summary>
    /// Toggle slide active status
    /// </summary>
    Task<SlideResult> ToggleSlideStatusAsync(int id);
    
    /// <summary>
    /// Reorder slides
    /// </summary>
    Task<bool> ReorderSlidesAsync(Dictionary<int, int> slideOrders);
    
    /// <summary>
    /// Upload slide image and return the path
    /// </summary>
    Task<string> UploadSlideImageAsync(IFormFile file);
    
    /// <summary>
    /// Delete slide image from storage
    /// </summary>
    Task<bool> DeleteSlideImageAsync(string imagePath);
}