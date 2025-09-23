using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service to seed sample data for categories and products
/// </summary>
public class DataSeederService
{
    private readonly ApplicationDbContext _context;

    public DataSeederService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed sample categories and products with bilingual content
    /// </summary>
    public async Task SeedSampleDataAsync()
    {
        // Check if we already have data
        if (await _context.Categories.AnyAsync())
        {
            return; // Data already exists
        }

        // Create Categories
        var categories = new List<Category>
        {
            new Category
            {
                Slug = "espresso-milk-based-coffee",
                Name = "Espresso & Milk-Based Coffee",
                NameAr = "قهوة الإسبريسو و الحليب",
                Description = "Espresso beans are coffee beans roasted specifically to suit the unique requirements of brewing espresso.",
                DescriptionAr = "حبوب الإسبريسو هي حبوب قهوة محمصة خصيصًا لتناسب المتطلبات الفريدة لتحضير الإسبريسو.",
                ImagePath = "/images/categories/specialty-coffee-beans-roastery-oman-spirithub-espresso-coffee.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 1
            },
            new Category
            {
                Slug = "filter-pour-over-coffee",
                Name = "Filter & Pour-Over Coffee",
                NameAr = "القهوة المقطرة بالترشيح",
                Description = "Filter coffee, also known as drip coffee, is a method of brewing coffee where hot water is poured over ground coffee.",
                DescriptionAr = "القهوة المقطرة، والمعروفة أيضاً بالقهوة المنقطة، هي طريقة لتحضير القهوة حيث يتم سكب الماء الساخن على القهوة المطحونة.",
                ImagePath = "/images/categories/specialty-coffee-beans-roastery-oman-spirithub-filter-coffee.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 2
            },
            new Category
            {
                Slug = "ufo-drip-coffee-filters",
                Name = "UFO Drip Coffee Filters",
                NameAr = "فلاتر قهوة UFO التنقيط",
                Description = "Single-serve UFO drip coffee filters for convenient brewing.",
                DescriptionAr = "فلاتر قهوة UFO التنقيط لتحضير مريح للقهوة الفردية.",
                ImagePath = "/images/categories/ufo-drip-coffee-filters.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 3
            },
            new Category
            {
                Slug = "specialty-coffee-capsules",
                Name = "SpiritHub Coffee Capsules",
                NameAr = "مجموعة كبسولات القهوة",
                Description = "Indulge in a classic espresso experience with our wide selection of capsules each offering its own unique flavor profile and rich aroma.",
                DescriptionAr = "استمتع بتجربة الإسبريسو الكلاسيكية مع مجموعة واسعة من الكبسولات، حيث يتميز كل نوع بطابعه الخاص ونكهته العطرية الفريدة.",
                ImagePath = "/images/categories/specialty-coffee-capsules.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 4
            },
            new Category
            {
                Slug = "competition-premium-series",
                Name = "Competition Premium Series",
                NameAr = "مجموعة المنافسة المميزة",
                Description = "Our premium collection of competition-grade coffees for serious coffee enthusiasts.",
                DescriptionAr = "مجموعتنا الفاخرة من قهوات درجة المنافسة لعشاق القهوة الجادين.",
                ImagePath = "/images/categories/competition-premium-series.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 5
            },
            new Category
            {
                Slug = "merchandise",
                Name = "SpiritHub Merchandise",
                NameAr = "منتجات سبيريت هب",
                Description = "Official SpiritHub merchandise and accessories.",
                DescriptionAr = "المنتجات والإكسسوارات الرسمية لسبيريت هب.",
                ImagePath = "/images/categories/merchandise.webp",
                IsActive = true,
                IsDisplayedOnHomepage = false,
                DisplayOrder = 6
            }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();

 
 
    }
}