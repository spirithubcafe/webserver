using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SpirithubCofe.Application.DTOs;
using SpirithubCofe.Application.Services;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Infrastructure.Services;

/// <summary>
/// Implementation of slide service
/// </summary>
public class SlideService : ISlideService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SlideService> _logger;
    private readonly string _slidesPath = "images/slides";
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public SlideService(IApplicationDbContext context, ILogger<SlideService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SlidesResult> GetAllSlidesAsync()
    {
        try
        {
            var slides = await _context.Slides
                .OrderBy(s => s.Order)
                .ThenBy(s => s.Id)
                .Select(s => MapToDto(s))
                .ToListAsync();

            return SlidesResult.SuccessResult(slides);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all slides");
            return SlidesResult.FailureResult("Error getting slides");
        }
    }

    public async Task<SlidesResult> GetActiveSlidesAsync()
    {
        try
        {
            var slides = await _context.Slides
                .Where(s => s.IsActive)
                .OrderBy(s => s.Order)
                .ThenBy(s => s.Id)
                .Select(s => MapToDto(s))
                .ToListAsync();

            return SlidesResult.SuccessResult(slides);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active slides");
            return SlidesResult.FailureResult("Error getting active slides");
        }
    }

    public async Task<SlideResult> GetSlideByIdAsync(int id)
    {
        try
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null)
            {
                return SlideResult.FailureResult("Slide not found");
            }

            return SlideResult.SuccessResult(MapToDto(slide));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slide by id {Id}", id);
            return SlideResult.FailureResult("Error getting slide");
        }
    }

    public async Task<SlideResult> CreateSlideAsync(CreateSlideDto createSlideDto)
    {
        try
        {
            var slide = new Slide
            {
                Title = createSlideDto.Title,
                TitleAr = createSlideDto.TitleAr,
                Subtitle = createSlideDto.Subtitle,
                SubtitleAr = createSlideDto.SubtitleAr,
                ImagePath = createSlideDto.ImagePath,
                ButtonText = createSlideDto.ButtonText,
                ButtonTextAr = createSlideDto.ButtonTextAr,
                ButtonUrl = createSlideDto.ButtonUrl,
                Order = createSlideDto.Order,
                IsActive = createSlideDto.IsActive,
                BackgroundColor = createSlideDto.BackgroundColor,
                TextColor = createSlideDto.TextColor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Slides.Add(slide);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new slide with id {Id}", slide.Id);
            return SlideResult.SuccessResult(MapToDto(slide), "Slide created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slide");
            return SlideResult.FailureResult("Error creating slide");
        }
    }

    public async Task<SlideResult> UpdateSlideAsync(UpdateSlideDto updateSlideDto)
    {
        try
        {
            var slide = await _context.Slides.FindAsync(updateSlideDto.Id);
            if (slide == null)
            {
                return SlideResult.FailureResult("Slide not found");
            }

            slide.Title = updateSlideDto.Title;
            slide.TitleAr = updateSlideDto.TitleAr;
            slide.Subtitle = updateSlideDto.Subtitle;
            slide.SubtitleAr = updateSlideDto.SubtitleAr;
            slide.ImagePath = updateSlideDto.ImagePath;
            slide.ButtonText = updateSlideDto.ButtonText;
            slide.ButtonTextAr = updateSlideDto.ButtonTextAr;
            slide.ButtonUrl = updateSlideDto.ButtonUrl;
            slide.Order = updateSlideDto.Order;
            slide.IsActive = updateSlideDto.IsActive;
            slide.BackgroundColor = updateSlideDto.BackgroundColor;
            slide.TextColor = updateSlideDto.TextColor;
            slide.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated slide with id {Id}", slide.Id);
            return SlideResult.SuccessResult(MapToDto(slide), "Slide updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating slide with id {Id}", updateSlideDto.Id);
            return SlideResult.FailureResult("Error updating slide");
        }
    }

    public async Task<bool> DeleteSlideAsync(int id)
    {
        try
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null)
            {
                return false;
            }

            // Delete image file if exists
            if (!string.IsNullOrEmpty(slide.ImagePath))
            {
                await DeleteSlideImageAsync(slide.ImagePath);
            }

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted slide with id {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting slide with id {Id}", id);
            return false;
        }
    }

    public async Task<SlideResult> ToggleSlideStatusAsync(int id)
    {
        try
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null)
            {
                return SlideResult.FailureResult("Slide not found");
            }

            slide.IsActive = !slide.IsActive;
            slide.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Toggled slide status for id {Id} to {Status}", id, slide.IsActive);
            return SlideResult.SuccessResult(MapToDto(slide), $"Slide status changed to {(slide.IsActive ? "active" : "inactive")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling slide status for id {Id}", id);
            return SlideResult.FailureResult("Error changing slide status");
        }
    }

    public async Task<bool> ReorderSlidesAsync(Dictionary<int, int> slideOrders)
    {
        try
        {
            foreach (var (slideId, newOrder) in slideOrders)
            {
                var slide = await _context.Slides.FindAsync(slideId);
                if (slide != null)
                {
                    slide.Order = newOrder;
                    slide.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Reordered {Count} slides", slideOrders.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering slides");
            return false;
        }
    }

    public async Task<string> UploadSlideImageAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file selected");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file format. Only JPG, PNG, WEBP are allowed");
            }

            if (file.Length > _maxFileSize)
            {
                throw new ArgumentException("File size cannot exceed 5 MB");
            }

            var uploadsPath = Path.Combine("wwwroot", _slidesPath);
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/{_slidesPath}/{fileName}";
            _logger.LogInformation("Uploaded slide image to {Path}", relativePath);
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading slide image");
            throw;
        }
    }

    public async Task<bool> DeleteSlideImageAsync(string imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return true;

            var fullPath = Path.Combine("wwwroot", imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Deleted slide image at {Path}", imagePath);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting slide image at {Path}", imagePath);
            return false;
        }
    }

    private static SlideDto MapToDto(Slide slide)
    {
        return new SlideDto
        {
            Id = slide.Id,
            Title = slide.Title,
            TitleAr = slide.TitleAr,
            Subtitle = slide.Subtitle,
            SubtitleAr = slide.SubtitleAr,
            ImagePath = slide.ImagePath,
            ButtonText = slide.ButtonText,
            ButtonTextAr = slide.ButtonTextAr,
            ButtonUrl = slide.ButtonUrl,
            Order = slide.Order,
            IsActive = slide.IsActive,
            CreatedAt = slide.CreatedAt,
            UpdatedAt = slide.UpdatedAt,
            BackgroundColor = slide.BackgroundColor,
            TextColor = slide.TextColor
        };
    }
}