using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class RefundPolicyService : IRefundPolicyService
{
    private readonly ApplicationDbContext _context;

    public RefundPolicyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefundPolicyPage?> GetRefundPolicyPageAsync()
    {
        return await _context.RefundPolicyPages
            .Include(p => p.Sections.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<RefundPolicyPage> CreateOrUpdateRefundPolicyPageAsync(RefundPolicyPage refundPolicyPage)
    {
        var existingPage = await _context.RefundPolicyPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = refundPolicyPage.Title;
            existingPage.TitleAr = refundPolicyPage.TitleAr;
            existingPage.Subtitle = refundPolicyPage.Subtitle;
            existingPage.SubtitleAr = refundPolicyPage.SubtitleAr;
            existingPage.BgType = refundPolicyPage.BgType;
            existingPage.BgValue = refundPolicyPage.BgValue;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.RefundPolicyPages.Update(existingPage);
        }
        else
        {
            refundPolicyPage.CreatedAt = DateTime.UtcNow;
            refundPolicyPage.UpdatedAt = DateTime.UtcNow;
            refundPolicyPage.IsActive = true;
            
            _context.RefundPolicyPages.Add(refundPolicyPage);
            existingPage = refundPolicyPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }

    public async Task<List<RefundPolicySection>> GetRefundPolicySectionsAsync()
    {
        var page = await GetRefundPolicyPageAsync();
        if (page == null) return new List<RefundPolicySection>();
        
        return await _context.RefundPolicySections
            .Where(s => s.RefundPolicyPageId == page.Id && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<RefundPolicySection?> GetRefundPolicySectionByIdAsync(int id)
    {
        return await _context.RefundPolicySections.FindAsync(id);
    }

    public async Task<RefundPolicySection> CreateRefundPolicySectionAsync(RefundPolicySection section)
    {
        var page = await GetRefundPolicyPageAsync();
        if (page == null)
        {
            page = new RefundPolicyPage();
            page = await CreateOrUpdateRefundPolicyPageAsync(page);
        }
        
        section.RefundPolicyPageId = page.Id;
        section.CreatedAt = DateTime.UtcNow;
        section.UpdatedAt = DateTime.UtcNow;
        section.IsActive = true;
        
        // Set display order to last
        var maxOrder = await _context.RefundPolicySections
            .Where(s => s.RefundPolicyPageId == page.Id)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        section.DisplayOrder = maxOrder + 1;
        
        _context.RefundPolicySections.Add(section);
        await _context.SaveChangesAsync();
        
        return section;
    }

    public async Task<RefundPolicySection> UpdateRefundPolicySectionAsync(RefundPolicySection section)
    {
        var existingSection = await _context.RefundPolicySections.FindAsync(section.Id);
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

    public async Task<bool> DeleteRefundPolicySectionAsync(int id)
    {
        var section = await _context.RefundPolicySections.FindAsync(id);
        if (section == null) return false;
        
        _context.RefundPolicySections.Remove(section);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ReorderSectionsAsync(List<int> sectionIds)
    {
        try
        {
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var section = await _context.RefundPolicySections.FindAsync(sectionIds[i]);
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
