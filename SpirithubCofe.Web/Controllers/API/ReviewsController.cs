using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Web.Data;
using SpirithubCofe.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Web.Controllers.API;

/// <summary>
/// API Controller for managing product reviews
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get approved reviews for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="page">Page number (starts from 1)</param>
    /// <param name="pageSize">Items per page (max 50)</param>
    /// <returns>Paginated list of approved reviews</returns>
    [HttpGet("product/{productId:int}")]
    [ProducesResponseType(typeof(ReviewsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ReviewsResponse>> GetProductReviews(
        int productId,
        [FromQuery][Range(1, int.MaxValue)] int page = 1,
        [FromQuery][Range(1, 50)] int pageSize = 10)
    {
        // Check if product exists
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            return NotFound(new { Message = "Product not found" });
        }

        var totalReviews = await _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .CountAsync();

        var reviews = await _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Title = r.Title,
                TitleAr = r.TitleAr,
                Content = r.Content,
                ContentAr = r.ContentAr,
                CustomerName = r.CustomerName,
                CreatedAt = r.CreatedAt,
                IsFeatured = r.IsFeatured
            })
            .ToListAsync();

        // Calculate rating statistics
        var ratingStats = await _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .GroupBy(r => r.Rating)
            .Select(g => new RatingStatDto
            {
                Rating = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var averageRating = ratingStats.Any() 
            ? Math.Round(ratingStats.Sum(r => r.Rating * r.Count) / (double)totalReviews, 1)
            : 0;

        var response = new ReviewsResponse
        {
            Reviews = reviews,
            TotalReviews = totalReviews,
            AverageRating = averageRating,
            RatingStats = ratingStats,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalReviews / (double)pageSize)
        };

        return Ok(response);
    }

    /// <summary>
    /// Submit a new review for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="reviewRequest">Review details</param>
    /// <returns>Success message</returns>
    [HttpPost("product/{productId:int}")]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> SubmitReview(int productId, [FromBody] SubmitReviewRequest reviewRequest)
    {
        // Validate product exists
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            return NotFound(new { Message = "Product not found" });
        }

        // Check if user has already reviewed this product (by email)
        var existingReview = await _context.ProductReviews
            .AnyAsync(r => r.ProductId == productId && r.CustomerEmail == reviewRequest.CustomerEmail);

        if (existingReview)
        {
            return BadRequest(new { Message = "You have already reviewed this product" });
        }

        var review = new ProductReview
        {
            ProductId = productId,
            Rating = reviewRequest.Rating,
            Title = reviewRequest.Title,
            TitleAr = reviewRequest.TitleAr,
            Content = reviewRequest.Content,
            ContentAr = reviewRequest.ContentAr,
            CustomerName = reviewRequest.CustomerName,
            CustomerEmail = reviewRequest.CustomerEmail,
            IsApproved = false, // Requires admin approval
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProductReviews), 
            new { productId }, 
            new { Message = "Review submitted successfully. It will be published after admin approval." }
        );
    }
}

/// <summary>
/// DTO for review data
/// </summary>
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? TitleAr { get; set; }
    public string? Content { get; set; }
    public string? ContentAr { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsFeatured { get; set; }
}

/// <summary>
/// DTO for rating statistics
/// </summary>
public class RatingStatDto
{
    public int Rating { get; set; }
    public int Count { get; set; }
}

/// <summary>
/// Response for reviews endpoint
/// </summary>
public class ReviewsResponse
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public List<RatingStatDto> RatingStats { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Request model for submitting a review
/// </summary>
public class SubmitReviewRequest
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(200)]
    public string? TitleAr { get; set; }

    [MaxLength(2000)]
    public string? Content { get; set; }

    [MaxLength(2000)]
    public string? ContentAr { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;
}