using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class DeliveryPolicyService : IDeliveryPolicyService
{
    private readonly ApplicationDbContext _context;

    public DeliveryPolicyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryPolicyPage?> GetDeliveryPolicyPageAsync()
    {
        return await _context.DeliveryPolicyPages
            .Include(p => p.Sections.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<DeliveryPolicyPage> CreateOrUpdateDeliveryPolicyPageAsync(DeliveryPolicyPage deliveryPolicyPage)
    {
        var existingPage = await _context.DeliveryPolicyPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = deliveryPolicyPage.Title;
            existingPage.TitleAr = deliveryPolicyPage.TitleAr;
            existingPage.Subtitle = deliveryPolicyPage.Subtitle;
            existingPage.SubtitleAr = deliveryPolicyPage.SubtitleAr;
            existingPage.BgType = deliveryPolicyPage.BgType;
            existingPage.BgValue = deliveryPolicyPage.BgValue;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.DeliveryPolicyPages.Update(existingPage);
        }
        else
        {
            deliveryPolicyPage.CreatedAt = DateTime.UtcNow;
            deliveryPolicyPage.UpdatedAt = DateTime.UtcNow;
            deliveryPolicyPage.IsActive = true;
            
            _context.DeliveryPolicyPages.Add(deliveryPolicyPage);
            existingPage = deliveryPolicyPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }

    public async Task<List<DeliveryPolicySection>> GetDeliveryPolicySectionsAsync()
    {
        var page = await GetDeliveryPolicyPageAsync();
        if (page == null) return new List<DeliveryPolicySection>();
        
        return await _context.DeliveryPolicySections
            .Where(s => s.DeliveryPolicyPageId == page.Id && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<DeliveryPolicySection?> GetDeliveryPolicySectionByIdAsync(int id)
    {
        return await _context.DeliveryPolicySections.FindAsync(id);
    }

    public async Task<DeliveryPolicySection> CreateDeliveryPolicySectionAsync(DeliveryPolicySection section)
    {
        var page = await GetDeliveryPolicyPageAsync();
        if (page == null)
        {
            page = new DeliveryPolicyPage();
            page = await CreateOrUpdateDeliveryPolicyPageAsync(page);
        }
        
        section.DeliveryPolicyPageId = page.Id;
        section.CreatedAt = DateTime.UtcNow;
        section.UpdatedAt = DateTime.UtcNow;
        section.IsActive = true;
        
        // Set display order to last
        var maxOrder = await _context.DeliveryPolicySections
            .Where(s => s.DeliveryPolicyPageId == page.Id)
            .MaxAsync(s => (int?)s.DisplayOrder) ?? 0;
        section.DisplayOrder = maxOrder + 1;
        
        _context.DeliveryPolicySections.Add(section);
        await _context.SaveChangesAsync();
        
        return section;
    }

    public async Task<DeliveryPolicySection> UpdateDeliveryPolicySectionAsync(DeliveryPolicySection section)
    {
        var existingSection = await _context.DeliveryPolicySections.FindAsync(section.Id);
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

    public async Task<bool> DeleteDeliveryPolicySectionAsync(int id)
    {
        var section = await _context.DeliveryPolicySections.FindAsync(id);
        if (section == null) return false;
        
        _context.DeliveryPolicySections.Remove(section);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ReorderSectionsAsync(List<int> sectionIds)
    {
        try
        {
            for (int i = 0; i < sectionIds.Count; i++)
            {
                var section = await _context.DeliveryPolicySections.FindAsync(sectionIds[i]);
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
