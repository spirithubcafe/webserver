using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Application.Interfaces;

namespace SpirithubCofe.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Slide> Slides { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure Slide entity
        builder.Entity<Slide>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.TitleAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.Subtitle)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.SubtitleAr)
                .HasMaxLength(500);
                
            entity.Property(e => e.ImagePath)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.ButtonText)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.ButtonTextAr)
                .HasMaxLength(100);
                
            entity.Property(e => e.ButtonUrl)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.BackgroundColor)
                .HasMaxLength(50);
                
            entity.Property(e => e.TextColor)
                .HasMaxLength(50);
                
            entity.HasIndex(e => e.Order)
                .HasDatabaseName("IX_Slides_Order");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Slides_IsActive");
        });
    }
}
