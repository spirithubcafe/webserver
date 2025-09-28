using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCofe.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShippingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarrierCode",
                table: "ShippingMethods",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarrierCode",
                table: "ShippingMethods");
        }
    }
}
