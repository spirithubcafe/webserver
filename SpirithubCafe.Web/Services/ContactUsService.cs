using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Data;

namespace SpirithubCafe.Web.Services;

public class ContactUsService : IContactUsService
{
    private readonly ApplicationDbContext _context;

    public ContactUsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContactUsPage?> GetContactUsPageAsync()
    {
        return await _context.ContactUsPages
            .FirstOrDefaultAsync(p => p.IsActive);
    }

    public async Task<ContactUsPage> CreateOrUpdateContactUsPageAsync(ContactUsPage contactUsPage)
    {
        var existingPage = await _context.ContactUsPages.FirstOrDefaultAsync(p => p.IsActive);
        
        if (existingPage != null)
        {
            existingPage.Title = contactUsPage.Title;
            existingPage.TitleAr = contactUsPage.TitleAr;
            existingPage.Subtitle = contactUsPage.Subtitle;
            existingPage.SubtitleAr = contactUsPage.SubtitleAr;
            existingPage.Description = contactUsPage.Description;
            existingPage.DescriptionAr = contactUsPage.DescriptionAr;
            existingPage.BgType = contactUsPage.BgType;
            existingPage.BgValue = contactUsPage.BgValue;
            existingPage.ShowContactForm = contactUsPage.ShowContactForm;
            existingPage.ShowContactInfo = contactUsPage.ShowContactInfo;
            existingPage.ShowMap = contactUsPage.ShowMap;
            existingPage.ShowSocialMedia = contactUsPage.ShowSocialMedia;
            existingPage.FormTitle = contactUsPage.FormTitle;
            existingPage.FormTitleAr = contactUsPage.FormTitleAr;
            existingPage.FormDescription = contactUsPage.FormDescription;
            existingPage.FormDescriptionAr = contactUsPage.FormDescriptionAr;
            existingPage.BusinessHours = contactUsPage.BusinessHours;
            existingPage.BusinessHoursAr = contactUsPage.BusinessHoursAr;
            existingPage.MapEmbedCode = contactUsPage.MapEmbedCode;
            existingPage.MapAddress = contactUsPage.MapAddress;
            existingPage.MapAddressAr = contactUsPage.MapAddressAr;
            existingPage.ContactFormOrder = contactUsPage.ContactFormOrder;
            existingPage.ContactInfoOrder = contactUsPage.ContactInfoOrder;
            existingPage.MapOrder = contactUsPage.MapOrder;
            existingPage.SocialMediaOrder = contactUsPage.SocialMediaOrder;
            existingPage.SuccessMessage = contactUsPage.SuccessMessage;
            existingPage.SuccessMessageAr = contactUsPage.SuccessMessageAr;
            existingPage.UpdatedAt = DateTime.UtcNow;
            
            _context.ContactUsPages.Update(existingPage);
        }
        else
        {
            contactUsPage.CreatedAt = DateTime.UtcNow;
            contactUsPage.UpdatedAt = DateTime.UtcNow;
            contactUsPage.IsActive = true;
            
            _context.ContactUsPages.Add(contactUsPage);
            existingPage = contactUsPage;
        }
        
        await _context.SaveChangesAsync();
        return existingPage;
    }
}