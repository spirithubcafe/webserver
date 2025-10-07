using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GiftMessage",
                table: "Orders",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiftRecipientAddressLine1",
                table: "Orders",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiftRecipientAddressLine2",
                table: "Orders",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GiftRecipientCityId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GiftRecipientCountryId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiftRecipientName",
                table: "Orders",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiftRecipientPhone",
                table: "Orders",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiftRecipientPostalCode",
                table: "Orders",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGift",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AboutUsPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUsPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VisitorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VisitorEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasUnreadAdminMessages = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasUnreadVisitorMessages = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactUsPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", nullable: true),
                    BgValue = table.Column<string>(type: "TEXT", nullable: true),
                    ShowContactForm = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowContactInfo = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowMap = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowSocialMedia = table.Column<bool>(type: "INTEGER", nullable: false),
                    FormTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FormTitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FormDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FormDescriptionAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BusinessHours = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BusinessHoursAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MapEmbedCode = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    MapAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MapAddressAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ContactFormOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactInfoOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    MapOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SocialMediaOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SuccessMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SuccessMessageAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUsPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPolicyPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPolicyPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscribedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnsubscribedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivacyPolicyPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicyPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefundPolicyPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundPolicyPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermsConditionsPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsConditionsPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsTranslated = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AboutUsSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AboutUsPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ContentAr = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAlt = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ImageAltAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LayoutType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUsSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutUsSections_AboutUsPages_AboutUsPageId",
                        column: x => x.AboutUsPageId,
                        principalTable: "AboutUsPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ChatSessionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenderName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    SenderType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPolicySections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeliveryPolicyPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ContentAr = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAlt = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ImageAltAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LayoutType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPolicySections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPolicySections_DeliveryPolicyPages_DeliveryPolicyPageId",
                        column: x => x.DeliveryPolicyPageId,
                        principalTable: "DeliveryPolicyPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivacyPolicySections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrivacyPolicyPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ContentAr = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAlt = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ImageAltAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LayoutType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicySections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivacyPolicySections_PrivacyPolicyPages_PrivacyPolicyPageId",
                        column: x => x.PrivacyPolicyPageId,
                        principalTable: "PrivacyPolicyPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundPolicySections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefundPolicyPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ContentAr = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAlt = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ImageAltAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LayoutType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundPolicySections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundPolicySections_RefundPolicyPages_RefundPolicyPageId",
                        column: x => x.RefundPolicyPageId,
                        principalTable: "RefundPolicyPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermsConditionsSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TermsConditionsPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ContentAr = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageAlt = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ImageAltAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LayoutType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BgValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsConditionsSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermsConditionsSections_TermsConditionsPages_TermsConditionsPageId",
                        column: x => x.TermsConditionsPageId,
                        principalTable: "TermsConditionsPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AboutUsSections_AboutUsPageId",
                table: "AboutUsSections",
                column: "AboutUsPageId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutUsSections_DisplayOrder",
                table: "AboutUsSections",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatSessionId",
                table: "ChatMessages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CreatedAt",
                table: "ChatMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_IsRead",
                table: "ChatMessages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderType",
                table: "ChatMessages",
                column: "SenderType");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_CreatedAt",
                table: "ChatSessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_IsActive",
                table: "ChatSessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPolicySections_DeliveryPolicyPageId",
                table: "DeliveryPolicySections",
                column: "DeliveryPolicyPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacyPolicySections_PrivacyPolicyPageId",
                table: "PrivacyPolicySections",
                column: "PrivacyPolicyPageId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPolicySections_RefundPolicyPageId",
                table: "RefundPolicySections",
                column: "RefundPolicyPageId");

            migrationBuilder.CreateIndex(
                name: "IX_TermsConditionsSections_TermsConditionsPageId",
                table: "TermsConditionsSections",
                column: "TermsConditionsPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Category",
                table: "Translations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_IsTranslated",
                table: "Translations",
                column: "IsTranslated");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Key_Language",
                table: "Translations",
                columns: new[] { "Key", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUsSections");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ContactUsPages");

            migrationBuilder.DropTable(
                name: "DeliveryPolicySections");

            migrationBuilder.DropTable(
                name: "NewsletterSubscriptions");

            migrationBuilder.DropTable(
                name: "PrivacyPolicySections");

            migrationBuilder.DropTable(
                name: "RefundPolicySections");

            migrationBuilder.DropTable(
                name: "TermsConditionsSections");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "AboutUsPages");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "DeliveryPolicyPages");

            migrationBuilder.DropTable(
                name: "PrivacyPolicyPages");

            migrationBuilder.DropTable(
                name: "RefundPolicyPages");

            migrationBuilder.DropTable(
                name: "TermsConditionsPages");

            migrationBuilder.DropColumn(
                name: "GiftMessage",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientAddressLine1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientAddressLine2",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientCityId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientCountryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientPhone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GiftRecipientPostalCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsGift",
                table: "Orders");
        }
    }
}
