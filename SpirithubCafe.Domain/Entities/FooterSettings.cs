using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities
{
    public class FooterSettings
    {
        public int Id { get; set; }
        
        // General Footer Settings
        public bool ShowFooter { get; set; } = true;
        
        // Logo Settings
        [MaxLength(500)]
        public string? LogoUrl { get; set; } = "/images/logo.png";
        [MaxLength(200)]
        public string? CompanyName { get; set; } = "SpirithubCafe";
        [MaxLength(200)]
        public string? CompanyNameAr { get; set; } = "سبيريت هب كافيه";
        
        // Description Settings
        [MaxLength(1000)]
        public string? Description { get; set; } = "Premium Coffee Experience";
        [MaxLength(1000)]
        public string? DescriptionAr { get; set; } = "تجربة قهوة متميزة";
        
        // Background Settings
        public string? BgType { get; set; } = "color"; // color, image, video
        public string? BgValue { get; set; } = "#111827"; // color code, image path, or video path
        public bool EnableOverlay { get; set; } = false;
        public string? OverlayType { get; set; } = "blur"; // blur, mask, gradient
        public string? OverlayValue { get; set; } = "0.5"; // blur intensity (px), mask opacity (0-1), gradient colors
        
        // Text Colors
        public string? TextColor { get; set; } = "#ffffff";
        public string? AccentColor { get; set; } = "#f59e0b";
        
        // Contact Information
        [MaxLength(500)]
        public string? Address { get; set; } = "Al Mouj St, Muscat, Oman";
        [MaxLength(500)]
        public string? AddressAr { get; set; } = "شارع الموج، مسقط، عمان";
        
        [MaxLength(100)]
        public string? Phone1 { get; set; } = "+968 9190 0005";
        [MaxLength(100)]
        public string? Phone2 { get; set; } = "+968 7272 6999";
        
        [MaxLength(200)]
        public string? Email { get; set; } = "info@spirithubcafe.com";
        
        [MaxLength(200)]
        public string? WorkingHours { get; set; } = "Daily: 7 AM - 12 AM";
        [MaxLength(200)]
        public string? WorkingHoursAr { get; set; } = "يومياً: 7 صباحاً - 12 منتصف الليل";
        
        // Copyright Text
        [MaxLength(500)]
        public string? CopyrightText { get; set; } = "2025 SpirithubCafe. All rights reserved.";
        [MaxLength(500)]
        public string? CopyrightTextAr { get; set; } = "2025 سبيريت هب كافيه. جميع الحقوق محفوظة.";
        
        // Social Media Links
        [MaxLength(500)]
        public string? FacebookUrl { get; set; }
        [MaxLength(500)]
        public string? InstagramUrl { get; set; }
        [MaxLength(500)]
        public string? TwitterUrl { get; set; }
        [MaxLength(500)]
        public string? LinkedInUrl { get; set; }
        [MaxLength(500)]
        public string? WhatsAppUrl { get; set; }
        [MaxLength(500)]
        public string? YouTubeUrl { get; set; }
        [MaxLength(500)]
        public string? TikTokUrl { get; set; }
        [MaxLength(500)]
        public string? SnapchatUrl { get; set; }
        [MaxLength(500)]
        public string? PinterestUrl { get; set; }
        [MaxLength(500)]
        public string? TelegramUrl { get; set; }
        
        // Social Media Section Settings
        public bool ShowSocialMedia { get; set; } = true;
        [MaxLength(200)]
        public string? SocialMediaTitle { get; set; } = "Follow Us";
        [MaxLength(200)]
        public string? SocialMediaTitleAr { get; set; } = "تابعنا";
        
        // Column 1: Quick Links Settings
        public bool ShowQuickLinks { get; set; } = true;
        [MaxLength(200)]
        public string? QuickLinksTitle { get; set; } = "Quick Links";
        [MaxLength(200)]
        public string? QuickLinksTitleAr { get; set; } = "روابط سريعة";
        
        // Column 2: Legal Pages Settings
        public bool ShowLegalPages { get; set; } = true;
        [MaxLength(200)]
        public string? LegalPagesTitle { get; set; } = "Legal Pages";
        [MaxLength(200)]
        public string? LegalPagesTitleAr { get; set; } = "الصفحات القانونية";
        
        // Column 3: Contact Info Settings
        public bool ShowContactInfo { get; set; } = true;
        [MaxLength(200)]
        public string? ContactTitle { get; set; } = "Contact Us";
        [MaxLength(200)]
        public string? ContactTitleAr { get; set; } = "اتصل بنا";
    }
}