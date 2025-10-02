# Copilot Instructions for SpirithubCafe

## Components Usage
- Use <Toast /> component from '@using SpirithubCafe.Web.Components.Shared' to show toast messages
- Use file upload component from '@using SpirithubCafe.Web.Components.Shared' to upload files FileUpload.razor
- FileUpload component parameters: FileType, Folder, Prefix, CurrentFileUrl (two-way binding)

## Entity Framework Migrations
### Prerequisites
1. Ensure EF Core tools are installed globally:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
2. Add dotnet global tools to PATH:
   ```bash
   export PATH="$PATH:/home/milad/.dotnet/tools"
   ```

### Creating and Applying Migrations
1. Navigate to the Web project directory:
   ```bash
   cd /home/milad/Documents/GitHub/spirithubcafe/webserver/SpirithubCafe.Web
   ```

2. Create a new migration:
   ```bash
   dotnet ef migrations add MigrationName
   ```

3. Update the database:
   ```bash
   dotnet ef database update
   ```

4. Remove last migration (if needed):
   ```bash
   dotnet ef migrations remove
   ```

### Entity Changes Pattern
- When adding new fields to entities, always use appropriate MaxLength attributes
- Follow naming convention: `SectionNameFieldType` (e.g., NewsletterBgType, NewsletterBgValue)
- Background types should be: "color", "image", "video"
- Background values store: color codes (#ffffff), image paths, or video paths

## Database Structure
- Main database: SQLite at `/home/milad/Documents/GitHub/spirithubcafe/webserver/SpirithubCafe.Web/Data/app.db`
- Use SQLite commands to check data:
  ```bash
  sqlite3 /home/milad/Documents/GitHub/spirithubcafe/webserver/SpirithubCafe.Web/Data/app.db "SELECT * FROM TableName;"
  ```

## Admin Interface Patterns
### Background Settings UI Pattern
For sections with background settings, use this conditional rendering pattern:

```razor
<!-- Background Type Selection -->
<div class="space-y-2">
    <label class="block text-sm font-semibold text-gray-700">Background Type</label>
    <select @onchange="OnBgTypeChanged" class="w-full px-4 py-3 border border-gray-200 rounded-lg">
        <option value="color" selected="@(settings.SectionBgType == "color")">Color</option>
        <option value="image" selected="@(settings.SectionBgType == "image")">Image</option>
        <option value="video" selected="@(settings.SectionBgType == "video")">Video</option>
    </select>
</div>

<!-- Color Picker (when color is selected) -->
@if (settings.SectionBgType == "color")
{
    <div class="space-y-2">
        <label class="block text-sm font-semibold text-gray-700">Background Color</label>
        <div class="flex gap-2">
            <input type="color" @bind="settings.SectionBgValue" 
                   class="w-12 h-10 border border-gray-200 rounded cursor-pointer" />
            <InputText @bind-Value="settings.SectionBgValue" 
                       class="flex-1 px-4 py-3 border border-gray-200 rounded-lg" 
                       placeholder="#ffffff" />
        </div>
    </div>
}

<!-- File Upload (when image or video is selected) -->
@if (settings.SectionBgType == "image" || settings.SectionBgType == "video")
{
    <div class="space-y-2">
        <label class="block text-sm font-semibold text-gray-700">
            @(settings.SectionBgType == "image" ? "Background Image" : "Background Video")
        </label>
        <FileUpload FileType="@(settings.SectionBgType == "image" ? "image" : "video")"
                    Folder="backgrounds"
                    Prefix="@($"section-{settings.SectionBgType}")"
                    @bind-CurrentFileUrl="settings.SectionBgValue" />
    </div>
}
```

## Build and Run Commands
### Development
```bash
cd /home/milad/Documents/GitHub/spirithubcafe/webserver
dotnet build
dotnet run --project SpirithubCafe.Web
```

### Publish
```bash
dotnet publish -c Release -o publish/windows-x64 --self-contained true -r win-x64
```

## Common Entity Patterns
### Homepage Settings Entity Structure
- Show{Section}: bool (enable/disable section)
- {Section}Title: string (English title)
- {Section}TitleAr: string (Arabic title)  
- {Section}Subtitle: string (English subtitle)
- {Section}SubtitleAr: string (Arabic subtitle)
- {Section}BgType: string ("color", "image", "video")
- {Section}BgValue: string (color code, image path, video path)
- Additional section-specific fields as needed

## Project Structure
- Domain Layer: `/SpirithubCafe.Domain/Entities/` - Entity definitions
- Web Layer: `/SpirithubCafe.Web/Components/Pages/Admin/` - Admin interface
- Shared Components: `/SpirithubCafe.Web/Components/Shared/` - Reusable components
- Data: `/SpirithubCafe.Web/Data/` - Database context and files
- Migrations: `/SpirithubCafe.Web/Migrations/` - EF Core migrations