using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class TermsConditionsService : ITermsConditionsService
{
    private readonly ApplicationDbContext _context;

    public TermsConditionsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TermsConditionsPage?> GetTermsConditionsPageAsync()
    {
        return await _context.TermsConditionsPages
            .Include(p => p.Sections.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<TermsConditionsPage> CreateOrUpdateTermsConditionsPageAsync(TermsConditionsPage termsConditionsPage)
    {
        var existingPage = await _context.TermsConditionsPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = termsConditionsPage.Title;
            existingPage.TitleAr = termsConditionsPage.TitleAr;
            existingPage.Subtitle = termsConditionsPage.Subtitle;
            existingPage.SubtitleAr = termsConditionsPage.SubtitleAr;
            existingPage.BgType = termsConditionsPage.BgType;
            existingPage.BgValue = termsConditionsPage.BgValue;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.TermsConditionsPages.Update(existingPage);
        }
        else
        {
            termsConditionsPage.CreatedAt = DateTime.UtcNow;
            termsConditionsPage.UpdatedAt = DateTime.UtcNow;
            termsConditionsPage.IsActive = true;
            
            _context.TermsConditionsPages.Add(termsConditionsPage);
            existingPage = termsConditionsPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }

    public async Task<List<TermsConditionsSection>> GetTermsConditionsSectionsAsync()
    {
        var page = await GetTermsConditionsPageAsync();
        if (page == null) return new List<TermsConditionsSection>();
        
        return await _context.TermsConditionsSections
            .Where(s => s.TermsConditionsPageId == page.Id && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<TermsConditionsSection?> GetTermsConditionsSectionByIdAsync(int id)
    {
        return await _context.TermsConditionsSections.FindAsync(id);
    }

    public async Task<TermsConditionsSection> CreateTermsConditionsSectionAsync(TermsConditionsSection section)
    {
        var page = await GetTermsConditionsPageAsync();
        if (page == null)
        {
            page = new TermsConditionsPage();
            page = await CreateOrUpdateTermsConditionsPageAsync(page);
        }
        
        section.TermsConditionsPageId = page.Id;
        section.CreatedAt = DateTime.UtcNow;
        section.UpdatedAt = DateTime.UtcNow;
        section.IsActive = true;
        
        // Set display order to last
        var maxOrder = await _context.TermsConditionsSections
            .Where(s => s.TermsConditionsPageId == page.Id)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        section.DisplayOrder = maxOrder + 1;
        
        _context.TermsConditionsSections.Add(section);
        await _context.SaveChangesAsync();
        
        return section;
    }

    public async Task<TermsConditionsSection> UpdateTermsConditionsSectionAsync(TermsConditionsSection section)
    {
        var existingSection = await _context.TermsConditionsSections.FindAsync(section.Id);
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

    public async Task<bool> DeleteTermsConditionsSectionAsync(int id)
    {
        var section = await _context.TermsConditionsSections.FindAsync(id);
        if (section == null) return false;
        
        _context.TermsConditionsSections.Remove(section);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ReorderSectionsAsync(List<int> sectionIds)
    {
        try
        {
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var section = await _context.TermsConditionsSections.FindAsync(sectionIds[i]);
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
