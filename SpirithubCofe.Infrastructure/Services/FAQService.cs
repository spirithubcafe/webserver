using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Application.DTOs;
using SpirithubCofe.Application.Interfaces;
using SpirithubCofe.Application.Services;
using SpirithubCofe.Domain.Entities;

namespace SpirithubCofe.Infrastructure.Services;

public class FAQService : IFAQService
{
    private readonly IApplicationDbContext _context;

    public FAQService(IApplicationDbContext context)
    {
        _context = context;
    }

    // FAQ Management
    public async Task<IEnumerable<FAQDto>> GetAllFAQsAsync()
    {
        return await _context.FAQs
            .Include(f => f.Category)
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Id)
            .Select(f => new FAQDto
            {
                Id = f.Id,
                QuestionEn = f.QuestionEn,
                QuestionAr = f.QuestionAr,
                AnswerEn = f.AnswerEn,
                AnswerAr = f.AnswerAr,
                Order = f.Order,
                IsActive = f.IsActive,
                CategoryId = f.CategoryId,
                CategoryName = f.Category != null ? f.Category.NameEn : null,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<FAQDto>> GetActiveFAQsAsync()
    {
        return await _context.FAQs
            .Include(f => f.Category)
            .Where(f => f.IsActive)
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Id)
            .Select(f => new FAQDto
            {
                Id = f.Id,
                QuestionEn = f.QuestionEn,
                QuestionAr = f.QuestionAr,
                AnswerEn = f.AnswerEn,
                AnswerAr = f.AnswerAr,
                Order = f.Order,
                IsActive = f.IsActive,
                CategoryId = f.CategoryId,
                CategoryName = f.Category != null ? f.Category.NameEn : null,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<FAQDto>> GetFAQsByCategoryAsync(int categoryId)
    {
        return await _context.FAQs
            .Include(f => f.Category)
            .Where(f => f.CategoryId == categoryId && f.IsActive)
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Id)
            .Select(f => new FAQDto
            {
                Id = f.Id,
                QuestionEn = f.QuestionEn,
                QuestionAr = f.QuestionAr,
                AnswerEn = f.AnswerEn,
                AnswerAr = f.AnswerAr,
                Order = f.Order,
                IsActive = f.IsActive,
                CategoryId = f.CategoryId,
                CategoryName = f.Category != null ? f.Category.NameEn : null,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<FAQDto?> GetFAQByIdAsync(int id)
    {
        var faq = await _context.FAQs
            .Include(f => f.Category)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faq == null) return null;

        return new FAQDto
        {
            Id = faq.Id,
            QuestionEn = faq.QuestionEn,
            QuestionAr = faq.QuestionAr,
            AnswerEn = faq.AnswerEn,
            AnswerAr = faq.AnswerAr,
            Order = faq.Order,
            IsActive = faq.IsActive,
            CategoryId = faq.CategoryId,
            CategoryName = faq.Category?.NameEn,
            CreatedAt = faq.CreatedAt,
            UpdatedAt = faq.UpdatedAt
        };
    }

    public async Task<FAQDto> CreateFAQAsync(CreateFAQDto createDto)
    {
        var faq = new FAQ
        {
            QuestionEn = createDto.QuestionEn,
            QuestionAr = createDto.QuestionAr,
            AnswerEn = createDto.AnswerEn,
            AnswerAr = createDto.AnswerAr,
            Order = createDto.Order,
            IsActive = createDto.IsActive,
            CategoryId = createDto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.FAQs.Add(faq);
        await _context.SaveChangesAsync();

        return await GetFAQByIdAsync(faq.Id) ?? throw new InvalidOperationException();
    }

    public async Task<FAQDto> UpdateFAQAsync(UpdateFAQDto updateDto)
    {
        var faq = await _context.FAQs.FindAsync(updateDto.Id);
        if (faq == null) throw new ArgumentException("FAQ not found");

        faq.QuestionEn = updateDto.QuestionEn;
        faq.QuestionAr = updateDto.QuestionAr;
        faq.AnswerEn = updateDto.AnswerEn;
        faq.AnswerAr = updateDto.AnswerAr;
        faq.Order = updateDto.Order;
        faq.IsActive = updateDto.IsActive;
        faq.CategoryId = updateDto.CategoryId;
        faq.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetFAQByIdAsync(faq.Id) ?? throw new InvalidOperationException();
    }

    public async Task<bool> DeleteFAQAsync(int id)
    {
        var faq = await _context.FAQs.FindAsync(id);
        if (faq == null) return false;

        _context.FAQs.Remove(faq);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderFAQsAsync(Dictionary<int, int> orderUpdates)
    {
        foreach (var update in orderUpdates)
        {
            var faq = await _context.FAQs.FindAsync(update.Key);
            if (faq != null)
            {
                faq.Order = update.Value;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // FAQ Categories
    public async Task<IEnumerable<FAQCategoryDto>> GetAllCategoriesAsync()
    {
        return await _context.FAQCategories
            .Include(c => c.FAQs)
            .OrderBy(c => c.Order)
            .ThenBy(c => c.Id)
            .Select(c => new FAQCategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Order = c.Order,
                IsActive = c.IsActive,
                FAQCount = c.FAQs.Count,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<FAQCategoryDto>> GetActiveCategoriesAsync()
    {
        return await _context.FAQCategories
            .Include(c => c.FAQs)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Order)
            .ThenBy(c => c.Id)
            .Select(c => new FAQCategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Order = c.Order,
                IsActive = c.IsActive,
                FAQCount = c.FAQs.Count(f => f.IsActive),
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<FAQCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.FAQCategories
            .Include(c => c.FAQs)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new FAQCategoryDto
        {
            Id = category.Id,
            NameEn = category.NameEn,
            NameAr = category.NameAr,
            Order = category.Order,
            IsActive = category.IsActive,
            FAQCount = category.FAQs.Count,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<FAQCategoryDto> CreateCategoryAsync(CreateFAQCategoryDto createDto)
    {
        var category = new FAQCategory
        {
            NameEn = createDto.NameEn,
            NameAr = createDto.NameAr,
            Order = createDto.Order,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.FAQCategories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryByIdAsync(category.Id) ?? throw new InvalidOperationException();
    }

    public async Task<FAQCategoryDto> UpdateCategoryAsync(UpdateFAQCategoryDto updateDto)
    {
        var category = await _context.FAQCategories.FindAsync(updateDto.Id);
        if (category == null) throw new ArgumentException("Category not found");

        category.NameEn = updateDto.NameEn;
        category.NameAr = updateDto.NameAr;
        category.Order = updateDto.Order;
        category.IsActive = updateDto.IsActive;

        await _context.SaveChangesAsync();

        return await GetCategoryByIdAsync(category.Id) ?? throw new InvalidOperationException();
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.FAQCategories
            .Include(c => c.FAQs)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (category == null) return false;

        // Set all FAQs in this category to have no category
        foreach (var faq in category.FAQs)
        {
            faq.CategoryId = null;
        }

        _context.FAQCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderCategoriesAsync(Dictionary<int, int> orderUpdates)
    {
        foreach (var update in orderUpdates)
        {
            var category = await _context.FAQCategories.FindAsync(update.Key);
            if (category != null)
            {
                category.Order = update.Value;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // FAQ Page Settings
    public async Task<FAQPageDto> GetFAQPageSettingsAsync()
    {
        var settings = await _context.FAQPages.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            // Create default settings
            settings = new FAQPage
            {
                TitleEn = "Frequently Asked Questions",
                TitleAr = "الأسئلة الشائعة",
                DescriptionEn = "Find answers to common questions about our coffee products and services.",
                DescriptionAr = "اعثر على إجابات للأسئلة الشائعة حول منتجات القهوة والخدمات لدينا.",
                IsActive = true,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.FAQPages.Add(settings);
            await _context.SaveChangesAsync();
        }

        return new FAQPageDto
        {
            Id = settings.Id,
            TitleEn = settings.TitleEn,
            TitleAr = settings.TitleAr,
            DescriptionEn = settings.DescriptionEn,
            DescriptionAr = settings.DescriptionAr,
            MetaTitleEn = settings.MetaTitleEn,
            MetaTitleAr = settings.MetaTitleAr,
            MetaDescriptionEn = settings.MetaDescriptionEn,
            MetaDescriptionAr = settings.MetaDescriptionAr,
            IsActive = settings.IsActive,
            UpdatedAt = settings.UpdatedAt
        };
    }

    public async Task<FAQPageDto> UpdateFAQPageSettingsAsync(FAQPageDto updateDto)
    {
        var settings = await _context.FAQPages.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new FAQPage();
            _context.FAQPages.Add(settings);
        }

        settings.TitleEn = updateDto.TitleEn;
        settings.TitleAr = updateDto.TitleAr;
        settings.DescriptionEn = updateDto.DescriptionEn;
        settings.DescriptionAr = updateDto.DescriptionAr;
        settings.MetaTitleEn = updateDto.MetaTitleEn;
        settings.MetaTitleAr = updateDto.MetaTitleAr;
        settings.MetaDescriptionEn = updateDto.MetaDescriptionEn;
        settings.MetaDescriptionAr = updateDto.MetaDescriptionAr;
        settings.IsActive = updateDto.IsActive;
        settings.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetFAQPageSettingsAsync();
    }

    // Public Methods
    public async Task<IEnumerable<FAQDto>> GetPublicFAQsAsync(string? categorySlug = null)
    {
        var query = _context.FAQs
            .Include(f => f.Category)
            .Where(f => f.IsActive);

        if (!string.IsNullOrEmpty(categorySlug))
        {
            query = query.Where(f => f.Category != null && f.Category.IsActive);
        }

        return await query
            .OrderBy(f => f.Order)
            .ThenBy(f => f.Id)
            .Select(f => new FAQDto
            {
                Id = f.Id,
                QuestionEn = f.QuestionEn,
                QuestionAr = f.QuestionAr,
                AnswerEn = f.AnswerEn,
                AnswerAr = f.AnswerAr,
                Order = f.Order,
                IsActive = f.IsActive,
                CategoryId = f.CategoryId,
                CategoryName = f.Category != null ? f.Category.NameEn : null
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<FAQCategoryDto>> GetPublicCategoriesAsync()
    {
        return await _context.FAQCategories
            .Include(c => c.FAQs)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Order)
            .ThenBy(c => c.Id)
            .Select(c => new FAQCategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                Order = c.Order,
                IsActive = c.IsActive,
                FAQCount = c.FAQs.Count(f => f.IsActive)
            })
            .ToListAsync();
    }
}