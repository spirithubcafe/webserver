using System.ComponentModel.DataAnnotations;

namespace SpirithubCafe.Domain.Entities
{
    public static class FooterMenuType
    {
        public const int QuickLinks = 1;
        public const int LegalPages = 2;
    }

    public class FooterMenu
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "";
        
        [MaxLength(200)]
        public string? TitleAr { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = "";
        
        public int MenuType { get; set; } // 1 = Quick Links, 2 = Legal Pages
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public bool OpenInNewTab { get; set; } = false;
        
        [MaxLength(100)]
        public string? IconClass { get; set; } // CSS class for icon (optional)
        
        [MaxLength(500)]
        public string? Description { get; set; } // Optional description for the link
        
        [MaxLength(500)]
        public string? DescriptionAr { get; set; } // Optional description in Arabic
    }
}