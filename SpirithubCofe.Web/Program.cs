using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Web.Components;
using SpirithubCofe.Web.Components.Account;
using SpirithubCofe.Web.Data;

using SpirithubCofe.Web.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Circuit Options for detailed errors in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
    {
        options.DetailedErrors = true;
    });
}

// Add MVC controllers for culture switching
builder.Services.AddControllers();

// Add localization services
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizer<SpirithubCofe.Langs.Resources>, StringLocalizer<SpirithubCofe.Langs.Resources>>();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    // Add cookie provider as the first provider
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider
    {
        CookieName = "SpirithubCofe.Culture"
    });
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

// Register localization service
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

// Register cart service
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ToastService>();

// Register admin services
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<RoleManagementService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();

// Register slide service
builder.Services.AddScoped<SpirithubCofe.Application.Services.ISlideService, SpirithubCofe.Infrastructure.Services.SlideService>();

// Register setting service
builder.Services.AddScoped<SpirithubCofe.Application.Services.ISettingService, SpirithubCofe.Infrastructure.Services.SettingService>();

// Register FAQ service
builder.Services.AddScoped<SpirithubCofe.Application.Services.IFAQService, SpirithubCofe.Infrastructure.Services.FAQService>();
builder.Services.AddScoped<SpirithubCofe.Application.Interfaces.IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);



// Register data seeder service
builder.Services.AddScoped<DataSeederService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Add localization middleware
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.UseAntiforgery();

app.UseStaticFiles(); // Enable static file serving
app.MapStaticAssets();

// Map controllers for culture switching FIRST
app.MapControllerRoute(
    name: "culture",
    pattern: "Culture/{action=Index}/{id?}",
    defaults: new { controller = "Culture" });

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Seed roles and admin user
await SeedAdminUser(app);

// Seed sample data for categories and products
 await SeedSampleData(app); // Commented out for testing

app.Run();

async Task SeedAdminUser(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles if they don't exist
    string[] roles = { "Admin", "Staff", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create admin user if doesn't exist
    string adminEmail = "admin@spirithubcofe.com";
    string adminPassword = "Admin@123456";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

async Task SeedSampleData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seederService = scope.ServiceProvider.GetRequiredService<DataSeederService>();
    await seederService.SeedSampleDataAsync();
}
