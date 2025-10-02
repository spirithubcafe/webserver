using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddFooterManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FooterMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MenuType = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpenInNewTab = table.Column<bool>(type: "INTEGER", nullable: false),
                    IconClass = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DescriptionAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FooterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShowFooter = table.Column<bool>(type: "INTEGER", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CompanyNameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "color"),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true, defaultValue: "#111827"),
                    EnableOverlay = table.Column<bool>(type: "INTEGER", nullable: false),
                    OverlayType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "blur"),
                    OverlayValue = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "0.5"),
                    TextColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "#ffffff"),
                    AccentColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "#f59e0b"),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AddressAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkingHours = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkingHoursAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CopyrightText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CopyrightTextAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FacebookUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    InstagramUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TwitterUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WhatsAppUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TikTokUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SnapchatUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PinterestUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TelegramUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ShowSocialMedia = table.Column<bool>(type: "INTEGER", nullable: false),
                    SocialMediaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SocialMediaTitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ShowQuickLinks = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuickLinksTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    QuickLinksTitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ShowLegalPages = table.Column<bool>(type: "INTEGER", nullable: false),
                    LegalPagesTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LegalPagesTitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ShowContactInfo = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactTitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FooterMenus_IsActive",
                table: "FooterMenus",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FooterMenus_MenuType_SortOrder",
                table: "FooterMenus",
                columns: new[] { "MenuType", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterMenus");

            migrationBuilder.DropTable(
                name: "FooterSettings");
        }
    }
}
