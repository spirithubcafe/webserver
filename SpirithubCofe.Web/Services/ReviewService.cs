using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.DTOs;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

public class ReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewsResponse> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10)
    {
        var query = _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                CustomerName = r.CustomerName,
                Title = r.Title ?? "",
                Content = r.Content ?? "",
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                IsApproved = r.IsApproved
            })
            .ToListAsync();

        // Calculate average rating and breakdown
        var allReviews = await _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .ToListAsync();

        var averageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;
        var ratingBreakdown = allReviews
            .GroupBy(r => r.Rating)
            .ToDictionary(g => g.Key, g => g.Count());

        return new ReviewsResponse
        {
            Reviews = reviews,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            AverageRating = averageRating,
            RatingBreakdown = ratingBreakdown
        };
    }

    public async Task<bool> SubmitReviewAsync(int productId, SubmitReviewRequest request)
    {
        try
        {
            var review = new ProductReview
            {
                ProductId = productId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Title = request.Title,
                Content = request.Content,
                Rating = request.Rating,
                CreatedAt = DateTime.Now,
                IsApproved = false // Requires admin approval
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}