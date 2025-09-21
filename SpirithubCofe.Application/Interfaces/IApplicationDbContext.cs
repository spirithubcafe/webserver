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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}