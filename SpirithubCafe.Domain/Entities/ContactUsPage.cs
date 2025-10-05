using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities
{
    public class ContactUsPage
    {
        public int Id { get; set; }
        
        // Page Settings
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Page Content
        [MaxLength(200)]
        public string? Title { get; set; } = "Contact Us";
        [MaxLength(200)]
        public string? TitleAr { get; set; } = "اتصل بنا";
        
        [MaxLength(500)]
        public string? Subtitle { get; set; } = "Get in touch with us";
        [MaxLength(500)]
        public string? SubtitleAr { get; set; } = "تواصل معنا";
        
        [MaxLength(1000)]
        public string? Description { get; set; } = "We'd love to hear from you. Send us a message and we'll respond as soon as possible.";
        [MaxLength(1000)]
        public string? DescriptionAr { get; set; } = "نحن نحب أن نسمع منك. أرسل لنا رسالة وسنرد بأسرع ما يمكن.";
        
        // Background Settings
        public string? BgType { get; set; } = "color"; // color, image, video
        public string? BgValue { get; set; } = "#ffffff"; // color code, image path, or video path
        
        // Contact Information Settings
        public bool ShowContactForm { get; set; } = true;
        public bool ShowContactInfo { get; set; } = true;
        public bool ShowMap { get; set; } = true;
        public bool ShowSocialMedia { get; set; } = true;
        
        // Contact Form Settings
        [MaxLength(200)]
        public string? FormTitle { get; set; } = "Send us a Message";
        [MaxLength(200)]
        public string? FormTitleAr { get; set; } = "أرسل لنا رسالة";
        
        [MaxLength(500)]
        public string? FormDescription { get; set; } = "Fill out the form below and we'll get back to you.";
        [MaxLength(500)]
        public string? FormDescriptionAr { get; set; } = "املأ النموذج أدناه وسنعاود الاتصال بك.";
        
        // Business Hours
        [MaxLength(500)]
        public string? BusinessHours { get; set; } = "Monday - Friday: 7:00 AM - 10:00 PM\nSaturday - Sunday: 8:00 AM - 11:00 PM";
        [MaxLength(500)]
        public string? BusinessHoursAr { get; set; } = "الاثنين - الجمعة: 7:00 صباحاً - 10:00 مساءً\nالسبت - الأحد: 8:00 صباحاً - 11:00 مساءً";
        
        // Map Settings
        [MaxLength(1000)]
        public string? MapEmbedCode { get; set; }
        [MaxLength(500)]
        public string? MapAddress { get; set; } = "Al Mouj St, Muscat, Oman";
        [MaxLength(500)]
        public string? MapAddressAr { get; set; } = "شارع الموج، مسقط، عمان";
        
        // Contact Sections Order
        public int ContactFormOrder { get; set; } = 1;
        public int ContactInfoOrder { get; set; } = 2;
        public int MapOrder { get; set; } = 3;
        public int SocialMediaOrder { get; set; } = 4;
        
        // Success Message
        [MaxLength(500)]
        public string? SuccessMessage { get; set; } = "Thank you for your message! We'll get back to you soon.";
        [MaxLength(500)]
        public string? SuccessMessageAr { get; set; } = "شكراً لرسالتك! سنعاود الاتصال بك قريباً.";
    }
}