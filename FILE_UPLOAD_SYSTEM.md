# Universal File Upload System

## Overview
A comprehensive file upload system for SpirithubCofe that handles all file types across the application with proper validation, security, and organization.

## Features

### Supported File Types
- **Images**: JPG, JPEG, PNG, GIF, WebP, SVG (up to 5MB)
- **Documents**: PDF, DOC, DOCX, TXT, RTF (up to 10MB)
- **Videos**: MP4, AVI, MOV, WMV, FLV (up to 50MB)
- **Audio**: MP3, WAV, OGG, AAC (up to 20MB)

### Organized Storage
Files are automatically organized into appropriate folders:
- `wwwroot/images/categories/` - Category images
- `wwwroot/images/products/` - Product images
- `wwwroot/images/slides/` - Slider images
- `wwwroot/images/users/` - User profile images
- `wwwroot/images/brands/` - Brand logos
- `wwwroot/images/banners/` - Banner images
- `wwwroot/files/documents/` - Document files
- `wwwroot/files/videos/` - Video files
- `wwwroot/files/audio/` - Audio files
- `wwwroot/files/temp/` - Temporary files

## API Endpoints

### Upload File
```
POST /api/FileUpload/upload
```

**Parameters:**
- `file` (IFormFile): The file to upload
- `folder` (string): Destination folder (categories, products, slides, etc.)
- `fileType` (string): File type (image, document, video, audio)
- `prefix` (string, optional): Filename prefix

**Response:**
```json
{
  "success": true,
  "fileUrl": "/images/categories/category-20240321-143052-abc123def.jpg",
  "fileName": "category-20240321-143052-abc123def.jpg",
  "originalName": "my-image.jpg",
  "fileSize": 245760,
  "fileType": "image",
  "folder": "categories",
  "message": "File uploaded successfully!"
}
```

### Delete File
```
DELETE /api/FileUpload/delete?fileName=category-20240321-143052-abc123def.jpg&folder=categories
```

**Response:**
```json
{
  "success": true,
  "message": "File deleted successfully!"
}
```

### List Files
```
GET /api/FileUpload/list?folder=categories
```

**Response:**
```json
{
  "files": [
    {
      "fileName": "category-20240321-143052-abc123def.jpg",
      "fileUrl": "/images/categories/category-20240321-143052-abc123def.jpg",
      "fileSize": 245760,
      "createdDate": "2024-03-21T14:30:52",
      "modifiedDate": "2024-03-21T14:30:52"
    }
  ]
}
```

### Get Upload Info
```
GET /api/FileUpload/info
```

**Response:**
```json
{
  "allowedFolders": ["categories", "products", "slides", "users", "brands", "banners", "documents", "videos", "audio", "temp"],
  "allowedFileTypes": {
    "image": {
      "extensions": [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"],
      "maxSizeMB": 5,
      "mimeTypes": ["image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml"]
    }
  },
  "uploadPaths": {
    "categories": "images/categories",
    "products": "images/products"
  }
}
```

## Blazor Component Usage

### FileUpload Component
The reusable `FileUpload` component can be used in any Blazor page:

```razor
<FileUpload Folder="categories" 
           FileType="image" 
           Prefix="category"
           @bind-CurrentFileUrl="model.ImagePath"
           AltText="Category Image" 
           OnFileUploaded="OnFileUploaded"
           OnFileRemoved="OnFileRemoved" />
```

**Parameters:**
- `Folder`: Destination folder
- `FileType`: Type of file (image, document, video, audio)
- `Prefix`: Optional filename prefix
- `CurrentFileUrl`: Two-way binding for the file URL
- `AltText`: Alt text for images
- `OnFileUploaded`: Callback when file is uploaded
- `OnFileRemoved`: Callback when file is removed

## Security Features

### Validation
- File extension validation
- MIME type validation
- File size validation
- Allowed folder validation

### Access Control
- Admin role required for upload/delete operations
- Public access for info endpoint

### File Naming
- Unique filenames with timestamp and GUID
- Prevents file overwrites
- Organized by date and type

## Usage Examples

### Category Image Upload
```razor
<FileUpload Folder="categories" 
           FileType="image" 
           Prefix="category"
           @bind-CurrentFileUrl="category.ImagePath" />
```

### Product Document Upload
```razor
<FileUpload Folder="products" 
           FileType="document" 
           Prefix="manual"
           @bind-CurrentFileUrl="product.ManualUrl" />
```

### Slide Video Upload
```razor
<FileUpload Folder="slides" 
           FileType="video" 
           Prefix="hero"
           @bind-CurrentFileUrl="slide.VideoUrl" />
```

## Error Handling

The system provides comprehensive error handling for:
- Invalid file types
- File size exceeds limits
- Invalid folders
- Server errors
- Network issues

All errors are returned in a consistent format:
```json
{
  "error": "File size must be less than 5MB"
}
```

## Notes

- All text is in English and Arabic only (no Persian)
- Files are automatically organized by type and date
- Unique filenames prevent conflicts
- Proper MIME type validation for security
- Responsive UI with drag-and-drop support
- Real-time upload progress indication