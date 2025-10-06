using Microsoft.AspNetCore.Razor.TagHelpers;
using SpirithubCafe.Web.Services;

namespace SpirithubCafe.Web.TagHelpers
{
    /// <summary>
    /// Tag Helper for adding version query parameters to CSS and JS files
    /// </summary>
    [HtmlTargetElement("link", Attributes = "asp-version")]
    [HtmlTargetElement("script", Attributes = "asp-version")]
    public class AssetVersionTagHelper : TagHelper
    {
        private readonly AssetVersioningService _assetVersioningService;

        public AssetVersionTagHelper(AssetVersioningService assetVersioningService)
        {
            _assetVersioningService = assetVersioningService;
        }

        /// <summary>
        /// Whether to append version to the asset URL
        /// </summary>
        [HtmlAttributeName("asp-version")]
        public bool AppendVersion { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!AppendVersion)
                return;

            // Handle CSS files (link tags)
            if (output.TagName == "link" && output.Attributes.TryGetAttribute("href", out var hrefAttribute))
            {
                var href = hrefAttribute.Value?.ToString();
                if (!string.IsNullOrEmpty(href) && !href.StartsWith("http") && !href.Contains("?"))
                {
                    var versionedUrl = _assetVersioningService.GetVersionedCssUrl(href);
                    output.Attributes.SetAttribute("href", versionedUrl);
                }
            }
            
            // Handle JS files (script tags)
            else if (output.TagName == "script" && output.Attributes.TryGetAttribute("src", out var srcAttribute))
            {
                var src = srcAttribute.Value?.ToString();
                if (!string.IsNullOrEmpty(src) && !src.StartsWith("http") && !src.Contains("?"))
                {
                    var versionedUrl = _assetVersioningService.GetVersionedJsUrl(src);
                    output.Attributes.SetAttribute("src", versionedUrl);
                }
            }
        }
    }
}