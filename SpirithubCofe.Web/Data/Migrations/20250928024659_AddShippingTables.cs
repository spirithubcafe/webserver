using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCofe.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Code2 = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGccCountry = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    DescriptionEn = table.Column<string>(type: "TEXT", nullable: true),
                    DescriptionAr = table.Column<string>(type: "TEXT", nullable: true),
                    MetaTitleEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    MetaTitleAr = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    MetaDescriptionEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MetaDescriptionAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DescriptionAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFreePickup = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoolApiKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NoolAccountNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DeliveryDays = table.Column<int>(type: "INTEGER", nullable: false),
                    AramexAccountNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AramexUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AramexPassword = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AramexVersion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    AramexAccountPin = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AramexAccountEntity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AramexAccountCountryCode = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    AramexApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    NoolCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AramexCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    QuestionAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AnswerEn = table.Column<string>(type: "TEXT", nullable: false),
                    AnswerAr = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQs_FAQCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FAQCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ShippingZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ShippingMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingZones_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingZones_ShippingMethods_ShippingMethodId",
                        column: x => x.ShippingMethodId,
                        principalTable: "ShippingMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShippingZoneId = table.Column<int>(type: "INTEGER", nullable: false),
                    CityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    EstimatedDays = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRates_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRates_ShippingZones_ShippingZoneId",
                        column: x => x.ShippingZoneId,
                        principalTable: "ShippingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DisplayOrder",
                table: "Cities",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_IsActive",
                table: "Cities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code2",
                table: "Countries",
                column: "Code2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_DisplayOrder",
                table: "Countries",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_IsActive",
                table: "Countries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FAQCategories_IsActive",
                table: "FAQCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FAQCategories_Order",
                table: "FAQCategories",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_CategoryId",
                table: "FAQs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_CategoryId_Order",
                table: "FAQs",
                columns: new[] { "CategoryId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_IsActive",
                table: "FAQs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_Order",
                table: "FAQs",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_DisplayOrder",
                table: "ShippingMethods",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_IsActive",
                table: "ShippingMethods",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_Type",
                table: "ShippingMethods",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_CityId",
                table: "ShippingRates",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_IsActive",
                table: "ShippingRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_ShippingZoneId_CityId",
                table: "ShippingRates",
                columns: new[] { "ShippingZoneId", "CityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZones_CountryId",
                table: "ShippingZones",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZones_DisplayOrder",
                table: "ShippingZones",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZones_IsActive",
                table: "ShippingZones",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingZones_ShippingMethodId_CountryId",
                table: "ShippingZones",
                columns: new[] { "ShippingMethodId", "CountryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQPages");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropTable(
                name: "FAQCategories");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ShippingZones");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "ShippingMethods");
        }
    }
}
