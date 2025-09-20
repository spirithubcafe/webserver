# Copilot Instructions for spirithubcofe

## Project Overview
- Online shop for a coffee shop in Oman.
- Built with **Blazor Server**.
- Database: **SQLite** (stored in `wwwroot/data/`).
- Styling: **TailwindCSS**.

## Features
- Category management
- Product listing
- Cart & Checkout
- Users & Roles
- Orders
- Shipping integrations:
  - NoOl Oman
  - Aramex
- Payment gateway:
  - Bank Muscat

## Requirements
- Provide **Web API endpoints** that return JSON:
  - `/api/categories`
  - `/api/products`
  - `/api/cart`
  - `/api/orders`
- Follow **Clean Architecture** principles:
  - Domain → Application → Infrastructure → UI (Blazor).
- Use **Entity Framework Core** for data access.
- Use **ASP.NET Core Identity** for users/roles.
- Store database in `wwwroot/data/shop.db`.

## Coding Style
- Use C# 12 features where applicable.
- Write minimal, clean, and commented code.
- Use async/await properly for all I/O operations.
- Prefer Dependency Injection over static classes.


# 0) Tech choices (sane defaults)

* **.NET**: 9 (works on 8 too)
* **DB**: SQLite (dev) → Postgres/MySQL (prod). Start with SQLite for speed.
* **Auth**: ASP.NET Core Identity (cookie for Blazor; JWT bearer for API if/when you need external clients).
* **Styling**: TailwindCSS (optional but recommended).
* **i18n**: English + Arabic (RTL) ready.
* **Currency**: OMR.
* **Integrations**: Abstractions for **Shipping** (Nool Oman, Aramex) & **Payment** (Bank Muscat), with stubs you can fill when you have credentials.

---

# 1) Prepare your Linux box

```bash
# Install/confirm dotnet
dotnet --info

# VS Code extensions (recommended)
#   - ms-dotnettools.csharp
#   - ms-azuretools.vscode-docker (optional)
#   - ritwickdey.liveserver (optional for static)
#   - formulahendry.dotnet-test-explorer (optional)

# EF Core CLI
dotnet tool install --global dotnet-ef
```

---

# 2) Create the solution & projects (Clean Architecture)

```bash
mkdir SpirithubCofe && cd SpirithubCofe
dotnet new sln -n SpirithubCofe

# Projects
dotnet new classlib -n SpirithubCofe.Domain
dotnet new classlib -n SpirithubCofe.Application
dotnet new classlib -n SpirithubCofe.Infrastructure
dotnet new blazorserver -n SpirithubCofe.Web -au Individual

# Add to solution
dotnet sln add SpirithubCofe.Domain/SpirithubCofe.Domain.csproj
dotnet sln add SpirithubCofe.Application/SpirithubCofe.Application.csproj
dotnet sln add SpirithubCofe.Infrastructure/SpirithubCofe.Infrastructure.csproj
dotnet sln add SpirithubCofe.Web/SpirithubCofe.Web.csproj

# References
dotnet add SpirithubCofe.Application reference SpirithubCofe.Domain
dotnet add SpirithubCofe.Infrastructure reference SpirithubCofe.Domain
dotnet add SpirithubCofe.Infrastructure reference SpirithubCofe.Application
dotnet add SpirithubCofe.Web reference SpirithubCofe.Application
dotnet add SpirithubCofe.Web reference SpirithubCofe.Infrastructure
```

---

# 3) Domain model (keep it lean first)

`SpirithubCofe.Domain/Entities.cs`

```csharp
namespace SpirithubCofe.Domain;

public class Category {
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string Name { get; set; } = "";
    public string? NameAr { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = [];
}

public class Product {
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Price { get; set; }      // OMR
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDigital { get; set; } = false;
}

public class Cart {
    public int Id { get; set; }
    public string UserId { get; set; } = ""; // Identity user
    public ICollection<CartItem> Items { get; set; } = [];
}

public class CartItem {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // snapshot
    public int CartId { get; set; }
}

public enum OrderStatus { Pending, Paid, Shipped, Delivered, Cancelled, Refunded }

public class Order {
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "OMR";
    public ICollection<OrderItem> Items { get; set; } = [];
    public ShippingAddress ShippingAddress { get; set; } = new();
    public string? PaymentReference { get; set; }     // Bank Muscat reference
    public string? ShippingProvider { get; set; }     // "nool" | "aramex"
    public string? TrackingNumber { get; set; }
}

public class OrderItem {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}

public class ShippingAddress {
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Line1 { get; set; } = "";
    public string? Line2 { get; set; }
    public string City { get; set; } = "";
    public string State { get; set; } = ""; // Governorate
    public string Country { get; set; } = "OM"; // Oman
    public string PostalCode { get; set; } = "";
}
```

---

# 4) Infrastructure (EF Core + Identity)

Install EF packages:

```bash
dotnet add SpirithubCofe.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add SpirithubCofe.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
dotnet add SpirithubCofe.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add SpirithubCofe.Web package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add SpirithubCofe.Web package Microsoft.EntityFrameworkCore.Sqlite
```

`SpirithubCofe.Infrastructure/AppDbContext.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain;

namespace SpirithubCofe.Infrastructure;

public class AppUser : IdentityUser { /* add profile fields later */ }
public class AppRole : IdentityRole { }

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingAddress> Addresses => Set<ShippingAddress>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Category>().HasIndex(x => x.Slug).IsUnique();
        b.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(18,3)"); // OMR precision
        b.Entity<Order>().Property(x => x.Subtotal).HasColumnType("decimal(18,3)");
        b.Entity<Order>().Property(x => x.ShippingCost).HasColumnType("decimal(18,3)");
        b.Entity<Order>().Property(x => x.Total).HasColumnType("decimal(18,3)");

        b.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Order>()
            .OwnsOne(x => x.ShippingAddress);
    }
}
```

---

# 5) Configure Web app (DB, Identity, Localization, APIs)

`SpirithubCofe.Web/appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=Data/spirithub.db"
  },
  "Shop": {
    "Currency": "OMR",
    "Brand": "SpirithubCofe"
  },
  "BankMuscat": {
    "MerchantId": "",
    "ApiKey": "",
    "Secret": "",
    "CallbackUrl": "https://yourdomain.com/api/payment/bankmuscat/callback"
  },
  "Shipping": {
    "Nool": { "ApiKey": "", "Account": "" },
    "Aramex": { "AccountNumber": "", "Username": "", "Password": "" }
  },
  "AllowedHosts": "*"
}
```

Create `Data/` folder:

```bash
mkdir -p SpirithubCofe.Web/Data
```

`SpirithubCofe.Web/Program.cs` (key parts)

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Infrastructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Blazor + Razor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Localization (en + ar)
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    var cultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };
    opt.SupportedCultures = cultures;
    opt.SupportedUICultures = cultures;
    opt.SetDefaultCulture("en");
});

// AuthN/Z for APIs
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

var app = builder.Build();

// Migrate & seed roles/admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    foreach (var r in new[] { "Admin", "Staff", "Customer" })
        if (!await roleMgr.RoleExistsAsync(r)) await roleMgr.CreateAsync(new AppRole { Name = r });

    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var adminEmail = "admin@spirithub.cofe";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userMgr.CreateAsync(admin, "Admin@12345");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}

// Static + localization
app.UseRequestLocalization();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();

// ====== Minimal JSON APIs under /api ======
var api = app.MapGroup("/api").RequireAuthorization();      // require login by default
MapCatalogApi(api);          // categories, products
MapCartApi(api);             // cart
MapCheckoutApi(api);         // checkout & orders
MapUserAdminApi(api);        // users/roles
MapShippingApi(api);         // shipping rates, labels
MapPaymentApi(api);          // bank muscat

app.MapFallbackToPage("/_Host");

app.Run();

// ---------- Endpoints (in same file or split to partial classes) ----------
static void MapCatalogApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/catalog").AllowAnonymous(); // browsing products is public
    g.MapGet("/categories", async (AppDbContext db) =>
        await db.Categories.Where(c=>c.IsActive).OrderBy(c=>c.Name).ToListAsync());

    g.MapGet("/products", async (AppDbContext db, int? categoryId, string? q) =>
        await db.Products
           .Where(p => p.IsActive
                   && (categoryId == null || p.CategoryId == categoryId)
                   && (string.IsNullOrEmpty(q) || EF.Functions.Like(p.Name, $"%{q}%")))
           .Select(p => new { p.Id, p.Name, p.Price, p.ImageUrl, p.CategoryId })
           .ToListAsync());

    g.MapGet("/products/{id:int}", async (AppDbContext db, int id) =>
        await db.Products.FindAsync(id) is { } p
           ? Results.Ok(p)
           : Results.NotFound());
}

static void MapCartApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/cart");
    g.MapGet("/", async (AppDbContext db, UserManager<AppUser> um, HttpContext ctx) =>
    {
        var uid = um.GetUserId(ctx.User)!;
        var cart = await db.Carts.Include(c=>c.Items).ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(c=>c.UserId==uid) ?? new() { UserId = uid };
        if (cart.Id == 0) { db.Carts.Add(cart); await db.SaveChangesAsync(); }
        return Results.Ok(cart);
    });

    g.MapPost("/items", async (AppDbContext db, UserManager<AppUser> um, HttpContext ctx, CartItemDto dto) =>
    {
        var uid = um.GetUserId(ctx.User)!;
        var cart = await db.Carts.Include(c=>c.Items).FirstAsync(c=>c.UserId==uid);
        var prod = await db.Products.FindAsync(dto.ProductId);
        if (prod is null || !prod.IsActive) return Results.BadRequest("Invalid product");

        var existing = cart.Items.FirstOrDefault(i=>i.ProductId==dto.ProductId);
        if (existing == null)
            cart.Items.Add(new() { ProductId = prod.Id, Quantity = dto.Quantity, UnitPrice = prod.Price });
        else
            existing.Quantity += dto.Quantity;

        await db.SaveChangesAsync();
        return Results.Ok(cart);
    });

    g.MapDelete("/items/{itemId:int}", async (AppDbContext db, int itemId, UserManager<AppUser> um, HttpContext ctx) =>
    {
        var uid = um.GetUserId(ctx.User)!;
        var item = await db.CartItems.Include(i=>i.Product)
            .Where(i=>i.Cart!.UserId==uid && i.Id==itemId).FirstOrDefaultAsync();
        if (item == null) return Results.NotFound();
        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    });
}

record CartItemDto(int ProductId, int Quantity);

static void MapCheckoutApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/checkout");
    g.MapPost("/create-order", async (AppDbContext db, UserManager<AppUser> um, HttpContext ctx, ShippingAddress addr) =>
    {
        var uid = um.GetUserId(ctx.User)!;
        var cart = await db.Carts.Include(c=>c.Items).ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(c=>c.UserId==uid);
        if (cart == null || cart.Items.Count==0) return Results.BadRequest("Cart is empty");

        var subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        var shipping = 0m; // you can compute via shipping provider
        var order = new SpirithubCofe.Domain.Order {
            UserId = uid, Subtotal = subtotal, ShippingCost = shipping, Total = subtotal + shipping,
            Items = cart.Items.Select(i => new SpirithubCofe.Domain.OrderItem {
                ProductId = i.ProductId, ProductName = i.Product!.Name, Quantity = i.Quantity, UnitPrice = i.UnitPrice
            }).ToList(),
            ShippingAddress = addr
        };
        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        await db.SaveChangesAsync();
        return Results.Ok(new { order.Id, order.Total, order.Currency });
    });
}

static void MapUserAdminApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/admin").RequireAuthorization("AdminOnly");
    g.MapGet("/users", async (UserManager<AppUser> um) =>
        (await um.Users.ToListAsync()).Select(u => new { u.Id, u.Email, u.UserName }));

    g.MapPost("/roles/{userId}/{role}", async (UserManager<AppUser> um, string userId, string role) =>
    {
        var u = await um.FindByIdAsync(userId);
        if (u == null) return Results.NotFound();
        await um.AddToRoleAsync(u, role);
        return Results.Ok();
    });
}

static void MapShippingApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/shipping");
    g.MapGet("/rates", (decimal weightKg, string city) =>
    {
        // TODO: call provider(s) and return the best prices
        return Results.Ok(new[] {
            new { provider = "nool",   service="standard", price= 1.500m },
            new { provider = "aramex", service="express",  price= 3.200m },
        });
    });

    g.MapPost("/create", (CreateShipmentDto dto) =>
    {
        // TODO: call chosen provider (Nool/Aramex), return label/tracking
        return Results.Ok(new { trackingNumber = "TRK123456", provider = dto.Provider });
    });
}

record CreateShipmentDto(string Provider, int OrderId);

static void MapPaymentApi(RouteGroupBuilder api)
{
    var g = api.MapGroup("/payment");
    g.MapPost("/bankmuscat/start", (int orderId) =>
    {
        // TODO: create payment session with Bank Muscat and return redirect URL
        var redirectUrl = $"https://bankmuscat.example/redirect?order={orderId}";
        return Results.Ok(new { redirectUrl });
    });

    g.MapPost("/bankmuscat/callback", async (AppDbContext db, BankMuscatCallback payload) =>
    {
        // TODO: verify signature, then set order.Status = Paid
        var order = await db.Orders.FindAsync(payload.OrderId);
        if (order == null) return Results.NotFound();
        order.Status = payload.Success ? SpirithubCofe.Domain.OrderStatus.Paid : SpirithubCofe.Domain.OrderStatus.Pending;
        order.PaymentReference = payload.Reference;
        await db.SaveChangesAsync();
        return Results.Ok();
    });
}

record BankMuscatCallback(int OrderId, bool Success, string Reference);
```

---

# 6) Initial migration & run

```bash
# from solution root
dotnet ef migrations add InitialCreate -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web
dotnet ef database update -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web

# run
dotnet run --project SpirithubCofe.Web
```

Open `https://localhost:5001` (or port shown). Use the seeded admin:
**[admin@spirithub.cofe](mailto:admin@spirithub.cofe) / Admin\@12345**

---

# 7) Seeding sample data (optional)

Create a one-time seeder (e.g., in `Program.cs` after migrate) to add categories/products.

```csharp
var sample = await db.Categories.AnyAsync();
if (!sample) {
    var cat = new SpirithubCofe.Domain.Category { Name="Coffee Beans", Slug="coffee-beans" };
    db.Categories.Add(cat);
    db.Products.AddRange(
      new SpirithubCofe.Domain.Product { Name="Ethiopian Yirgacheffe", Category=cat, Price=4.900m, ImageUrl="/images/yirg.jpg" },
      new SpirithubCofe.Domain.Product { Name="Brazil Santos", Category=cat, Price=3.700m, ImageUrl="/images/santos.jpg" }
    );
    await db.SaveChangesAsync();
}
```

---

# 8) Frontend (Blazor Server UI)

* Use the default Identity pages (Login/Register).
* Build pages:

  * `/catalog` (list categories/products, search)
  * `/product/{id}`
  * `/cart`
  * `/checkout`
  * `/orders` (history)
  * `/admin` (manage products, categories, orders, users/roles)

You can call your own JSON endpoints via `HttpClient` inside Blazor components.

---

# 9) Tailwind (optional but recommended)

```bash
cd SpirithubCofe.Web
npm init -y
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

`tailwind.config.js`

```js
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: { extend: {} },
  plugins: [],
}
```

`wwwroot/css/site.css` (add)

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Build CSS during dev:

```bash
npx tailwindcss -i ./wwwroot/css/site.css -o ./wwwroot/css/site.bundle.css --watch
```

Reference `site.bundle.css` in `_Host.cshtml`.

---

# 10) Shipping & Payment abstractions (ready for real APIs)

Create interfaces in **Application**:

`SpirithubCofe.Application/Payments.cs`

```csharp
namespace SpirithubCofe.Application;

public interface IPaymentGateway {
    Task<StartPaymentResult> StartPaymentAsync(int orderId, decimal amount, string currency, CancellationToken ct);
    Task<VerifyResult> VerifyCallbackAsync(IDictionary<string,string> form, CancellationToken ct);
}

public record StartPaymentResult(bool Ok, string? RedirectUrl, string? Error);
public record VerifyResult(bool Success, string Reference, string Raw);
```

`SpirithubCofe.Application/Shipping.cs`

```csharp
namespace SpirithubCofe.Application;

public interface IShippingProvider {
    Task<IEnumerable<ShippingQuote>> GetQuotesAsync(decimal weightKg, string city, CancellationToken ct);
    Task<CreateShipmentResult> CreateShipmentAsync(int orderId, CancellationToken ct);
    Task<TrackResult> TrackAsync(string trackingNumber, CancellationToken ct);
}

public record ShippingQuote(string Provider, string Service, decimal Price);
public record CreateShipmentResult(string TrackingNumber, string LabelUrl);
public record TrackResult(string Status, string? Location);
```

Implement stubs in **Infrastructure**:

`SpirithubCofe.Infrastructure/Payments/BankMuscatGateway.cs`

```csharp
using Microsoft.Extensions.Configuration;
using SpirithubCofe.Application;

namespace SpirithubCofe.Infrastructure.Payments;

public class BankMuscatGateway(IConfiguration cfg) : IPaymentGateway
{
    public Task<StartPaymentResult> StartPaymentAsync(int orderId, decimal amount, string currency, CancellationToken ct)
    {
        // TODO: call real API with merchant creds and signature
        var redirectUrl = $"https://bankmuscat.example/pay?order={orderId}&amount={amount}";
        return Task.FromResult(new StartPaymentResult(true, redirectUrl, null));
    }

    public Task<VerifyResult> VerifyCallbackAsync(IDictionary<string,string> form, CancellationToken ct)
    {
        // TODO: verify signature/hash, parse reference
        var reference = form.GetValueOrDefault("reference") ?? "N/A";
        return Task.FromResult(new VerifyResult(true, reference, string.Join("&", form.Select(kv=>$"{kv.Key}={kv.Value}"))));
    }
}
```

`SpirithubCofe.Infrastructure/Shipping/NoolProvider.cs`

```csharp
using SpirithubCofe.Application;

namespace SpirithubCofe.Infrastructure.Shipping;

public class NoolProvider : IShippingProvider
{
    public Task<IEnumerable<ShippingQuote>> GetQuotesAsync(decimal weightKg, string city, CancellationToken ct) =>
        Task.FromResult<IEnumerable<ShippingQuote>>([ new("nool","standard",1.500m) ]);

    public Task<CreateShipmentResult> CreateShipmentAsync(int orderId, CancellationToken ct) =>
        Task.FromResult(new CreateShipmentResult("NOOL123456", "https://label.example/nool/NOOL123456.pdf"));

    public Task<TrackResult> TrackAsync(string trackingNumber, CancellationToken ct) =>
        Task.FromResult(new TrackResult("In Transit", "Muscat Hub"));
}
```

`SpirithubCofe.Infrastructure/Shipping/AramexProvider.cs` (similar).

Register services in `Program.cs` (top, before `build()`):

```csharp
using SpirithubCofe.Application;
using SpirithubCofe.Infrastructure.Payments;
using SpirithubCofe.Infrastructure.Shipping;

builder.Services.AddScoped<IPaymentGateway, BankMuscatGateway>();
builder.Services.AddScoped<IShippingProvider, NoolProvider>(); // swap per user choice or compose both
```

Then update the API handlers to call these interfaces instead of returning hardcoded objects.

---

# 11) Security notes (Blazor Server + API)

* Your Blazor pages use **cookie auth** (Identity).
* Your `/api` is currently **cookie-protected**; perfect for same-site calls.
* If you want third-party/mobile to call the API, add a JWT bearer auth scheme and expose only the endpoints you need (keep admin cookie-only).

---

# 12) Best folder structure (final form)

```
SpirithubCofe/
  SpirithubCofe.sln
  src/
    SpirithubCofe.Domain/
      Entities.cs
      (domain events, value objects later)
    SpirithubCofe.Application/
      Payments.cs
      Shipping.cs
      (use cases, DTOs, validators)
    SpirithubCofe.Infrastructure/
      AppDbContext.cs
      Payments/BankMuscatGateway.cs
      Shipping/NoolProvider.cs
      Shipping/AramexProvider.cs
      (repos, external clients)
    SpirithubCofe.Web/
      Data/spirithub.db           # dev only; keep DB outside wwwroot
      Pages/                      # Blazor UI
      Areas/Identity/             # Identity UI
      wwwroot/                    # static files (images, css)
      Program.cs
      appsettings*.json
  tests/
    SpirithubCofe.Tests/          # unit/integration tests (optional)
```

---

# 13) Everyday commands

```bash
# run dev
dotnet watch --project SpirithubCofe.Web

# add migration
dotnet ef migrations add AddSomething -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web
dotnet ef database update -p SpirithubCofe.Infrastructure -s SpirithubCofe.Web
```

---

# 14) Next steps for SpirithubCofe

1. Hook **Bank Muscat** real API: replace the stub in `BankMuscatGateway` with their init & callback verification (HMAC/signature) and set the callback route you already mapped.
2. Implement **Nool Oman** and **Aramex** official API calls in providers (rates, create label, tracking).
3. Build your **Admin area**: CRUD for Category/Product/Orders/Users (restrict with `AdminOnly`).
4. Add **Arabic resources** and set RTL classes; price formatting in **OMR** with 3 decimals (as set).
5. Add **images upload** (local disk or S3/R2).
6. CI/CD (GitHub Actions) and swap SQLite → Postgres for production.

---

If you want, I can generate:

* a ready-to-run repo scaffold (all files laid out),
* admin CRUD Razor pages,
* and a basic Tailwind UI for catalog/cart/checkout.
