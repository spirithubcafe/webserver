using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class AboutUsService : IAboutUsService
{
    private readonly ApplicationDbContext _context;

    public AboutUsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AboutUsPage?> GetAboutUsPageAsync()
    {
        return await _context.AboutUsPages
            .Include(p => p.Sections.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<AboutUsPage> CreateOrUpdateAboutUsPageAsync(AboutUsPage aboutUsPage)
    {
        var existingPage = await _context.AboutUsPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = aboutUsPage.Title;
            existingPage.TitleAr = aboutUsPage.TitleAr;
            existingPage.Subtitle = aboutUsPage.Subtitle;
            existingPage.SubtitleAr = aboutUsPage.SubtitleAr;
            existingPage.BgType = aboutUsPage.BgType;
            existingPage.BgValue = aboutUsPage.BgValue;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.AboutUsPages.Update(existingPage);
        }
        else
        {
            aboutUsPage.CreatedAt = DateTime.UtcNow;
            aboutUsPage.UpdatedAt = DateTime.UtcNow;
            aboutUsPage.IsActive = true;
            
            _context.AboutUsPages.Add(aboutUsPage);
            existingPage = aboutUsPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }

    public async Task<List<AboutUsSection>> GetAboutUsSectionsAsync()
    {
        var page = await GetAboutUsPageAsync();
        if (page == null) return new List<AboutUsSection>();
        
        return await _context.AboutUsSections
            .Where(s => s.AboutUsPageId == page.Id && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<AboutUsSection?> GetAboutUsSectionByIdAsync(int id)
    {
        return await _context.AboutUsSections.FindAsync(id);
    }

    public async Task<AboutUsSection> CreateAboutUsSectionAsync(AboutUsSection section)
    {
        var page = await GetAboutUsPageAsync();
        if (page == null)
        {
            page = new AboutUsPage();
            page = await CreateOrUpdateAboutUsPageAsync(page);
        }
        
        section.AboutUsPageId = page.Id;
        section.CreatedAt = DateTime.UtcNow;
        section.UpdatedAt = DateTime.UtcNow;
        section.IsActive = true;
        
        // Set display order to last
        var maxOrder = await _context.AboutUsSections
            .Where(s => s.AboutUsPageId == page.Id)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        section.DisplayOrder = maxOrder + 1;
        
        _context.AboutUsSections.Add(section);
        await _context.SaveChangesAsync();
        
        return section;
    }

    public async Task<AboutUsSection> UpdateAboutUsSectionAsync(AboutUsSection section)
    {
        var existingSection = await _context.AboutUsSections.FindAsync(section.Id);
        if (existingSection == null)
            throw new InvalidOperationException($"Section with ID {section.Id} not found");

        // Update all fields explicitly
        existingSection.Title = section.Title;
        existingSection.TitleAr = section.TitleAr;
        existingSection.Content = section.Content;
        existingSection.ContentAr = section.ContentAr;
        existingSection.ImagePath = section.ImagePath;
        existingSection.ImageAlt = section.ImageAlt;
        existingSection.ImageAltAr = section.ImageAltAr;
        existingSection.LayoutType = section.LayoutType;
        existingSection.DisplayOrder = section.DisplayOrder;
        existingSection.IsActive = section.IsActive;
        existingSection.BgType = section.BgType;
        existingSection.BgValue = section.BgValue;
        existingSection.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return existingSection;
    }

    public async Task<bool> DeleteAboutUsSectionAsync(int id)
    {
        var section = await _context.AboutUsSections.FindAsync(id);
        if (section == null) return false;
        
        _context.AboutUsSections.Remove(section);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ReorderSectionsAsync(List<int> sectionIds)
    {
        try
        {
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var section = await _context.AboutUsSections.FindAsync(sectionIds[i]);
                if (section != null)
                {
                    section.DisplayOrder = i + 1;
                    section.UpdatedAt = DateTime.UtcNow;
                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}