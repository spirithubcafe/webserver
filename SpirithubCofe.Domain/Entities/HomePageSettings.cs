using System.ComponentModel.DataAnnotations;

namespace SpirithubCofe.Domain.Entities
{
    public class HomePageSettings
    {
        public int Id { get; set; }
        
        // Slideshow Section
        public bool ShowSlideshow { get; set; } = true;
        
        // Categories Section
        public bool ShowCategories { get; set; } = true;
        [MaxLength(200)]
        public string? CategoriesTitle { get; set; } = "Our Categories";
        [MaxLength(200)]
        public string? CategoriesTitleAr { get; set; } = "فئاتنا";
        [MaxLength(500)]
        public string? CategoriesSubtitle { get; set; } = "Explore our wide range of products";
        [MaxLength(500)]
        public string? CategoriesSubtitleAr { get; set; } = "اكتشف مجموعتنا الواسعة من المنتجات";
        public int CategoriesDisplayCount { get; set; } = 8;
        public string? CategoriesBgType { get; set; } = "color"; // color, image, video
        public string? CategoriesBgValue { get; set; } = "#f8f9fa"; // color code, image path, or video path
        
        // Mission Section
        public bool ShowMission { get; set; } = true;
        [MaxLength(200)]
        public string? MissionTitle { get; set; } = "Our Mission";
        [MaxLength(200)]
        public string? MissionTitleAr { get; set; } = "مهمتنا";
        [MaxLength(500)]
        public string? MissionSubtitle { get; set; } = "What drives us forward";
        [MaxLength(500)]
        public string? MissionSubtitleAr { get; set; } = "ما يدفعنا إلى الأمام";
        [MaxLength(1000)]
        public string? MissionText { get; set; }
        [MaxLength(1000)]
        public string? MissionTextAr { get; set; }
        public string? MissionBgType { get; set; } = "color"; // color, image, video
        public string? MissionBgValue { get; set; } = "#ffffff"; // color code, image path, or video path
        
        // Latest Products Section
        public bool ShowLatestProducts { get; set; } = true;
        [MaxLength(200)]
        public string? LatestProductsTitle { get; set; } = "Latest Products";
        [MaxLength(200)]
        public string? LatestProductsTitleAr { get; set; } = "أحدث المنتجات";
        [MaxLength(500)]
        public string? LatestProductsSubtitle { get; set; } = "Discover our newest arrivals";
        [MaxLength(500)]
        public string? LatestProductsSubtitleAr { get; set; } = "اكتشف أحدث وصولاتنا";
        public int LatestProductsCount { get; set; } = 6;
        public string? LatestProductsBgType { get; set; } = "color"; // color, image, video
        public string? LatestProductsBgValue { get; set; } = "#f8f9fa"; // color code, image path, or video path
        
        // Newsletter Section
        public bool ShowNewsletter { get; set; } = true;
        [MaxLength(200)]
        public string? NewsletterTitle { get; set; } = "Stay Updated";
        [MaxLength(200)]
        public string? NewsletterTitleAr { get; set; } = "ابق على اطلاع";
        [MaxLength(500)]
        public string? NewsletterSubtitle { get; set; } = "Subscribe to get the latest news and offers";
        [MaxLength(500)]
        public string? NewsletterSubtitleAr { get; set; } = "اشترك للحصول على آخر الأخبار والعروض";
        public string? NewsletterBgType { get; set; } = "color"; // color, image, video
        public string? NewsletterBgValue { get; set; } = "#f8f9fa"; // color code, image path, or video path
        [MaxLength(1000)]
        public string? NewsletterImage { get; set; } // Image path for newsletter section
        
        // About Us Section
        public bool ShowAboutUs { get; set; } = false;
        [MaxLength(200)]
        public string? AboutUsTitle { get; set; } = "About Us";
        [MaxLength(200)]
        public string? AboutUsTitleAr { get; set; } = "من نحن";
        [MaxLength(500)]
        public string? AboutUsSubtitle { get; set; } = "Learn more about our story";
        [MaxLength(500)]
        public string? AboutUsSubtitleAr { get; set; } = "تعرف على قصتنا";
        [MaxLength(2000)]
        public string? AboutUsText { get; set; }
        [MaxLength(2000)]
        public string? AboutUsTextAr { get; set; }
        public string? AboutUsBgType { get; set; } = "color"; // color, image, video
        public string? AboutUsBgValue { get; set; } = "#ffffff"; // color code, image path, or video path
        [MaxLength(1000)]
        public string? AboutUsImage { get; set; } // Image path for about us section
        
        // Section Display Order
        public int SlideshowOrder { get; set; } = 1;
        public int CategoriesOrder { get; set; } = 2;
        public int MissionOrder { get; set; } = 3;
        public int LatestProductsOrder { get; set; } = 4;
        public int AboutUsOrder { get; set; } = 5;
        public int NewsletterOrder { get; set; } = 6;
        
        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}