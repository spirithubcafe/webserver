using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCofe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Countries
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Code", "Name", "NameAr", "IsActive" },
                values: new object[,]
                {
                    { "OM", "Oman", "عُمان", true },
                    { "AE", "United Arab Emirates", "الإمارات العربية المتحدة", true },
                    { "SA", "Saudi Arabia", "المملكة العربية السعودية", true },
                    { "KW", "Kuwait", "الكويت", true },
                    { "QA", "Qatar", "قطر", true },
                    { "BH", "Bahrain", "البحرين", true }
                });

            // Insert Cities for Oman (assuming Oman gets ID 1)
            migrationBuilder.Sql(@"
                INSERT INTO Cities (Name, NameAr, CountryId, IsActive) 
                SELECT 'Muscat', 'مسقط', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Salalah', 'صلالة', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Nizwa', 'نزوى', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Sur', 'صور', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Sohar', 'صحار', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Rustaq', 'الرستاق', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Buraimi', 'البريمي', Id, 1 FROM Countries WHERE Code = 'OM'
                UNION ALL
                SELECT 'Ibri', 'عبري', Id, 1 FROM Countries WHERE Code = 'OM'
            ");

            // Insert Cities for UAE
            migrationBuilder.Sql(@"
                INSERT INTO Cities (Name, NameAr, CountryId, IsActive) 
                SELECT 'Dubai', 'دبي', Id, 1 FROM Countries WHERE Code = 'AE'
                UNION ALL
                SELECT 'Abu Dhabi', 'أبو ظبي', Id, 1 FROM Countries WHERE Code = 'AE'
                UNION ALL
                SELECT 'Sharjah', 'الشارقة', Id, 1 FROM Countries WHERE Code = 'AE'
                UNION ALL
                SELECT 'Ajman', 'عجمان', Id, 1 FROM Countries WHERE Code = 'AE'
            ");

            // Insert Shipping Methods
            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Type", "Name", "NameAr", "Description", "DescriptionAr", "IsActive", "DisplayOrder", "CreatedAt" },
                values: new object[,]
                {
                    { "Pickup", "Store Pickup", "استلام من المتجر", "Pickup your order from our store", "استلام طلبك من متجرنا", true, 1, DateTime.UtcNow },
                    { "NoolOman", "Nool Oman Delivery", "توصيل نول عُمان", "Fast delivery within Oman", "توصيل سريع داخل عُمان", true, 2, DateTime.UtcNow },
                    { "Aramex", "Aramex International", "أرامكس الدولي", "International shipping via Aramex", "الشحن الدولي عبر أرامكس", true, 3, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove sample data
            migrationBuilder.Sql("DELETE FROM Cities WHERE CountryId IN (SELECT Id FROM Countries WHERE Code IN ('OM', 'AE', 'SA', 'KW', 'QA', 'BH'))");
            migrationBuilder.Sql("DELETE FROM ShippingMethods WHERE Type IN ('Pickup', 'NoolOman', 'Aramex')");
            migrationBuilder.Sql("DELETE FROM Countries WHERE Code IN ('OM', 'AE', 'SA', 'KW', 'QA', 'BH')");
        }
    }
}
