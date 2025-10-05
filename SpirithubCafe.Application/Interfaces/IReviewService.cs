using SpirithubCafe.Application.DTOs;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewsResponse> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10);
    Task<bool> SubmitReviewAsync(int productId, SubmitReviewRequest request);
    Task<List<ProductReview>> GetAllReviewsAsync();
    Task<bool> ApproveReviewAsync(int reviewId);
    Task<bool> RejectReviewAsync(int reviewId);
    Task<bool> DeleteReviewAsync(int reviewId);
}
