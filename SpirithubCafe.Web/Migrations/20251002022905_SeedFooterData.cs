using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedFooterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert default footer settings
            migrationBuilder.InsertData(
                table: "FooterSettings",
                columns: new[] {
                    "Id", "CompanyName", "CompanyNameAr", "Description", "DescriptionAr",
                    "Address", "AddressAr", "Phone1", "Phone2", "Email", "WorkingHours", "WorkingHoursAr",
                    "BgType", "BgValue", "OverlayType", "OverlayValue", 
                    "FacebookUrl", "TwitterUrl", "InstagramUrl", "YouTubeUrl", "TikTokUrl", 
                    "WhatsAppUrl", "TelegramUrl", "LinkedInUrl", "SnapchatUrl", "PinterestUrl",
                    "ShowSocialMedia", "ShowQuickLinks", "ShowLegalPages", "ShowContactInfo",
                    "CopyrightText", "CopyrightTextAr"
                },
                values: new object[] {
                    1, "SpirithubCafe", "سبيريت هاب كافيه", 
                    "Premium Coffee Experience", "تجربة قهوة مميزة",
                    "Al Mouj St, Muscat, Oman", "شارع الموج، مسقط، عمان",
                    "+968 9190 0005", "+968 7272 6999", "info@spirithubcafe.com",
                    "Daily: 7 AM - 12 AM", "يومياً: 7 صباحاً - 12 منتصف الليل",
                    "color", "#111827", "blur", "0.5",
                    "", "", "", "", "",
                    "", "", "", "", "",
                    true, true, true, true,
                    "2025 SpirithubCafe. All rights reserved.", "2025 سبيريت هاب كافيه. جميع الحقوق محفوظة."
                });

            // Insert default Quick Links menu items
            migrationBuilder.InsertData(
                table: "FooterMenus",
                columns: new[] { "Id", "MenuType", "Title", "TitleAr", "Url", "SortOrder", "IsActive", "OpenInNewTab" },
                values: new object[,]
                {
                    { 1, 1, "Products", "المنتجات", "/products", 1, true, false },
                    { 2, 1, "Shipping Info", "معلومات الشحن", "/shipping", 2, true, false },
                    { 3, 1, "About Us", "من نحن", "/about", 3, true, false },
                    { 4, 1, "Contact", "اتصل بنا", "/contact", 4, true, false }
                });

            // Insert default Legal Pages menu items
            migrationBuilder.InsertData(
                table: "FooterMenus",
                columns: new[] { "Id", "MenuType", "Title", "TitleAr", "Url", "SortOrder", "IsActive", "OpenInNewTab" },
                values: new object[,]
                {
                    { 5, 2, "Privacy Policy", "سياسة الخصوصية", "/privacy", 1, true, false },
                    { 6, 2, "Terms of Service", "شروط الخدمة", "/terms", 2, true, false },
                    { 7, 2, "Return Policy", "سياسة الإرجاع", "/returns", 3, true, false },
                    { 8, 2, "FAQ", "الأسئلة الشائعة", "/faq", 4, true, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded menu items
            migrationBuilder.DeleteData(
                table: "FooterMenus",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            // Remove seeded footer settings
            migrationBuilder.DeleteData(
                table: "FooterSettings",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
