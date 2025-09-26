using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpirithubCofe.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadController> _logger;

    // Define allowed file types
    private readonly Dictionary<string, AllowedFileType> _allowedFileTypes = new()
    {
        {
            "image", new AllowedFileType
            {
                Extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" },
                MaxSizeBytes = 5 * 1024 * 1024, // 5MB
                MimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml" }
            }
        },
        {
            "document", new AllowedFileType
            {
                Extensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
                MaxSizeBytes = 10 * 1024 * 1024, // 10MB
                MimeTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain", "application/rtf" }
            }
        },
        {
            "video", new AllowedFileType
            {
                Extensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv" },
                MaxSizeBytes = 50 * 1024 * 1024, // 50MB
                MimeTypes = new[] { "video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv", "video/x-flv" }
            }
        },
        {
            "audio", new AllowedFileType
            {
                Extensions = new[] { ".mp3", ".wav", ".ogg", ".aac" },
                MaxSizeBytes = 20 * 1024 * 1024, // 20MB
                MimeTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac" }
            }
        }
    };

    // Define storage paths
    private readonly Dictionary<string, string> _uploadPaths = new()
    {
        { "categories", "images/categories" },
        { "products", "images/products" },
        { "slides", "images/slides" },
        { "users", "images/users" },
        { "brands", "images/brands" },
        { "banners", "images/banners" },
        { "documents", "files/documents" },
        { "videos", "files/videos" },
        { "audio", "files/audio" },
        { "temp", "files/temp" }
    };

    public FileUploadController(IWebHostEnvironment environment, ILogger<FileUploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Upload file to server
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="folder">Target folder (categories, products, slides, users, etc.)</param>
    /// <param name="fileType">File type (image, document, video, audio)</param>
    /// <param name="prefix">File name prefix (optional)</param>
    /// <returns></returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(
        IFormFile file, 
        [FromForm] string folder, 
        [FromForm] string fileType = "image",
        [FromForm] string? prefix = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file selected" });
            }

            // Check allowed folder
            if (!_uploadPaths.ContainsKey(folder))
            {
                return BadRequest(new { error = $"Folder '{folder}' not allowed. Allowed folders: {string.Join(", ", _uploadPaths.Keys)}" });
            }

            // Check allowed file type
            if (!_allowedFileTypes.ContainsKey(fileType))
            {
                return BadRequest(new { error = $"File type '{fileType}' not allowed. Allowed types: {string.Join(", ", _allowedFileTypes.Keys)}" });
            }

            var allowedType = _allowedFileTypes[fileType];
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            // Check file extension
            if (!allowedType.Extensions.Contains(fileExtension))
            {
                return BadRequest(new { 
                    error = $"File extension not allowed. Allowed extensions for {fileType}: {string.Join(", ", allowedType.Extensions)}" 
                });
            }

            // Check file size
            if (file.Length > allowedType.MaxSizeBytes)
            {
                var maxSizeMB = allowedType.MaxSizeBytes / (1024 * 1024);
                return BadRequest(new { error = $"File size must be less than {maxSizeMB}MB" });
            }

            // Create unique filename
            var fileName = $"{prefix ?? folder}-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
            var uploadPath = _uploadPaths[folder];
            var uploadsFolder = Path.Combine(_environment.WebRootPath, uploadPath);
            
            // Ensure directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/{uploadPath}/{fileName}".Replace("\\", "/");
            
            _logger.LogInformation("File uploaded successfully: {FileUrl} to folder: {Folder}", fileUrl, folder);

            return Ok(new { 
                success = true, 
                fileUrl = fileUrl,
                fileName = fileName,
                originalName = file.FileName,
                fileSize = file.Length,
                fileType = fileType,
                folder = folder,
                message = "File uploaded successfully!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to folder: {Folder}", folder);
            return StatusCode(500, new { error = "Error uploading file" });
        }
    }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="folder">File folder</param>
        /// <returns></returns>
        [HttpDelete("delete")]
        public IActionResult DeleteFile([FromQuery] string fileName, [FromQuery] string folder)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest(new { error = "File name is required" });
                }

                if (string.IsNullOrEmpty(folder) || !_uploadPaths.ContainsKey(folder))
                {
                    return BadRequest(new { error = "Invalid folder" });
                }            var uploadPath = _uploadPaths[folder];
            var uploadsFolder = Path.Combine(_environment.WebRootPath, uploadPath);
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileName} from folder: {Folder}", fileName, folder);
                return Ok(new { success = true, message = "File deleted successfully!" });
            }

            return NotFound(new { error = "File not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileName} from folder: {Folder}", fileName, folder);
            return StatusCode(500, new { error = "Error deleting file" });
        }
    }

        /// <summary>
        /// List files in a folder
        /// </summary>
        /// <param name="folder">Folder name</param>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult ListFiles([FromQuery] string folder)
        {
            try
            {
                if (string.IsNullOrEmpty(folder) || !_uploadPaths.ContainsKey(folder))
                {
                    return BadRequest(new { error = "Invalid folder" });
                }            var uploadPath = _uploadPaths[folder];
            var uploadsFolder = Path.Combine(_environment.WebRootPath, uploadPath);

            if (!Directory.Exists(uploadsFolder))
            {
                return Ok(new { files = new object[0] });
            }

            var files = Directory.GetFiles(uploadsFolder)
                .Select(filePath => 
                {
                    var fileInfo = new FileInfo(filePath);
                    var fileName = fileInfo.Name;
                    var fileUrl = $"/{uploadPath}/{fileName}".Replace("\\", "/");
                    
                    return new
                    {
                        fileName = fileName,
                        fileUrl = fileUrl,
                        fileSize = fileInfo.Length,
                        createdDate = fileInfo.CreationTime,
                        modifiedDate = fileInfo.LastWriteTime
                    };
                })
                .OrderByDescending(f => f.createdDate)
                .ToArray();

            return Ok(new { files = files });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in folder: {Folder}", folder);
            return StatusCode(500, new { error = "Error listing files" });
        }
    }

    /// <summary>
    /// Get upload folders and allowed file types information
    /// </summary>
    /// <returns></returns>
    [HttpGet("info")]
    [AllowAnonymous] // Public information
    public IActionResult GetUploadInfo()
    {
        var info = new
        {
            allowedFolders = _uploadPaths.Keys.ToArray(),
            allowedFileTypes = _allowedFileTypes.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    extensions = kvp.Value.Extensions,
                    maxSizeMB = kvp.Value.MaxSizeBytes / (1024 * 1024),
                    mimeTypes = kvp.Value.MimeTypes
                }
            ),
            uploadPaths = _uploadPaths
        };

        return Ok(info);
    }

    // Helper class for defining allowed file types
    private class AllowedFileType
    {
        public string[] Extensions { get; set; } = Array.Empty<string>();
        public long MaxSizeBytes { get; set; }
        public string[] MimeTypes { get; set; } = Array.Empty<string>();
    }
}
