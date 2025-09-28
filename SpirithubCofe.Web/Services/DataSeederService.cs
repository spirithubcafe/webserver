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
        try
        {
            // Seed categories if they don't exist
            if (!await _context.Categories.AnyAsync())
            {
                await SeedCategoriesAsync();
            }

            // Seed GCC countries and cities if they don't exist (independent of categories)
            if (!await _context.Countries.AnyAsync())
            {
                await SeedGccCountriesAsync();
            }
        }
        catch (Exception ex)
        {
            // If Countries table doesn't exist, create it via migration
            if (ex.Message.Contains("no such table: Countries"))
            {
                // Create tables manually using raw SQL as fallback
                await CreateCountriesTableAsync();
                await CreateCitiesTableAsync();
                
                // Try seeding again
                if (!await _context.Countries.AnyAsync())
                {
                    await SeedGccCountriesAsync();
                }
            }
            else
            {
                throw;
            }
        }
    }

    private async Task CreateCountriesTableAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS Countries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Code TEXT NOT NULL,
                Name TEXT NOT NULL,
                NameAr TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1
            )");
        
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE UNIQUE INDEX IF NOT EXISTS IX_Countries_Code ON Countries (Code)");
        
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS IX_Countries_IsActive ON Countries (IsActive)");
    }

    private async Task CreateCitiesTableAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS Cities (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Code TEXT,
                Name TEXT NOT NULL,
                NameAr TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1,
                CountryId INTEGER NOT NULL,
                FOREIGN KEY (CountryId) REFERENCES Countries (Id) ON DELETE CASCADE
            )");
        
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS IX_Cities_CountryId ON Cities (CountryId)");
        
        await _context.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS IX_Cities_IsActive ON Cities (IsActive)");
    }

    private async Task SeedCategoriesAsync()
    {

        // Create Categories
        var categories = new List<Category>
        {
            new Category
            {
                Slug = "espresso-milk-based-coffee",
                Name = "Espresso & Milk-Based Coffee",
                NameAr = "قهوة الإسبريسو و الحليب",
                Description = "Espresso beans are coffee beans roasted specifically to suit the unique requirements of brewing espresso.",
                DescriptionAr = "حبوب الإسبريسو هي حبوب قهوة محمصة خصيصًا لتناسب المتطلبات الفريدة لتحضير الإسبريسو.",
                ImagePath = "/images/categories/specialty-coffee-beans-roastery-oman-spirithub-espresso-coffee.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 1
            },
            new Category
            {
                Slug = "filter-pour-over-coffee",
                Name = "Filter & Pour-Over Coffee",
                NameAr = "القهوة المقطرة بالترشيح",
                Description = "Filter coffee, also known as drip coffee, is a method of brewing coffee where hot water is poured over ground coffee.",
                DescriptionAr = "القهوة المقطرة، والمعروفة أيضاً بالقهوة المنقطة، هي طريقة لتحضير القهوة حيث يتم سكب الماء الساخن على القهوة المطحونة.",
                ImagePath = "/images/categories/specialty-coffee-beans-roastery-oman-spirithub-filter-coffee.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 2
            },
            new Category
            {
                Slug = "ufo-drip-coffee-filters",
                Name = "UFO Drip Coffee Filters",
                NameAr = "فلاتر قهوة UFO التنقيط",
                Description = "Single-serve UFO drip coffee filters for convenient brewing.",
                DescriptionAr = "فلاتر قهوة UFO التنقيط لتحضير مريح للقهوة الفردية.",
                ImagePath = "/images/categories/ufo-drip-coffee-filters.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 3
            },
            new Category
            {
                Slug = "specialty-coffee-capsules",
                Name = "SpiritHub Coffee Capsules",
                NameAr = "مجموعة كبسولات القهوة",
                Description = "Indulge in a classic espresso experience with our wide selection of capsules each offering its own unique flavor profile and rich aroma.",
                DescriptionAr = "استمتع بتجربة الإسبريسو الكلاسيكية مع مجموعة واسعة من الكبسولات، حيث يتميز كل نوع بطابعه الخاص ونكهته العطرية الفريدة.",
                ImagePath = "/images/categories/specialty-coffee-capsules.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 4
            },
            new Category
            {
                Slug = "competition-premium-series",
                Name = "Competition Premium Series",
                NameAr = "مجموعة المنافسة المميزة",
                Description = "Our premium collection of competition-grade coffees for serious coffee enthusiasts.",
                DescriptionAr = "مجموعتنا الفاخرة من قهوات درجة المنافسة لعشاق القهوة الجادين.",
                ImagePath = "/images/categories/competition-premium-series.webp",
                IsActive = true,
                IsDisplayedOnHomepage = true,
                DisplayOrder = 5
            },
            new Category
            {
                Slug = "merchandise",
                Name = "SpiritHub Merchandise",
                NameAr = "منتجات سبيريت هب",
                Description = "Official SpiritHub merchandise and accessories.",
                DescriptionAr = "المنتجات والإكسسوارات الرسمية لسبيريت هب.",
                ImagePath = "/images/categories/merchandise.webp",
                IsActive = true,
                IsDisplayedOnHomepage = false,
                DisplayOrder = 6
            }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();
    }

    private async Task SeedGccCountriesAsync()
    {
            var gccCountries = new List<Country>
            {
                new Country
                {
                    Code = "AE",
                    Name = "United Arab Emirates",
                    NameAr = "الإمارات العربية المتحدة",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Abu Dhabi Emirate
                        new City { Name = "Abu Dhabi", NameAr = "أبو ظبي", Code = "AUH", IsActive = true },
                        new City { Name = "Al Ain", NameAr = "العين", IsActive = true },
                        new City { Name = "Madinat Zayed", NameAr = "مدينة زايد", IsActive = true },
                        new City { Name = "Ruwais", NameAr = "الرويس", IsActive = true },
                        new City { Name = "Liwa", NameAr = "ليوا", IsActive = true },
                        new City { Name = "Ghayathi", NameAr = "الغياثي", IsActive = true },
                        new City { Name = "Mirfa", NameAr = "مرفأ", IsActive = true },
                        new City { Name = "Sila", NameAr = "السلع", IsActive = true },
                        
                        // Dubai Emirate
                        new City { Name = "Dubai", NameAr = "دبي", Code = "DXB", IsActive = true },
                        new City { Name = "Deira", NameAr = "ديرة", IsActive = true },
                        new City { Name = "Bur Dubai", NameAr = "بر دبي", IsActive = true },
                        new City { Name = "Jumeirah", NameAr = "جميرا", IsActive = true },
                        new City { Name = "Dubai Marina", NameAr = "مرسى دبي", IsActive = true },
                        new City { Name = "Downtown Dubai", NameAr = "وسط مدينة دبي", IsActive = true },
                        new City { Name = "Business Bay", NameAr = "الخليج التجاري", IsActive = true },
                        new City { Name = "Hatta", NameAr = "حتا", IsActive = true },
                        
                        // Sharjah Emirate
                        new City { Name = "Sharjah", NameAr = "الشارقة", IsActive = true },
                        new City { Name = "Khor Fakkan", NameAr = "خورفكان", IsActive = true },
                        new City { Name = "Kalba", NameAr = "كلباء", IsActive = true },
                        new City { Name = "Dibba Al Hisn", NameAr = "دبا الحصن", IsActive = true },
                        new City { Name = "Mleiha", NameAr = "مليحة", IsActive = true },
                        
                        // Ajman Emirate
                        new City { Name = "Ajman", NameAr = "عجمان", IsActive = true },
                        new City { Name = "Manama", NameAr = "المنامة", IsActive = true },
                        new City { Name = "Masfoot", NameAr = "مصفوت", IsActive = true },
                        
                        // Ras Al Khaimah Emirate
                        new City { Name = "Ras Al Khaimah", NameAr = "رأس الخيمة", IsActive = true },
                        new City { Name = "Julfar", NameAr = "جلفار", IsActive = true },
                        new City { Name = "Digdaga", NameAr = "دقداقة", IsActive = true },
                        
                        // Fujairah Emirate
                        new City { Name = "Fujairah", NameAr = "الفجيرة", IsActive = true },
                        new City { Name = "Dibba Al Fujairah", NameAr = "دبا الفجيرة", IsActive = true },
                        new City { Name = "Bidiyah", NameAr = "البدية", IsActive = true },
                        new City { Name = "Masafi", NameAr = "مسافي", IsActive = true },
                        
                        // Umm Al Quwain Emirate
                        new City { Name = "Umm Al Quwain", NameAr = "أم القيوين", IsActive = true },
                        new City { Name = "Falaj Al Mualla", NameAr = "فلج المعلا", IsActive = true }
                    }
                },
                new Country
                {
                    Code = "SA",
                    Name = "Saudi Arabia",
                    NameAr = "المملكة العربية السعودية",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Riyadh Province
                        new City { Name = "Riyadh", NameAr = "الرياض", Code = "RUH", IsActive = true },
                        new City { Name = "Al Kharj", NameAr = "الخرج", IsActive = true },
                        new City { Name = "Dawadmi", NameAr = "الدوادمي", IsActive = true },
                        new City { Name = "Al Majmaah", NameAr = "المجمعة", IsActive = true },
                        new City { Name = "Al Quwayiyah", NameAr = "القويعية", IsActive = true },
                        new City { Name = "Afif", NameAr = "عفيف", IsActive = true },
                        new City { Name = "Al Zulfi", NameAr = "الزلفي", IsActive = true },
                        new City { Name = "Shaqra", NameAr = "شقراء", IsActive = true },
                        
                        // Makkah Province
                        new City { Name = "Mecca", NameAr = "مكة المكرمة", IsActive = true },
                        new City { Name = "Jeddah", NameAr = "جدة", Code = "JED", IsActive = true },
                        new City { Name = "Taif", NameAr = "الطائف", IsActive = true },
                        new City { Name = "Rabigh", NameAr = "رابغ", IsActive = true },
                        new City { Name = "Yanbu", NameAr = "ينبع", IsActive = true },
                        new City { Name = "Al Qunfudhah", NameAr = "القنفذة", IsActive = true },
                        new City { Name = "Al Lith", NameAr = "الليث", IsActive = true },
                        new City { Name = "Thuwal", NameAr = "ثول", IsActive = true },
                        new City { Name = "Khulais", NameAr = "خليص", IsActive = true },
                        
                        // Medina Province
                        new City { Name = "Medina", NameAr = "المدينة المنورة", Code = "MED", IsActive = true },
                        new City { Name = "Yanbu Al Bahr", NameAr = "ينبع البحر", IsActive = true },
                        new City { Name = "Al Ula", NameAr = "العلا", IsActive = true },
                        new City { Name = "Khaybar", NameAr = "خيبر", IsActive = true },
                        new City { Name = "Wadi Al Fara", NameAr = "وادي الفرع", IsActive = true },
                        new City { Name = "Al Mahd", NameAr = "المهد", IsActive = true },
                        
                        // Eastern Province
                        new City { Name = "Dammam", NameAr = "الدمام", Code = "DMM", IsActive = true },
                        new City { Name = "Khobar", NameAr = "الخبر", IsActive = true },
                        new City { Name = "Dhahran", NameAr = "الظهران", IsActive = true },
                        new City { Name = "Al Jubail", NameAr = "الجبيل", IsActive = true },
                        new City { Name = "Al Ahsa", NameAr = "الأحساء", IsActive = true },
                        new City { Name = "Hofuf", NameAr = "الهفوف", IsActive = true },
                        new City { Name = "Qatif", NameAr = "القطيف", IsActive = true },
                        new City { Name = "Ras Tanura", NameAr = "رأس تنورة", IsActive = true },
                        new City { Name = "Khafji", NameAr = "الخفجي", IsActive = true },
                        new City { Name = "Abqaiq", NameAr = "بقيق", IsActive = true },
                        
                        // Asir Province
                        new City { Name = "Abha", NameAr = "أبها", IsActive = true },
                        new City { Name = "Khamis Mushait", NameAr = "خميس مشيط", IsActive = true },
                        new City { Name = "Najran", NameAr = "نجران", IsActive = true },
                        new City { Name = "Bisha", NameAr = "بيشة", IsActive = true },
                        new City { Name = "Mahayel", NameAr = "محايل", IsActive = true },
                        
                        // Tabuk Province
                        new City { Name = "Tabuk", NameAr = "تبوك", IsActive = true },
                        new City { Name = "NEOM", NameAr = "نيوم", IsActive = true },
                        new City { Name = "Duba", NameAr = "ضباء", IsActive = true },
                        new City { Name = "Tayma", NameAr = "تيماء", IsActive = true },
                        new City { Name = "Al Wajh", NameAr = "الوجه", IsActive = true },
                        
                        // Hail Province
                        new City { Name = "Hail", NameAr = "حائل", IsActive = true },
                        new City { Name = "Baqaa", NameAr = "بقعاء", IsActive = true },
                        
                        // Northern Borders Province
                        new City { Name = "Arar", NameAr = "عرعر", IsActive = true },
                        new City { Name = "Rafha", NameAr = "رفحاء", IsActive = true },
                        new City { Name = "Turaif", NameAr = "طريف", IsActive = true },
                        
                        // Jazan Province
                        new City { Name = "Jazan", NameAr = "جازان", IsActive = true },
                        new City { Name = "Sabya", NameAr = "صبيا", IsActive = true },
                        new City { Name = "Abu Arish", NameAr = "أبو عريش", IsActive = true },
                        
                        // Al Qassim Province
                        new City { Name = "Buraydah", NameAr = "بريدة", IsActive = true },
                        new City { Name = "Unaizah", NameAr = "عنيزة", IsActive = true },
                        new City { Name = "Ar Rass", NameAr = "الرس", IsActive = true },
                        
                        // Al Bahah Province
                        new City { Name = "Al Bahah", NameAr = "الباحة", IsActive = true },
                        new City { Name = "Baljurashi", NameAr = "بلجرشي", IsActive = true },
                        
                        // Al Jouf Province
                        new City { Name = "Sakaka", NameAr = "سكاكا", IsActive = true },
                        new City { Name = "Qurayyat", NameAr = "القريات", IsActive = true }
                    }
                },
                new Country
                {
                    Code = "KW",
                    Name = "Kuwait",
                    NameAr = "الكويت",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Capital Governorate
                        new City { Name = "Kuwait City", NameAr = "مدينة الكويت", Code = "KWI", IsActive = true },
                        new City { Name = "Dasman", NameAr = "دسمان", IsActive = true },
                        new City { Name = "Sharq", NameAr = "شرق", IsActive = true },
                        new City { Name = "Mirqab", NameAr = "المرقاب", IsActive = true },
                        new City { Name = "Jibla", NameAr = "قبلة", IsActive = true },
                        new City { Name = "Kaifan", NameAr = "كيفان", IsActive = true },
                        
                        // Hawalli Governorate
                        new City { Name = "Hawalli", NameAr = "حولي", IsActive = true },
                        new City { Name = "Salmiya", NameAr = "السالمية", IsActive = true },
                        new City { Name = "Jabriya", NameAr = "الجابرية", IsActive = true },
                        new City { Name = "Maidan Hawalli", NameAr = "ميدان حولي", IsActive = true },
                        new City { Name = "Bayan", NameAr = "بيان", IsActive = true },
                        new City { Name = "Mishref", NameAr = "مشرف", IsActive = true },
                        new City { Name = "Salwa", NameAr = "سلوى", IsActive = true },
                        new City { Name = "Rumaithiya", NameAr = "الرميثية", IsActive = true },
                        
                        // Farwaniya Governorate
                        new City { Name = "Farwaniya", NameAr = "الفروانية", IsActive = true },
                        new City { Name = "Jleeb Al-Shuyoukh", NameAr = "جليب الشيوخ", IsActive = true },
                        new City { Name = "Abraq Khaitan", NameAr = "أبرق خيطان", IsActive = true },
                        new City { Name = "Firdous", NameAr = "الفردوس", IsActive = true },
                        new City { Name = "Andalous", NameAr = "الأندلس", IsActive = true },
                        new City { Name = "Ardhiya", NameAr = "العارضية", IsActive = true },
                        new City { Name = "Rabiya", NameAr = "الرابية", IsActive = true },
                        
                        // Ahmadi Governorate
                        new City { Name = "Ahmadi", NameAr = "الأحمدي", IsActive = true },
                        new City { Name = "Fahaheel", NameAr = "الفحيحيل", IsActive = true },
                        new City { Name = "Fintas", NameAr = "الفنطاس", IsActive = true },
                        new City { Name = "Mangaf", NameAr = "المنقف", IsActive = true },
                        new City { Name = "Abu Halifa", NameAr = "أبو حليفة", IsActive = true },
                        new City { Name = "Sabah Al Salem", NameAr = "صباح السالم", IsActive = true },
                        new City { Name = "Al Rigga", NameAr = "الرقة", IsActive = true },
                        
                        // Jahra Governorate
                        new City { Name = "Jahra", NameAr = "الجهراء", IsActive = true },
                        new City { Name = "Sulaibiya", NameAr = "الصليبية", IsActive = true },
                        new City { Name = "Naeem", NameAr = "النعيم", IsActive = true },
                        new City { Name = "Qasr", NameAr = "القصر", IsActive = true },
                        new City { Name = "Taima", NameAr = "تيماء", IsActive = true },
                        
                        // Mubarak Al-Kabeer Governorate
                        new City { Name = "Mubarak Al-Kabeer", NameAr = "مبارك الكبير", IsActive = true },
                        new City { Name = "Qurain", NameAr = "القرين", IsActive = true },
                        new City { Name = "Adan", NameAr = "العدان", IsActive = true },
                        new City { Name = "Qusour", NameAr = "القصور", IsActive = true },
                        new City { Name = "Sabah Al-Ahmad", NameAr = "صباح الأحمد", IsActive = true }
                    }
                },
                new Country
                {
                    Code = "OM",
                    Name = "Oman",
                    NameAr = "عُمان",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Muscat Governorate
                        new City { Name = "Muscat", NameAr = "مسقط", Code = "MCT", IsActive = true },
                        new City { Name = "Muttrah", NameAr = "مطرح", IsActive = true },
                        new City { Name = "Ruwi", NameAr = "روي", IsActive = true },
                        new City { Name = "Seeb", NameAr = "السيب", IsActive = true },
                        new City { Name = "Bausher", NameAr = "بوشر", IsActive = true },
                        new City { Name = "Al Amerat", NameAr = "العامرات", IsActive = true },
                        new City { Name = "Quriyat", NameAr = "قريات", IsActive = true },
                        
                        // Dhofar Governorate
                        new City { Name = "Salalah", NameAr = "صلالة", Code = "SLL", IsActive = true },
                        new City { Name = "Taqah", NameAr = "طاقة", IsActive = true },
                        new City { Name = "Mirbat", NameAr = "مرباط", IsActive = true },
                        new City { Name = "Sadah", NameAr = "سدح", IsActive = true },
                        new City { Name = "Thumrait", NameAr = "ثمريت", IsActive = true },
                        
                        // Al Batinah North Governorate
                        new City { Name = "Sohar", NameAr = "صحار", IsActive = true },
                        new City { Name = "Shinas", NameAr = "شناص", IsActive = true },
                        new City { Name = "Liwa", NameAr = "لوى", IsActive = true },
                        new City { Name = "Saham", NameAr = "صحم", IsActive = true },
                        
                        // Al Batinah South Governorate
                        new City { Name = "Rustaq", NameAr = "الرستاق", IsActive = true },
                        new City { Name = "Nakhal", NameAr = "نخل", IsActive = true },
                        new City { Name = "Wadi Al Maawil", NameAr = "وادي المعاول", IsActive = true },
                        new City { Name = "Awabi", NameAr = "العوابي", IsActive = true },
                        new City { Name = "Al Musanaah", NameAr = "المصنعة", IsActive = true },
                        new City { Name = "Barka", NameAr = "بركاء", IsActive = true },
                        
                        // Ad Dakhiliyah Governorate
                        new City { Name = "Nizwa", NameAr = "نزوى", IsActive = true },
                        new City { Name = "Bahla", NameAr = "بهلاء", IsActive = true },
                        new City { Name = "Adam", NameAr = "أدم", IsActive = true },
                        new City { Name = "Al Hamra", NameAr = "الحمراء", IsActive = true },
                        new City { Name = "Manah", NameAr = "منح", IsActive = true },
                        new City { Name = "Izki", NameAr = "إزكي", IsActive = true },
                        new City { Name = "Samayil", NameAr = "سمائل", IsActive = true },
                        new City { Name = "Bidbid", NameAr = "بدبد", IsActive = true },
                        
                        // Ash Sharqiyah North Governorate
                        new City { Name = "Ibra", NameAr = "إبراء", IsActive = true },
                        new City { Name = "Al Mudaybi", NameAr = "المضيبي", IsActive = true },
                        new City { Name = "Bidiya", NameAr = "بدية", IsActive = true },
                        new City { Name = "Dima W At Taiyyin", NameAr = "دماء والطائيين", IsActive = true },
                        
                        // Ash Sharqiyah South Governorate
                        new City { Name = "Sur", NameAr = "صور", IsActive = true },
                        new City { Name = "Al Kamil W Al Wafi", NameAr = "الكامل والوافي", IsActive = true },
                        new City { Name = "Jaalan Bani Bu Hassan", NameAr = "جعلان بني بو حسن", IsActive = true },
                        new City { Name = "Jaalan Bani Bu Ali", NameAr = "جعلان بني بو علي", IsActive = true },
                        new City { Name = "Masirah", NameAr = "مصيرة", IsActive = true },
                        
                        // Ad Dhahirah Governorate
                        new City { Name = "Ibri", NameAr = "عبري", IsActive = true },
                        new City { Name = "Yanqul", NameAr = "ينقل", IsActive = true },
                        new City { Name = "Dhank", NameAr = "ضنك", IsActive = true },
                        
                        // Al Buraimi Governorate
                        new City { Name = "Al Buraimi", NameAr = "البريمي", IsActive = true },
                        new City { Name = "Mahadah", NameAr = "محضة", IsActive = true },
                        new City { Name = "Al Sunaynah", NameAr = "السنينة", IsActive = true },
                        
                        // Al Wusta Governorate
                        new City { Name = "Haima", NameAr = "هيماء", IsActive = true },
                        new City { Name = "Al Duqm", NameAr = "الدقم", IsActive = true },
                        new City { Name = "Mahout", NameAr = "محوت", IsActive = true },
                        
                        // Musandam Governorate
                        new City { Name = "Khasab", NameAr = "خصب", IsActive = true },
                        new City { Name = "Bukha", NameAr = "بخاء", IsActive = true },
                        new City { Name = "Daba", NameAr = "دبا", IsActive = true },
                        new City { Name = "Madha", NameAr = "مدحاء", IsActive = true }
                    }
                },
                new Country
                {
                    Code = "QA",
                    Name = "Qatar",
                    NameAr = "قطر",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Doha Municipality
                        new City { Name = "Doha", NameAr = "الدوحة", Code = "DOH", IsActive = true },
                        new City { Name = "West Bay", NameAr = "الخليج الغربي", IsActive = true },
                        new City { Name = "Al Sadd", NameAr = "السد", IsActive = true },
                        new City { Name = "Al Mansoura", NameAr = "المنصورة", IsActive = true },
                        new City { Name = "Bin Mahmoud", NameAr = "بن محمود", IsActive = true },
                        new City { Name = "Al Najma", NameAr = "النجمة", IsActive = true },
                        new City { Name = "Mushayrib", NameAr = "مشيرب", IsActive = true },
                        new City { Name = "Al Bidda", NameAr = "البدع", IsActive = true },
                        new City { Name = "Fereej Abdul Aziz", NameAr = "فريج عبدالعزيز", IsActive = true },
                        
                        // Al Rayyan Municipality
                        new City { Name = "Al Rayyan", NameAr = "الريان", IsActive = true },
                        new City { Name = "Education City", NameAr = "المدينة التعليمية", IsActive = true },
                        new City { Name = "Al Gharafa", NameAr = "الغرافة", IsActive = true },
                        new City { Name = "Al Waab", NameAr = "الواب", IsActive = true },
                        new City { Name = "Abu Hamour", NameAr = "أبو هامور", IsActive = true },
                        new City { Name = "Madinat Khalifa", NameAr = "مدينة خليفة", IsActive = true },
                        new City { Name = "Al Aziziyah", NameAr = "العزيزية", IsActive = true },
                        new City { Name = "Lusail", NameAr = "لوسيل", IsActive = true },
                        new City { Name = "Al Daayen", NameAr = "الدعين", IsActive = true },
                        
                        // Al Wakrah Municipality
                        new City { Name = "Al Wakrah", NameAr = "الوكرة", IsActive = true },
                        new City { Name = "Al Wukair", NameAr = "الوكير", IsActive = true },
                        new City { Name = "Mesaieed", NameAr = "مسيعيد", IsActive = true },
                        new City { Name = "Al Khor", NameAr = "الخور", IsActive = true },
                        new City { Name = "Al Thakira", NameAr = "الذخيرة", IsActive = true },
                        
                        // Umm Salal Municipality
                        new City { Name = "Umm Salal", NameAr = "أم صلال", IsActive = true },
                        new City { Name = "Umm Salal Mohammed", NameAr = "أم صلال محمد", IsActive = true },
                        new City { Name = "Umm Salal Ali", NameAr = "أم صلال علي", IsActive = true },
                        
                        // Al Shamal Municipality
                        new City { Name = "Al Ruwais", NameAr = "الرويس", IsActive = true },
                        new City { Name = "Madinat Ash Shamal", NameAr = "مدينة الشمال", IsActive = true },
                        
                        // Al Shahaniya Municipality
                        new City { Name = "Al Shahaniya", NameAr = "الشحانية", IsActive = true },
                        new City { Name = "Dukhan", NameAr = "دخان", IsActive = true },
                        
                        // Al Daayen Municipality
                        new City { Name = "Simaisma", NameAr = "سميسمة", IsActive = true },
                        new City { Name = "Al Kheesa", NameAr = "الخيسة", IsActive = true }
                    }
                },
                new Country
                {
                    Code = "BH",
                    Name = "Bahrain",
                    NameAr = "البحرين",
                    IsActive = true,
                    Cities = new List<City>
                    {
                        // Capital Governorate
                        new City { Name = "Manama", NameAr = "المنامة", Code = "BAH", IsActive = true },
                        new City { Name = "Juffair", NameAr = "الجفير", IsActive = true },
                        new City { Name = "Adliya", NameAr = "العدلية", IsActive = true },
                        new City { Name = "Gudaibiya", NameAr = "القضيبية", IsActive = true },
                        new City { Name = "Hoora", NameAr = "الحورة", IsActive = true },
                        new City { Name = "Zinj", NameAr = "الزنج", IsActive = true },
                        new City { Name = "Diplomatic Area", NameAr = "المنطقة الدبلوماسية", IsActive = true },
                        new City { Name = "Seef", NameAr = "السيف", IsActive = true },
                        
                        // Muharraq Governorate
                        new City { Name = "Muharraq", NameAr = "المحرق", IsActive = true },
                        new City { Name = "Busaiteen", NameAr = "البسيتين", IsActive = true },
                        new City { Name = "Hidd", NameAr = "الحد", IsActive = true },
                        new City { Name = "Dair", NameAr = "الدير", IsActive = true },
                        new City { Name = "Qalali", NameAr = "قلالي", IsActive = true },
                        new City { Name = "Arad", NameAr = "عراد", IsActive = true },
                        new City { Name = "Samaheej", NameAr = "السماهيج", IsActive = true },
                        
                        // Northern Governorate
                        new City { Name = "Hamad Town", NameAr = "مدينة حمد", IsActive = true },
                        new City { Name = "A'ali", NameAr = "عالي", IsActive = true },
                        new City { Name = "Janabiya", NameAr = "الجنبية", IsActive = true },
                        new City { Name = "Budaiya", NameAr = "البديع", IsActive = true },
                        new City { Name = "Bani Jamra", NameAr = "بني جمرة", IsActive = true },
                        new City { Name = "Tubli", NameAr = "توبلي", IsActive = true },
                        new City { Name = "Saar", NameAr = "سار", IsActive = true },
                        new City { Name = "Diraz", NameAr = "الدراز", IsActive = true },
                        
                        // Southern Governorate
                        new City { Name = "Riffa", NameAr = "الرفاع", IsActive = true },
                        new City { Name = "East Riffa", NameAr = "الرفاع الشرقي", IsActive = true },
                        new City { Name = "West Riffa", NameAr = "الرفاع الغربي", IsActive = true },
                        new City { Name = "Isa Town", NameAr = "مدينة عيسى", IsActive = true },
                        new City { Name = "Sitra", NameAr = "سترة", IsActive = true },
                        new City { Name = "Jaww", NameAr = "الجو", IsActive = true },
                        new City { Name = "Zallaq", NameAr = "الزلاق", IsActive = true },
                        new City { Name = "Awali", NameAr = "عوالي", IsActive = true },
                        new City { Name = "Dur", NameAr = "الدور", IsActive = true },
                        new City { Name = "Askar", NameAr = "عسكر", IsActive = true }
                    }
                }
            };

        _context.Countries.AddRange(gccCountries);
        await _context.SaveChangesAsync();
    }
}