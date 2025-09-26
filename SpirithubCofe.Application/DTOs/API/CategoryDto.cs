using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Application.DTOs.API;

/// <summary>
/// Category DTO for API responses
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique slug for the category (used in URLs)
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Category description in English
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category description in Arabic
    /// </summary>
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// Full URL to the category image
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Whether the category is active and visible
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the category should be displayed on the homepage
    /// </summary>
    public bool IsDisplayedOnHomepage { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Number of active products in this category
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create category request DTO
/// </summary>
public class CreateCategoryRequestDto
{
    /// <summary>
    /// Unique slug for the category (used in URLs)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category name in English
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category name in Arabic
    /// </summary>
    [StringLength(200)]
    public string? NameAr { get; set; }

    /// <summary>
    /// Category description in English
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Category description in Arabic
    /// </summary>
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// Whether the category is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the category should be displayed on the homepage
    /// </summary>
    public bool IsDisplayedOnHomepage { get; set; } = true;

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Update category request DTO
/// </summary>
public class UpdateCategoryRequestDto
{
    /// <summary>
    /// Unique slug for the category (used in URLs)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category name in English
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category name in Arabic
    /// </summary>
    [StringLength(200)]
    public string? NameAr { get; set; }

    /// <summary>
    /// Category description in English
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Category description in Arabic
    /// </summary>
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// Whether the category is active and visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the category should be displayed on the homepage
    /// </summary>
    public bool IsDisplayedOnHomepage { get; set; } = true;

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Category summary DTO for listing
/// </summary>
public class CategorySummaryDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique slug for the category (used in URLs)
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category name in English
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category name in Arabic
    /// </summary>
    public string? NameAr { get; set; }

    /// <summary>
    /// Full URL to the category image
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Number of active products in this category
    /// </summary>
    public int ProductCount { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }
}