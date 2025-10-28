using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpirithubCafe.Web.Components;
using SpirithubCafe.Web.Components.Account;
using SpirithubCafe.Web.Data;
using SpirithubCafe.Domain.Entities;
using SpirithubCafe.Web.Services;
using SpirithubCafe.Application.Interfaces;
using SpirithubCafe.Application.Services;
using SpirithubCafe.Application.Services.API;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using SpirithubCafe.Web.Middleware;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// PERFORMANCE OPTIMIZATION: Response Compression
// ============================================================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream", "image/svg+xml" });
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Configure Forwarded Headers for reverse proxy (NGINX, IIS, etc.)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                               ForwardedHeaders.XForwardedProto | 
                               ForwardedHeaders.XForwardedHost;
    
    // Clear known networks and proxies to allow any proxy
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    
    // Required for WebSocket connections behind reverse proxy
    options.AllowedHosts.Clear();
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Antiforgery services
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = ".AspNetCore.Antiforgery";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add HttpContextAccessor for Blazor Server
builder.Services.AddHttpContextAccessor();

// Configure Circuit Options for better error handling and stability
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    options.MaxBufferedUnacknowledgedRenderBatches = 10;
});

// ============================================================
// PERFORMANCE OPTIMIZATION: SignalR & WebSocket Settings
// ============================================================
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 128 * 1024; // 128KB - increased for better performance
    options.StreamBufferCapacity = 10;
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumParallelInvocationsPerClient = 1; // Prevent parallel invocations
});

// Configure Blazor Server options for better performance and stability
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.DisconnectedCircuitMaxRetained = 100;
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    options.MaxBufferedUnacknowledgedRenderBatches = 10;
}).AddHubOptions(options =>
{
    // Optimize SignalR Hub for better performance
    options.MaximumReceiveMessageSize = 32 * 1024; // 32 KB
    options.StreamBufferCapacity = 10;
    options.MaximumParallelInvocationsPerClient = 1;
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

// Add Circuit Handler for better error handling
builder.Services.AddScoped<CircuitHandler, SpirithubCafe.Web.Services.ErrorCircuitHandler>();

// Circuit error handling will be done via global exception handling

// Add MVC controllers for culture switching
builder.Services.AddControllers();

// Add localization services
builder.Services.AddLocalization();
// Old: builder.Services.AddSingleton<IStringLocalizer<SpirithubCafe.Langs.Resources>, StringLocalizer<SpirithubCafe.Langs.Resources>>();
// Now using TranslationLocalizer instead - already registered above
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
        CookieName = "SpirithubCafe.Culture"
    });
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Configure JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(secretKey);

var authBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    });

authBuilder.AddIdentityCookies();
authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "SpirithubCafe",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "SpirithubCafe",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Google Authentication
authBuilder.AddGoogle(googleOptions =>
{
    var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
    googleOptions.ClientId = googleAuthSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId not found in configuration.");
    googleOptions.ClientSecret = googleAuthSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found in configuration.");
    googleOptions.CallbackPath = "/signin-google";
    
    // Request additional scopes
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
    
    // Save tokens for later use
    googleOptions.SaveTokens = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ============================================================
// PERFORMANCE OPTIMIZATION: DbContext Configuration
// ============================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    
    // Disable change tracking for read-only queries (use AsNoTracking explicitly)
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    
    // Enable sensitive data logging only in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    
    // Configure connection pooling
    options.UseSqlite(connectionString, sqliteOptions =>
    {
        sqliteOptions.CommandTimeout(30);
    });
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Register email services
builder.Services.AddScoped<SpirithubCafe.Application.Services.IEmailService, SpirithubCafe.Application.Services.SmtpEmailService>();
builder.Services.AddScoped<IEmailSender<ApplicationUser>, SpirithubCafe.Web.Services.IdentityEmailSender>();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

// ============================================================
// PERFORMANCE OPTIMIZATION: Memory Cache with Size Limit
// ============================================================
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Limit cache size to prevent memory bloat
    options.CompactionPercentage = 0.25; // Remove 25% when limit reached
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // Scan every 5 minutes
});

// Register localization service
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

// Register translation service
builder.Services.AddScoped<ITranslationService, SpirithubCafe.Application.Services.TranslationService>();
builder.Services.AddScoped<LocalizationHelper>();
builder.Services.AddScoped<IStringLocalizer, TranslationLocalizer>();
builder.Services.AddScoped<TranslationLocalizer>();

// Register cart service
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ToastService>();

// Register chat service
builder.Services.AddScoped<IChatService, ChatService>();

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 64 * 1024; // 64KB
});

// Register admin services
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<RoleManagementService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ShippingService>();
builder.Services.AddScoped<ShippingMethodService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<HomePageSettingsService>();
builder.Services.AddScoped<PaymentGatewaySettingsService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IAboutUsService, AboutUsService>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<IDeliveryPolicyService, DeliveryPolicyService>();
builder.Services.AddScoped<IPrivacyPolicyService, PrivacyPolicyService>();
builder.Services.AddScoped<IRefundPolicyService, RefundPolicyService>();
builder.Services.AddScoped<ITermsConditionsService, TermsConditionsService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<SpirithubCafe.Application.Interfaces.IFooterService, SpirithubCafe.Application.Services.FooterService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<SpirithubCafe.Application.Services.ICheckoutService, SpirithubCafe.Application.Services.CheckoutService>();
builder.Services.AddScoped<SpirithubCafe.Application.Services.IAramexApiService, SpirithubCafe.Application.Services.AramexApiService>();
builder.Services.AddScoped<SpirithubCafe.Application.Services.IAramexRateService, SpirithubCafe.Application.Services.AramexRateService>();
builder.Services.AddScoped<SpirithubCafe.Application.Services.IPaymentService, SpirithubCafe.Application.Services.PaymentService>();
builder.Services.AddScoped<SpirithubCafe.Application.Services.IPaymentGatewayService, SpirithubCafe.Application.Services.PaymentGatewayService>();

// Register user login service
builder.Services.AddScoped<UserLoginService>();

// Register slide service
builder.Services.AddScoped<SpirithubCafe.Application.Services.ISlideService, SpirithubCafe.Infrastructure.Services.SlideService>();

// Register setting service
builder.Services.AddScoped<SpirithubCafe.Application.Services.ISettingService, SpirithubCafe.Infrastructure.Services.SettingService>();

// Register FAQ service
builder.Services.AddScoped<SpirithubCafe.Application.Services.IFAQService, SpirithubCafe.Infrastructure.Services.FAQService>();
builder.Services.AddScoped<SpirithubCafe.Application.Interfaces.IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

// Register Asset Versioning Service for CSS/JS cache busting
builder.Services.AddSingleton<AssetVersioningService>();

// ============================================================
// PERFORMANCE OPTIMIZATION: Data Preload Service
// ============================================================
builder.Services.AddSingleton<DataPreloadService>();

// Register API services
builder.Services.AddScoped<IAuthApiService, AuthApiService<ApplicationUser>>();
builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();
builder.Services.AddScoped<IProductApiService, ProductApiService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SpirithubCafe API", 
        Version = "v1",
        Description = "REST API for SpirithubCafe - Premium Coffee Shop",
        Contact = new OpenApiContact
        {
            Name = "SpirithubCafe",
            Email = "info@SpirithubCafe.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Only include API controllers in Swagger documentation
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Only include API routes that start with 'api/'
        return apiDesc.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) == true;
    });
});

// Register data seeder service
builder.Services.AddScoped<DataSeederService>();

var app = builder.Build();

// Use Forwarded Headers middleware (must be before other middleware)
app.UseForwardedHeaders();

// ============================================================
// PERFORMANCE OPTIMIZATION: Response Compression
// ============================================================
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpirithubCafe API v1");
        c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
        c.DocumentTitle = "SpirithubCafe API Documentation";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1);
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Add global exception handling middleware
app.UseMiddleware<SpirithubCafe.Web.Middleware.GlobalExceptionMiddleware>();

// Seed sample data when running in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeederService>();
    // Fire-and-forget during startup; ensure any exceptions bubble during development
    await seeder.SeedSampleDataAsync();
}

// ============================================================
// PERFORMANCE OPTIMIZATION: Preload critical data
// ============================================================
{
    using var scope = app.Services.CreateScope();
    var preloadService = scope.ServiceProvider.GetRequiredService<DataPreloadService>();
    // Preload translations and categories into cache on startup
    _ = Task.Run(async () =>
    {
        await Task.Delay(2000); // Wait 2 seconds after startup
        await preloadService.PreloadCriticalDataAsync();
    });
}

// Add localization middleware
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

// Enable WebSockets
app.UseWebSockets();

// ============================================================
// PERFORMANCE OPTIMIZATION: Static File Caching
// ============================================================
var cacheMaxAge = app.Environment.IsDevelopment() ? 3600 : 31536000; // 1 hour dev, 1 year prod
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cacheMaxAge}");
        ctx.Context.Response.Headers.Append("Vary", "Accept-Encoding");
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

// Map controllers for culture switching FIRST
app.MapControllerRoute(
    name: "culture",
    pattern: "Culture/{action=Index}/{id?}",
    defaults: new { controller = "Culture" });

app.MapControllers();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

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
    string adminEmail = "admin@SpirithubCafe.com";
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


