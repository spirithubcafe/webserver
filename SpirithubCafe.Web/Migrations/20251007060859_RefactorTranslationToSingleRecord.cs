using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpirithubCafe.Web.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTranslationToSingleRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create a temporary table with the new structure
            migrationBuilder.Sql(@"
                CREATE TABLE Translations_New (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL,
                    ValueEn TEXT NOT NULL,
                    ValueAr TEXT NOT NULL,
                    Category TEXT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NULL
                );
            ");

            // Step 2: Migrate data - merge EN and AR records into single records
            migrationBuilder.Sql(@"
                INSERT INTO Translations_New (Key, ValueEn, ValueAr, Category, CreatedAt, UpdatedAt)
                SELECT 
                    en.Key,
                    COALESCE(en.Value, en.Key) as ValueEn,
                    COALESCE(ar.Value, en.Key) as ValueAr,
                    COALESCE(en.Category, ar.Category) as Category,
                    MIN(en.CreatedAt, COALESCE(ar.CreatedAt, en.CreatedAt)) as CreatedAt,
                    MAX(en.UpdatedAt, ar.UpdatedAt) as UpdatedAt
                FROM Translations en
                LEFT JOIN Translations ar ON en.Key = ar.Key AND ar.Language = 'ar'
                WHERE en.Language = 'en'
                
                UNION
                
                SELECT 
                    ar.Key,
                    COALESCE(en.Value, ar.Key) as ValueEn,
                    COALESCE(ar.Value, ar.Key) as ValueAr,
                    COALESCE(ar.Category, en.Category) as Category,
                    MIN(ar.CreatedAt, COALESCE(en.CreatedAt, ar.CreatedAt)) as CreatedAt,
                    MAX(ar.UpdatedAt, en.UpdatedAt) as UpdatedAt
                FROM Translations ar
                LEFT JOIN Translations en ON ar.Key = en.Key AND en.Language = 'en'
                WHERE ar.Language = 'ar' AND en.Key IS NULL;
            ");

            // Step 3: Drop the old table
            migrationBuilder.DropTable(name: "Translations");

            // Step 4: Rename the new table to the original name
            migrationBuilder.RenameTable(
                name: "Translations_New",
                newName: "Translations");

            // Step 5: Create unique index on Key
            migrationBuilder.CreateIndex(
                name: "IX_Translations_Key",
                table: "Translations",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create old structure table
            migrationBuilder.Sql(@"
                CREATE TABLE Translations_Old (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL,
                    Language TEXT NOT NULL,
                    Value TEXT NOT NULL,
                    Category TEXT NULL,
                    IsTranslated INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NULL
                );
            ");

            // Step 2: Split combined records back into EN and AR records
            migrationBuilder.Sql(@"
                INSERT INTO Translations_Old (Key, Language, Value, Category, IsTranslated, CreatedAt, UpdatedAt)
                SELECT Key, 'en', ValueEn, Category, 1, CreatedAt, UpdatedAt
                FROM Translations
                
                UNION ALL
                
                SELECT Key, 'ar', ValueAr, Category, 1, CreatedAt, UpdatedAt
                FROM Translations;
            ");

            // Step 3: Drop the new table
            migrationBuilder.DropTable(name: "Translations");

            // Step 4: Rename the old table back
            migrationBuilder.RenameTable(
                name: "Translations_Old",
                newName: "Translations");

            // Step 5: Recreate old indexes
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
    }
}
