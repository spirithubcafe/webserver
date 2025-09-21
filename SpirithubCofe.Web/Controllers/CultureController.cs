using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SpirithubCofe.Web.Controllers;

public class CultureController : Controller
{
    private readonly ILogger<CultureController> _logger;

    public CultureController(ILogger<CultureController> logger)
    {
        _logger = logger;
    }

    public IActionResult Test()
    {
        _logger.LogInformation("Test endpoint called!");
        return Json(new { status = "ok", culture = Thread.CurrentThread.CurrentCulture.Name });
    }

    public IActionResult Set(string culture, string returnUrl)
    {
        _logger.LogInformation("CultureController.Set called with culture: {Culture}, returnUrl: {ReturnUrl}", culture, returnUrl);
        
        if (!string.IsNullOrEmpty(culture))
        {
            _logger.LogInformation("Setting culture cookie: SpirithubCofe.Culture = {Culture}", culture);
            HttpContext.Response.Cookies.Append(
                "SpirithubCofe.Culture",
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.Lax
                }
            );
        }

        var redirectUrl = returnUrl ?? "/";
        _logger.LogInformation("Redirecting to: {RedirectUrl}", redirectUrl);
        return LocalRedirect(redirectUrl);
    }
}