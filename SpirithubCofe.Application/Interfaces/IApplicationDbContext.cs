using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Application.Interfaces;

/// <summary>
/// Interface for application database context
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Slide> Slides { get; }
    DbSet<Setting> Settings { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<CategoryImage> CategoryImages { get; }
    DbSet<ProductReview> ProductReviews { get; }
    DbSet<FAQ> FAQs { get; }
    DbSet<FAQCategory> FAQCategories { get; }
    DbSet<FAQPage> FAQPages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}