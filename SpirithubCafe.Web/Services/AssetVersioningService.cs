using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SpirithubCafe.Web.Services
{
    /// <summary>
    /// Service for managing static asset versioning and cache busting
    /// </summary>
    public class AssetVersioningService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AssetVersioningService> _logger;
        private readonly Dictionary<string, string> _assetVersions;
        private readonly string _appVersion;

        public AssetVersioningService(IWebHostEnvironment environment, ILogger<AssetVersioningService> logger)
        {
            _environment = environment;
            _logger = logger;
            _assetVersions = new Dictionary<string, string>();
            
            // Get application version from assembly
            _appVersion = GetApplicationVersion();
            
            // Initialize asset versions
            InitializeAssetVersions();
        }

        /// <summary>
        /// Get versioned URL for a CSS file
        /// </summary>
        /// <param name="cssFile">CSS file path (e.g., "dist.css")</param>
        /// <returns>Versioned CSS URL</returns>
        public string GetVersionedCssUrl(string cssFile)
        {
            var version = GetAssetVersion(cssFile);
            return $"{cssFile}?v={version}";
        }

        /// <summary>
        /// Get versioned URL for a JS file
        /// </summary>
        /// <param name="jsFile">JS file path (e.g., "app.js")</param>
        /// <returns>Versioned JS URL</returns>
        public string GetVersionedJsUrl(string jsFile)
        {
            var version = GetAssetVersion(jsFile);
            return $"{jsFile}?v={version}";
        }

        /// <summary>
        /// Get version for any asset
        /// </summary>
        /// <param name="assetPath">Asset file path</param>
        /// <returns>Version string</returns>
        public string GetAssetVersion(string assetPath)
        {
            if (_assetVersions.TryGetValue(assetPath, out var version))
            {
                return version;
            }

            // Generate version based on file modification time or app version
            var fullPath = Path.Combine(_environment.WebRootPath, assetPath);
            
            if (File.Exists(fullPath))
            {
                var lastModified = File.GetLastWriteTimeUtc(fullPath);
                version = lastModified.Ticks.ToString("x");
            }
            else
            {
                // Fallback to app version if file doesn't exist
                version = _appVersion;
            }

            _assetVersions[assetPath] = version;
            return version;
        }

        /// <summary>
        /// Invalidate cache for a specific asset
        /// </summary>
        /// <param name="assetPath">Asset file path</param>
        public void InvalidateAssetCache(string assetPath)
        {
            _assetVersions.Remove(assetPath);
            _logger.LogInformation("Invalidated cache for asset: {AssetPath}", assetPath);
        }

        /// <summary>
        /// Clear all asset caches
        /// </summary>
        public void ClearAllCaches()
        {
            _assetVersions.Clear();
            _logger.LogInformation("Cleared all asset caches");
        }

        /// <summary>
        /// Get application version from assembly
        /// </summary>
        /// <returns>Application version string</returns>
        private string GetApplicationVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? DateTime.UtcNow.Ticks.ToString("x");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not get application version, using timestamp");
                return DateTime.UtcNow.Ticks.ToString("x");
            }
        }

        /// <summary>
        /// Initialize versions for common assets
        /// </summary>
        private void InitializeAssetVersions()
        {
            var commonAssets = new[]
            {
                "dist.css",
                "app.css",
                "site.css",
                "app.js",
                "site.js"
            };

            foreach (var asset in commonAssets)
            {
                GetAssetVersion(asset); // This will cache the version
            }
        }

        /// <summary>
        /// Get cache headers for static assets
        /// </summary>
        /// <returns>Cache control headers</returns>
        public static Dictionary<string, string> GetCacheHeaders()
        {
            return new Dictionary<string, string>
            {
                { "Cache-Control", "public, max-age=31536000, immutable" }, // 1 year cache
                { "Expires", DateTime.UtcNow.AddYears(1).ToString("R") },
                { "ETag", $"\"{DateTime.UtcNow.Ticks:x}\"" }
            };
        }
    }
}