using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCofe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAramexSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AramexSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountPin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountEntity = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AccountCountryCode = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    ApiVersion = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContactName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AddressLine1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AddressLine2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StateProvince = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    DomesticServices = table.Column<string>(type: "TEXT", nullable: false),
                    InternationalServices = table.Column<string>(type: "TEXT", nullable: false),
                    OndLabel = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CdsLabel = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EpxLabel = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PpxLabel = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    GrdLabel = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OndLabelAr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CdsLabelAr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    EpxLabelAr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PpxLabelAr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    GrdLabelAr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AramexSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AramexSettings");
        }
    }
}
