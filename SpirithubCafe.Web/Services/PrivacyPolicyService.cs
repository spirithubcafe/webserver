using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class PrivacyPolicyService : IPrivacyPolicyService
{
    private readonly ApplicationDbContext _context;

    public PrivacyPolicyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrivacyPolicyPage?> GetPrivacyPolicyPageAsync()
    {
        return await _context.PrivacyPolicyPages
            .Include(p => p.Sections.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<PrivacyPolicyPage> CreateOrUpdatePrivacyPolicyPageAsync(PrivacyPolicyPage privacyPolicyPage)
    {
        var existingPage = await _context.PrivacyPolicyPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = privacyPolicyPage.Title;
            existingPage.TitleAr = privacyPolicyPage.TitleAr;
            existingPage.Subtitle = privacyPolicyPage.Subtitle;
            existingPage.SubtitleAr = privacyPolicyPage.SubtitleAr;
            existingPage.BgType = privacyPolicyPage.BgType;
            existingPage.BgValue = privacyPolicyPage.BgValue;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.PrivacyPolicyPages.Update(existingPage);
        }
        else
        {
            privacyPolicyPage.CreatedAt = DateTime.UtcNow;
            privacyPolicyPage.UpdatedAt = DateTime.UtcNow;
            privacyPolicyPage.IsActive = true;
            
            _context.PrivacyPolicyPages.Add(privacyPolicyPage);
            existingPage = privacyPolicyPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }

    public async Task<List<PrivacyPolicySection>> GetPrivacyPolicySectionsAsync()
    {
        var page = await GetPrivacyPolicyPageAsync();
        if (page == null) return new List<PrivacyPolicySection>();
        
        return await _context.PrivacyPolicySections
            .Where(s => s.PrivacyPolicyPageId == page.Id && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<PrivacyPolicySection?> GetPrivacyPolicySectionByIdAsync(int id)
    {
        return await _context.PrivacyPolicySections.FindAsync(id);
    }

    public async Task<PrivacyPolicySection> CreatePrivacyPolicySectionAsync(PrivacyPolicySection section)
    {
        var page = await GetPrivacyPolicyPageAsync();
        if (page == null)
        {
            page = new PrivacyPolicyPage();
            page = await CreateOrUpdatePrivacyPolicyPageAsync(page);
        }
        
        section.PrivacyPolicyPageId = page.Id;
        section.CreatedAt = DateTime.UtcNow;
        section.UpdatedAt = DateTime.UtcNow;
        section.IsActive = true;
        
        // Set display order to last
        var maxOrder = await _context.PrivacyPolicySections
            .Where(s => s.PrivacyPolicyPageId == page.Id)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        section.DisplayOrder = maxOrder + 1;
        
        _context.PrivacyPolicySections.Add(section);
        await _context.SaveChangesAsync();
        
        return section;
    }

    public async Task<PrivacyPolicySection> UpdatePrivacyPolicySectionAsync(PrivacyPolicySection section)
    {
        var existingSection = await _context.PrivacyPolicySections.FindAsync(section.Id);
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

    public async Task<bool> DeletePrivacyPolicySectionAsync(int id)
    {
        var section = await _context.PrivacyPolicySections.FindAsync(id);
        if (section == null) return false;
        
        _context.PrivacyPolicySections.Remove(section);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ReorderSectionsAsync(List<int> sectionIds)
    {
        try
        {
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var section = await _context.PrivacyPolicySections.FindAsync(sectionIds[i]);
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
