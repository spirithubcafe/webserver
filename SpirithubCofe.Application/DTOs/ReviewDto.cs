using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }
}

public class ReviewsResponse
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingBreakdown { get; set; } = new();
}

public class SubmitReviewRequest
{
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string CustomerEmail { get; set; } = "";

    [Required(ErrorMessage = "Review title is required")]
    [StringLength(200, ErrorMessage = "Title must be less than 200 characters")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Review content is required")]
    [StringLength(2000, ErrorMessage = "Content must be less than 2000 characters")]
    public string Content { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; } = 5;
}