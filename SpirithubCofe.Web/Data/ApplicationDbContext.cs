using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Application.Interfaces;

namespace SpirithubCofe.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Slide> Slides { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<CategoryImage> CategoryImages { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<FAQCategory> FAQCategories { get; set; }
    public DbSet<FAQPage> FAQPages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure Setting entity
        builder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(2000);
                
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.DataType)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.HasIndex(e => e.Key)
                .IsUnique();
        });
        
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

        // Configure Category entity
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.NameAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
                
            entity.Property(e => e.DescriptionAr)
                .HasMaxLength(1000);
                
            entity.Property(e => e.ImagePath)
                .HasMaxLength(500);
                
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Slug");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");
                
            entity.HasIndex(e => e.IsDisplayedOnHomepage)
                .HasDatabaseName("IX_Categories_IsDisplayedOnHomepage");
                
            entity.HasIndex(e => e.DisplayOrder)
                .HasDatabaseName("IX_Categories_DisplayOrder");
        });

        // Configure Product entity
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.NameAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.Description)
                .HasMaxLength(2000);
                
            entity.Property(e => e.DescriptionAr)
                .HasMaxLength(2000);
                
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.NotesAr)
                .HasMaxLength(1000);
                
            entity.Property(e => e.AromaticProfile)
                .HasMaxLength(500);
                
            entity.Property(e => e.AromaticProfileAr)
                .HasMaxLength(500);
                
            entity.Property(e => e.Compatibility)
                .HasMaxLength(500);
                
            entity.Property(e => e.CompatibilityAr)
                .HasMaxLength(500);
                
            entity.Property(e => e.Uses)
                .HasMaxLength(1000);
                
            entity.Property(e => e.UsesAr)
                .HasMaxLength(1000);
                
            // Coffee Information Properties
            entity.Property(e => e.RoastLevel)
                .HasMaxLength(100);
                
            entity.Property(e => e.RoastLevelAr)
                .HasMaxLength(100);
                
            entity.Property(e => e.Process)
                .HasMaxLength(100);
                
            entity.Property(e => e.ProcessAr)
                .HasMaxLength(100);
                
            entity.Property(e => e.Variety)
                .HasMaxLength(200);
                
            entity.Property(e => e.VarietyAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.Farm)
                .HasMaxLength(200);
                
            entity.Property(e => e.FarmAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.TastingNotes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.TastingNotesAr)
                .HasMaxLength(1000);
                
            entity.Property(e => e.BrewingInstructions)
                .HasMaxLength(2000);
                
            entity.Property(e => e.BrewingInstructionsAr)
                .HasMaxLength(2000);
                
            // SEO and Additional Properties
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(200);
                
            entity.Property(e => e.MetaDescription)
                .HasMaxLength(500);
                
            entity.Property(e => e.MetaKeywords)
                .HasMaxLength(500);
                
            entity.Property(e => e.Tags)
                .HasMaxLength(1000);
                
            entity.Property(e => e.Slug)
                .HasMaxLength(200);
                
            entity.Property(e => e.ImageAlt)
                .HasMaxLength(200);
                
            entity.Property(e => e.ImageAltAr)
                .HasMaxLength(200);
                
            entity.HasIndex(e => e.Sku)
                .IsUnique()
                .HasDatabaseName("IX_Products_Sku");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Products_IsActive");
                
            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");
                
            entity.HasOne(e => e.Category)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.MainImage)
                .WithOne()
                .HasForeignKey<Product>("MainImageId")
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure ProductVariant entity
        builder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.VariantSku)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(10,3)");
                
            entity.Property(e => e.WeightUnit)
                .IsRequired()
                .HasMaxLength(10);
                
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,3)");
                
            entity.Property(e => e.DiscountPrice)
                .HasColumnType("decimal(10,3)");
                
            entity.Property(e => e.Length)
                .HasColumnType("decimal(10,2)");
                
            entity.Property(e => e.Width)
                .HasColumnType("decimal(10,2)");
                
            entity.Property(e => e.Height)
                .HasColumnType("decimal(10,2)");
                
            entity.HasIndex(e => e.VariantSku)
                .IsUnique()
                .HasDatabaseName("IX_ProductVariants_VariantSku");
                
            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_ProductVariants_ProductId");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_ProductVariants_IsActive");
                
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductImage entity
        builder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.ImagePath)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.AltText)
                .HasMaxLength(200);
                
            entity.Property(e => e.AltTextAr)
                .HasMaxLength(200);
                
            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_ProductImages_ProductId");
                
            entity.HasIndex(e => e.IsMain)
                .HasDatabaseName("IX_ProductImages_IsMain");
                
            entity.HasOne(e => e.Product)
                .WithMany(e => e.GalleryImages)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CategoryImage entity
        builder.Entity<CategoryImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.ImagePath)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.AltText)
                .HasMaxLength(200);
                
            entity.Property(e => e.AltTextAr)
                .HasMaxLength(200);
                
            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_CategoryImages_CategoryId");
                
            entity.HasOne(e => e.Category)
                .WithOne()
                .HasForeignKey<CategoryImage>(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductReview entity
        builder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .HasMaxLength(200);
                
            entity.Property(e => e.TitleAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.Content)
                .HasMaxLength(2000);
                
            entity.Property(e => e.ContentAr)
                .HasMaxLength(2000);
                
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(256);
                
            entity.Property(e => e.AdminNotes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.ApprovedByUserId)
                .HasMaxLength(450);
                
            entity.Property(e => e.UserId)
                .HasMaxLength(450);
                
            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_ProductReviews_ProductId");
                
            entity.HasIndex(e => e.IsApproved)
                .HasDatabaseName("IX_ProductReviews_IsApproved");
                
            entity.HasIndex(e => e.Rating)
                .HasDatabaseName("IX_ProductReviews_Rating");
                
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure FAQ entities
        builder.Entity<FAQCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.NameEn)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.NameAr)
                .HasMaxLength(200);
                
            entity.HasIndex(e => e.Order)
                .HasDatabaseName("IX_FAQCategories_Order");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_FAQCategories_IsActive");
        });

        builder.Entity<FAQ>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.QuestionEn)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.QuestionAr)
                .HasMaxLength(500);
                
            entity.Property(e => e.AnswerEn)
                .IsRequired();
                
            entity.Property(e => e.AnswerAr);
                
            entity.HasIndex(e => e.Order)
                .HasDatabaseName("IX_FAQs_Order");
                
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_FAQs_IsActive");
                
            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_FAQs_CategoryId");
                
            entity.HasOne(e => e.Category)
                .WithMany(e => e.FAQs)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<FAQPage>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TitleEn)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.TitleAr)
                .HasMaxLength(200);
                
            entity.Property(e => e.MetaTitleEn)
                .HasMaxLength(200);
                
            entity.Property(e => e.MetaTitleAr)
                .HasMaxLength(200);
        });

        // FAQ Category configuration
        builder.Entity<FAQCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            entity.HasIndex(e => e.Order);
        });

        // FAQ configuration
        builder.Entity<FAQ>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionEn).IsRequired().HasMaxLength(500);
            entity.Property(e => e.QuestionAr).HasMaxLength(500);
            entity.Property(e => e.AnswerEn).IsRequired();
            entity.Property(e => e.AnswerAr);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            entity.HasIndex(e => e.Order);
            entity.HasIndex(e => new { e.CategoryId, e.Order });
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.FAQs)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FAQ Page configuration
        builder.Entity<FAQPage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(300);
            entity.Property(e => e.TitleAr).HasMaxLength(300);
            entity.Property(e => e.DescriptionEn);
            entity.Property(e => e.DescriptionAr);
            entity.Property(e => e.MetaTitleEn).HasMaxLength(300);
            entity.Property(e => e.MetaTitleAr).HasMaxLength(300);
            entity.Property(e => e.MetaDescriptionEn).HasMaxLength(500);
            entity.Property(e => e.MetaDescriptionAr).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("datetime('now')");
        });
    }
}
