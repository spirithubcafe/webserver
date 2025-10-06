using Microsoft.Extensions.FileProviders;
using SpirithubCafe.Web.Services;

namespace SpirithubCafe.Web.Middleware
{
    /// <summary>
    /// Middleware for adding cache headers to static assets
    /// </summary>
    public class StaticAssetCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StaticAssetCacheMiddleware> _logger;
        private readonly AssetVersioningService _assetVersioning;
        private readonly HashSet<string> _cacheableExtensions;

        public StaticAssetCacheMiddleware(
            RequestDelegate next,
            ILogger<StaticAssetCacheMiddleware> logger,
            AssetVersioningService assetVersioning)
        {
            _next = next;
            _logger = logger;
            _assetVersioning = assetVersioning;
            _cacheableExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", 
                ".ico", ".woff", ".woff2", ".ttf", ".eot", ".otf"
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Only process successful responses for static assets
            if (context.Response.StatusCode == 200 && 
                context.Request.Path.HasValue && 
                !context.Response.HasStarted)
            {
                var path = context.Request.Path.Value;
                var extension = Path.GetExtension(path);

                // Check if this is a cacheable static asset
                if (_cacheableExtensions.Contains(extension))
                {
                    var hasVersion = context.Request.Query.ContainsKey("v");
                    
                    if (hasVersion)
                    {
                        // Long cache for versioned assets (1 year)
                        context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
                        context.Response.Headers.Expires = DateTime.UtcNow.AddYears(1).ToString("R");
                        
                        _logger.LogDebug("Applied long cache headers to versioned asset: {Path}", path);
                    }
                    else
                    {
                        // Short cache for non-versioned assets (1 hour)
                        context.Response.Headers.CacheControl = "public, max-age=3600";
                        context.Response.Headers.Expires = DateTime.UtcNow.AddHours(1).ToString("R");
                        
                        _logger.LogDebug("Applied short cache headers to non-versioned asset: {Path}", path);
                    }
                    
                    // Add ETag for better caching
                    if (!context.Response.Headers.ContainsKey("ETag"))
                    {
                        var etag = $"\"{_assetVersioning.GetAssetVersion(path.TrimStart('/'))}\"";
                        context.Response.Headers.ETag = etag;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extension methods for registering the middleware
    /// </summary>
    public static class StaticAssetCacheMiddlewareExtensions
    {
        /// <summary>
        /// Add static asset cache middleware to the pipeline
        /// </summary>
        public static IApplicationBuilder UseStaticAssetCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StaticAssetCacheMiddleware>();
        }
    }
}