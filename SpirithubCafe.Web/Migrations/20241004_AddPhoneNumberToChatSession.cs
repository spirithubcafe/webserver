using Microsoft.EntityFrameworkCore.Migrations;

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToChatSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ChatSessions",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ChatSessions");
        }
    }
}