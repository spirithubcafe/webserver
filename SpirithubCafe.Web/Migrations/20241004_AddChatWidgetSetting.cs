using Microsoft.EntityFrameworkCore.Migrations;

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddChatWidgetSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ChatWidgetEnabled setting
            migrationBuilder.Sql(@"
                INSERT INTO Settings (Key, Value, Description, DescriptionAr, Category, DataType, IsRequired, CreatedAt, UpdatedAt)
                VALUES (
                    'ChatWidgetEnabled',
                    'true',
                    'Enable or disable the chat widget on the website',
                    'تفعيل أو تعطيل أداة الدردشة على الموقع',
                    'Chat',
                    'Boolean',
                    1,
                    datetime('now'),
                    datetime('now')
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Settings WHERE Key = 'ChatWidgetEnabled'");
        }
    }
}