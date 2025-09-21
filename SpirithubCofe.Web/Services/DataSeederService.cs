using Microsoft.EntityFrameworkCore;
using SpirithubCofe.Domain.Entities;
using SpirithubCofe.Web.Data;

namespace SpirithubCofe.Web.Services;

/// <summary>
/// Service to seed sample data for categories and products
/// </summary>
public class DataSeederService
{
    private readonly ApplicationDbContext _context;

    public DataSeederService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed sample categories and products with bilingual content
    /// </summary>
    public async Task SeedSampleDataAsync()
    {
        // Check if we already have data
        if (await _context.Categories.AnyAsync())
        {
            return; // Data already exists
        }

        // Create Categories
        var categories = new List<Category>
        {
            new Category
            {
                Slug = "coffee-beans",
                Name = "Coffee Beans",
                NameAr = "حبوب القهوة",
                Description = "Premium coffee beans from around the world, carefully selected for exceptional flavor and aroma.",
                DescriptionAr = "حبوب قهوة فاخرة من جميع أنحاء العالم، مختارة بعناية للحصول على نكهة ورائحة استثنائية.",
                ImagePath = "/images/categories/coffee-beans.jpg",
                IsActive = true,
                DisplayOrder = 1
            },
            new Category
            {
                Slug = "ground-coffee",
                Name = "Ground Coffee",
                NameAr = "قهوة مطحونة",
                Description = "Freshly ground coffee ready for brewing, available in various grind sizes.",
                DescriptionAr = "قهوة مطحونة طازجة جاهزة للتحضير، متوفرة بأحجام طحن مختلفة.",
                ImagePath = "/images/categories/ground-coffee.jpg",
                IsActive = true,
                DisplayOrder = 2
            },
            new Category
            {
                Slug = "instant-coffee",
                Name = "Instant Coffee",
                NameAr = "قهوة سريعة",
                Description = "Quick and convenient instant coffee for busy lifestyles.",
                DescriptionAr = "قهوة سريعة ومريحة لأسلوب الحياة المزدحم.",
                ImagePath = "/images/categories/instant-coffee.jpg",
                IsActive = true,
                DisplayOrder = 3
            },
            new Category
            {
                Slug = "coffee-accessories",
                Name = "Coffee Accessories",
                NameAr = "إكسسوارات القهوة",
                Description = "Essential tools and accessories for the perfect coffee experience.",
                DescriptionAr = "أدوات وإكسسوارات أساسية لتجربة قهوة مثالية.",
                ImagePath = "/images/categories/accessories.jpg",
                IsActive = true,
                DisplayOrder = 4
            }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();

        // Create Products
        var coffeeBeansCat = categories.First(c => c.Slug == "coffee-beans");
        var groundCoffeeCat = categories.First(c => c.Slug == "ground-coffee");
        var instantCoffeeCat = categories.First(c => c.Slug == "instant-coffee");
        var accessoriesCat = categories.First(c => c.Slug == "coffee-accessories");

        var products = new List<Product>
        {
            // Coffee Beans
            new Product
            {
                Sku = "ETH-YIRG-001",
                Name = "Ethiopian Yirgacheffe",
                NameAr = "إثيوبيا يرجاتشيف",
                Description = "A bright, wine-like coffee with floral and citrus notes. This single-origin coffee from the Yirgacheffe region of Ethiopia offers a complex flavor profile that coffee enthusiasts adore.",
                DescriptionAr = "قهوة مشرقة تشبه النبيذ مع نكهات زهرية وحمضيات. هذه القهوة أحادية المنشأ من منطقة يرجاتشيف في إثيوبيا تقدم مذاقاً معقداً يعشقه محبو القهوة.",
                Notes = "Best enjoyed as pour-over or French press",
                NotesAr = "يُستمتع بها بشكل أفضل كصب أو فرنش بريس",
                AromaticProfile = "Floral, citrus, wine-like acidity",
                AromaticProfileAr = "زهري، حمضيات، حموضة تشبه النبيذ",
                Intensity = 6,
                Compatibility = "Compatible with all brewing methods",
                CompatibilityAr = "متوافقة مع جميع طرق التحضير",
                Uses = "Perfect for morning brew or afternoon coffee",
                UsesAr = "مثالية لقهوة الصباح أو قهوة بعد الظهر",
                CategoryId = coffeeBeansCat.Id,
                IsActive = true,
                DisplayOrder = 1
            },
            new Product
            {
                Sku = "BRA-SANT-001",
                Name = "Brazilian Santos",
                NameAr = "برازيلي سانتوس",
                Description = "A smooth, well-balanced coffee with chocolate and nutty undertones. This classic Brazilian coffee provides a rich, full-bodied experience.",
                DescriptionAr = "قهوة ناعمة ومتوازنة مع نكهات الشوكولاتة والمكسرات. هذه القهوة البرازيلية الكلاسيكية توفر تجربة غنية وممتلئة القوام.",
                Notes = "Excellent as espresso base",
                NotesAr = "ممتازة كقاعدة للإسبريسو",
                AromaticProfile = "Chocolate, nuts, caramel sweetness",
                AromaticProfileAr = "شوكولاتة، مكسرات، حلاوة الكراميل",
                Intensity = 7,
                Compatibility = "Ideal for espresso machines and moka pots",
                CompatibilityAr = "مثالية لآلات الإسبريسو وأباريق الموكا",
                Uses = "Great for espresso, cappuccino, and latte",
                UsesAr = "رائعة للإسبريسو والكابتشينو واللاتيه",
                CategoryId = coffeeBeansCat.Id,
                IsActive = true,
                DisplayOrder = 2
            },
            new Product
            {
                Sku = "COL-SUP-001",
                Name = "Colombian Supremo",
                NameAr = "كولومبي سوبريمو",
                Description = "Premium Colombian coffee known for its bright acidity and rich, full flavor. Grown in the high altitudes of the Colombian Andes.",
                DescriptionAr = "قهوة كولومبية فاخرة معروفة بحموضتها المشرقة ونكهتها الغنية والممتلئة. تُزرع في المرتفعات العالية لجبال الأنديز الكولومبية.",
                Notes = "High altitude grown for superior quality",
                NotesAr = "مزروعة في المرتفعات العالية للحصول على جودة فائقة",
                AromaticProfile = "Bright acidity, caramel, fruity notes",
                AromaticProfileAr = "حموضة مشرقة، كراميل، نكهات فاكهية",
                Intensity = 8,
                Compatibility = "Versatile for all brewing methods",
                CompatibilityAr = "متعددة الاستخدامات لجميع طرق التحضير",
                Uses = "Perfect for drip coffee and cold brew",
                UsesAr = "مثالية لقهوة التقطير والقهوة الباردة",
                CategoryId = coffeeBeansCat.Id,
                IsActive = false, // Out of stock
                DisplayOrder = 3
            },

            // Ground Coffee
            new Product
            {
                Sku = "HOUSE-BLD-001",
                Name = "House Blend Ground",
                NameAr = "خليط البيت المطحون",
                Description = "Our signature house blend, carefully crafted to provide a perfect balance of flavor and aroma. Medium roast with notes of chocolate and vanilla.",
                DescriptionAr = "خليطنا المميز للبيت، مُحضر بعناية لتوفير توازن مثالي من النكهة والرائحة. تحميص متوسط مع نكهات الشوكولاتة والفانيليا.",
                Notes = "Our most popular blend",
                NotesAr = "خليطنا الأكثر شعبية",
                AromaticProfile = "Balanced, chocolate, vanilla, smooth",
                AromaticProfileAr = "متوازن، شوكولاتة، فانيليا، ناعم",
                Intensity = 5,
                Compatibility = "Perfect for drip coffee makers and pour-over",
                CompatibilityAr = "مثالي لصانعات القهوة بالتقطير والصب",
                Uses = "Daily drinking, morning coffee",
                UsesAr = "للشرب اليومي، قهوة الصباح",
                CategoryId = groundCoffeeCat.Id,
                IsActive = true,
                DisplayOrder = 1
            },

            // Instant Coffee
            new Product
            {
                Sku = "INST-PREM-001",
                Name = "Premium Instant Coffee",
                NameAr = "قهوة سريعة فاخرة",
                Description = "High-quality instant coffee made from 100% Arabica beans. Just add hot water for a quick and delicious cup.",
                DescriptionAr = "قهوة سريعة عالية الجودة مصنوعة من 100% حبوب أرابيكا. فقط أضف الماء الساخن للحصول على كوب سريع ولذيذ.",
                Notes = "100% Arabica, freeze-dried",
                NotesAr = "100% أرابيكا، مجففة بالتجميد",
                AromaticProfile = "Rich, smooth, well-rounded",
                AromaticProfileAr = "غنية، ناعمة، متوازنة",
                Intensity = 4,
                Compatibility = "Just add hot water",
                CompatibilityAr = "فقط أضف الماء الساخن",
                Uses = "Quick coffee solution for busy days",
                UsesAr = "حل قهوة سريع للأيام المزدحمة",
                CategoryId = instantCoffeeCat.Id,
                IsActive = true,
                DisplayOrder = 1
            },

            // Accessories
            new Product
            {
                Sku = "ACC-GRIND-001",
                Name = "Burr Coffee Grinder",
                NameAr = "مطحنة قهوة بأقراص",
                Description = "Professional-grade burr grinder for consistent coffee grounds. Features multiple grind settings for different brewing methods.",
                DescriptionAr = "مطحنة أقراص بجودة احترافية للحصول على قهوة مطحونة متناسقة. تتميز بإعدادات طحن متعددة لطرق التحضير المختلفة.",
                Notes = "Stainless steel burrs, 15 grind settings",
                NotesAr = "أقراص من الستانلس ستيل، 15 إعداد طحن",
                AromaticProfile = "N/A - Equipment",
                AromaticProfileAr = "غير متاح - معدات",
                Intensity = null,
                Compatibility = "Compatible with all coffee beans",
                CompatibilityAr = "متوافقة مع جميع حبوب القهوة",
                Uses = "Essential for fresh coffee grinding",
                UsesAr = "أساسية لطحن القهوة الطازجة",
                CategoryId = accessoriesCat.Id,
                IsActive = true,
                IsDigital = false,
                DisplayOrder = 1
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Create Product Variants
        var variants = new List<ProductVariant>();
        
        foreach (var product in products.Where(p => p.CategoryId != accessoriesCat.Id))
        {
            // Add different weight variants for coffee products
            if (product.CategoryId == coffeeBeansCat.Id || product.CategoryId == groundCoffeeCat.Id)
            {
                variants.AddRange(new[]
                {
                    new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantSku = $"{product.Sku}-250G",
                        Weight = 250,
                        WeightUnit = "g",
                        Price = 15.500m,
                        DiscountPrice = product.Name.Contains("Ethiopian") ? 13.900m : null,
                        StockQuantity = product.IsActive ? 45 : 0,
                        IsDefault = true,
                        DisplayOrder = 1,
                        Length = 12,
                        Width = 8,
                        Height = 15
                    },
                    new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantSku = $"{product.Sku}-500G",
                        Weight = 500,
                        WeightUnit = "g",
                        Price = 28.000m,
                        DiscountPrice = product.Name.Contains("Brazilian") ? 25.500m : null,
                        StockQuantity = product.IsActive ? 23 : 0,
                        IsDefault = false,
                        DisplayOrder = 2,
                        Length = 15,
                        Width = 10,
                        Height = 20
                    },
                    new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantSku = $"{product.Sku}-1KG",
                        Weight = 1000,
                        WeightUnit = "g",
                        Price = 52.000m,
                        StockQuantity = product.IsActive ? 12 : 0,
                        IsDefault = false,
                        DisplayOrder = 3,
                        Length = 20,
                        Width = 12,
                        Height = 25
                    }
                });
            }
            else if (product.CategoryId == instantCoffeeCat.Id)
            {
                variants.AddRange(new[]
                {
                    new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantSku = $"{product.Sku}-100G",
                        Weight = 100,
                        WeightUnit = "g",
                        Price = 8.500m,
                        StockQuantity = 35,
                        IsDefault = true,
                        DisplayOrder = 1,
                        Length = 8,
                        Width = 8,
                        Height = 12
                    },
                    new ProductVariant
                    {
                        ProductId = product.Id,
                        VariantSku = $"{product.Sku}-200G",
                        Weight = 200,
                        WeightUnit = "g",
                        Price = 15.500m,
                        DiscountPrice = 14.200m,
                        StockQuantity = 20,
                        IsDefault = false,
                        DisplayOrder = 2,
                        Length = 10,
                        Width = 8,
                        Height = 15
                    }
                });
            }
        }

        // Add variant for accessories
        var grinderProduct = products.First(p => p.Sku == "ACC-GRIND-001");
        variants.Add(new ProductVariant
        {
            ProductId = grinderProduct.Id,
            VariantSku = "ACC-GRIND-001-UNIT",
            Weight = 2500,
            WeightUnit = "g",
            Price = 125.000m,
            DiscountPrice = 110.000m,
            StockQuantity = 8,
            IsDefault = true,
            DisplayOrder = 1,
            Length = 25,
            Width = 18,
            Height = 35
        });

        _context.ProductVariants.AddRange(variants);
        await _context.SaveChangesAsync();

        // Create sample reviews
        var reviews = new List<ProductReview>
        {
            new ProductReview
            {
                ProductId = products.First(p => p.Sku == "ETH-YIRG-001").Id,
                Rating = 5,
                Title = "Exceptional Coffee!",
                TitleAr = "قهوة استثنائية!",
                Content = "This Ethiopian coffee is absolutely amazing. The floral notes are so distinctive and the citrus finish is perfect. Will definitely buy again!",
                ContentAr = "هذه القهوة الإثيوبية مذهلة تماماً. النكهات الزهرية مميزة جداً والطعم الحمضي في النهاية مثالي. سأشتريها مرة أخرى بالتأكيد!",
                CustomerName = "Ahmed Al-Rashid",
                CustomerEmail = "ahmed@example.com",
                IsApproved = true,
                IsFeatured = true,
                ApprovedAt = DateTime.UtcNow.AddDays(-5)
            },
            new ProductReview
            {
                ProductId = products.First(p => p.Sku == "BRA-SANT-001").Id,
                Rating = 4,
                Title = "Great for Espresso",
                TitleAr = "رائعة للإسبريسو",
                Content = "Perfect beans for my morning espresso. Rich and smooth with great crema.",
                ContentAr = "حبوب مثالية لإسبريسو الصباح. غنية وناعمة مع كريما رائعة.",
                CustomerName = "Fatima Al-Zahra",
                CustomerEmail = "fatima@example.com",
                IsApproved = true,
                IsFeatured = false,
                ApprovedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ProductReview
            {
                ProductId = products.First(p => p.Sku == "HOUSE-BLD-001").Id,
                Rating = 5,
                Title = "Daily Go-To Coffee",
                TitleAr = "قهوة يومية مثالية",
                Content = "This is my daily coffee. Perfectly balanced and never disappoints.",
                ContentAr = "هذه قهوتي اليومية. متوازنة تماماً ولا تخيب أبداً.",
                CustomerName = "Mohammed Al-Hinai",
                CustomerEmail = "mohammed@example.com",
                IsApproved = true,
                IsFeatured = true,
                ApprovedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        _context.ProductReviews.AddRange(reviews);
        await _context.SaveChangesAsync();
    }
}