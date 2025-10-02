using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Domain.Entities;

namespace SpirithubCafe.Application.Services
{
    public class FooterService : IFooterService
    {
        private readonly IApplicationDbContext _context;

        public FooterService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FooterSettings> GetFooterSettingsAsync()
        {
            var settings = await _context.FooterSettings.FirstOrDefaultAsync();
            
            if (settings == null)
            {
                // Create default settings if none exist
                settings = new FooterSettings();
                _context.FooterSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            
            return settings;
        }

        public async Task<FooterSettings> UpdateFooterSettingsAsync(FooterSettings settings)
        {
            var existingSettings = await _context.FooterSettings.FirstOrDefaultAsync();
            
            if (existingSettings == null)
            {
                _context.FooterSettings.Add(settings);
            }
            else
            {
                // Update all properties
                existingSettings.ShowFooter = settings.ShowFooter;
                existingSettings.LogoUrl = settings.LogoUrl;
                existingSettings.CompanyName = settings.CompanyName;
                existingSettings.CompanyNameAr = settings.CompanyNameAr;
                existingSettings.Description = settings.Description;
                existingSettings.DescriptionAr = settings.DescriptionAr;
                existingSettings.BgType = settings.BgType;
                existingSettings.BgValue = settings.BgValue;
                existingSettings.EnableOverlay = settings.EnableOverlay;
                existingSettings.OverlayType = settings.OverlayType;
                existingSettings.OverlayValue = settings.OverlayValue;
                existingSettings.TextColor = settings.TextColor;
                existingSettings.AccentColor = settings.AccentColor;
                existingSettings.Address = settings.Address;
                existingSettings.AddressAr = settings.AddressAr;
                existingSettings.Phone1 = settings.Phone1;
                existingSettings.Phone2 = settings.Phone2;
                existingSettings.Email = settings.Email;
                existingSettings.WorkingHours = settings.WorkingHours;
                existingSettings.WorkingHoursAr = settings.WorkingHoursAr;
                existingSettings.CopyrightText = settings.CopyrightText;
                existingSettings.CopyrightTextAr = settings.CopyrightTextAr;
                existingSettings.FacebookUrl = settings.FacebookUrl;
                existingSettings.InstagramUrl = settings.InstagramUrl;
                existingSettings.TwitterUrl = settings.TwitterUrl;
                existingSettings.LinkedInUrl = settings.LinkedInUrl;
                existingSettings.WhatsAppUrl = settings.WhatsAppUrl;
                existingSettings.YouTubeUrl = settings.YouTubeUrl;
                existingSettings.TikTokUrl = settings.TikTokUrl;
                existingSettings.SnapchatUrl = settings.SnapchatUrl;
                existingSettings.PinterestUrl = settings.PinterestUrl;
                existingSettings.TelegramUrl = settings.TelegramUrl;
                existingSettings.ShowSocialMedia = settings.ShowSocialMedia;
                existingSettings.SocialMediaTitle = settings.SocialMediaTitle;
                existingSettings.SocialMediaTitleAr = settings.SocialMediaTitleAr;
                existingSettings.ShowQuickLinks = settings.ShowQuickLinks;
                existingSettings.QuickLinksTitle = settings.QuickLinksTitle;
                existingSettings.QuickLinksTitleAr = settings.QuickLinksTitleAr;
                existingSettings.ShowLegalPages = settings.ShowLegalPages;
                existingSettings.LegalPagesTitle = settings.LegalPagesTitle;
                existingSettings.LegalPagesTitleAr = settings.LegalPagesTitleAr;
                existingSettings.ShowContactInfo = settings.ShowContactInfo;
                existingSettings.ContactTitle = settings.ContactTitle;
                existingSettings.ContactTitleAr = settings.ContactTitleAr;
                
                settings = existingSettings;
            }
            
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<List<FooterMenu>> GetFooterMenusAsync()
        {
            return await _context.FooterMenus
                .OrderBy(m => m.MenuType)
                .ThenBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<FooterMenu>> GetFooterMenusByTypeAsync(int menuType)
        {
            return await _context.FooterMenus
                .Where(m => m.MenuType == menuType)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<FooterMenu> GetFooterMenuByIdAsync(int id)
        {
            var menu = await _context.FooterMenus.FindAsync(id);
            if (menu == null)
                throw new InvalidOperationException($"Footer menu with ID {id} not found.");
            
            return menu;
        }

        public async Task<FooterMenu> CreateFooterMenuAsync(FooterMenu menu)
        {
            // Auto-assign sort order
            var maxOrder = await _context.FooterMenus
                .Where(m => m.MenuType == menu.MenuType)
                .MaxAsync(m => (int?)m.SortOrder) ?? 0;
            
            menu.SortOrder = maxOrder + 1;
            
            _context.FooterMenus.Add(menu);
            await _context.SaveChangesAsync();
            
            return menu;
        }

        public async Task<FooterMenu> UpdateFooterMenuAsync(FooterMenu menu)
        {
            var existingMenu = await _context.FooterMenus.FindAsync(menu.Id);
            if (existingMenu == null)
                throw new InvalidOperationException($"Footer menu with ID {menu.Id} not found.");

            existingMenu.Title = menu.Title;
            existingMenu.TitleAr = menu.TitleAr;
            existingMenu.Url = menu.Url;
            existingMenu.MenuType = menu.MenuType;
            existingMenu.IsActive = menu.IsActive;
            existingMenu.OpenInNewTab = menu.OpenInNewTab;
            existingMenu.IconClass = menu.IconClass;
            existingMenu.Description = menu.Description;
            existingMenu.DescriptionAr = menu.DescriptionAr;
            
            await _context.SaveChangesAsync();
            return existingMenu;
        }

        public async Task<bool> DeleteFooterMenuAsync(int id)
        {
            var menu = await _context.FooterMenus.FindAsync(id);
            if (menu == null)
                return false;

            _context.FooterMenus.Remove(menu);
            await _context.SaveChangesAsync();
            
            // Reorder remaining items
            var remainingMenus = await _context.FooterMenus
                .Where(m => m.MenuType == menu.MenuType && m.SortOrder > menu.SortOrder)
                .ToListAsync();
                
            foreach (var remainingMenu in remainingMenus)
            {
                remainingMenu.SortOrder--;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMenuOrderAsync(List<(int id, int order)> menuOrders)
        {
            foreach (var (id, order) in menuOrders)
            {
                var menu = await _context.FooterMenus.FindAsync(id);
                if (menu != null)
                {
                    menu.SortOrder = order;
                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleMenuStatusAsync(int id)
        {
            var menu = await _context.FooterMenus.FindAsync(id);
            if (menu == null)
                return false;

            menu.IsActive = !menu.IsActive;
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}