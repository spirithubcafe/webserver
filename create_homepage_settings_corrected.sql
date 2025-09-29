-- Create HomePageSettings table with correct field names
CREATE TABLE "HomePageSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HomePageSettings" PRIMARY KEY AUTOINCREMENT,
    "ShowSlideshow" INTEGER NOT NULL DEFAULT 1,
    "ShowCategories" INTEGER NOT NULL DEFAULT 1,
    "CategoriesTitle" TEXT,
    "CategoriesTitleAr" TEXT,
    "CategoriesSubtitle" TEXT,
    "CategoriesSubtitleAr" TEXT,
    "CategoriesDisplayCount" INTEGER NOT NULL DEFAULT 8,
    "CategoriesBgType" TEXT DEFAULT 'color',
    "CategoriesBgValue" TEXT DEFAULT '#f8f9fa',
    "ShowMission" INTEGER NOT NULL DEFAULT 1,
    "MissionTitle" TEXT,
    "MissionTitleAr" TEXT,
    "MissionSubtitle" TEXT,
    "MissionSubtitleAr" TEXT,
    "MissionText" TEXT,
    "MissionTextAr" TEXT,
    "MissionBgType" TEXT DEFAULT 'color',
    "MissionBgValue" TEXT DEFAULT '#ffffff',
    "ShowLatestProducts" INTEGER NOT NULL DEFAULT 1,
    "LatestProductsTitle" TEXT,
    "LatestProductsTitleAr" TEXT,
    "LatestProductsSubtitle" TEXT,
    "LatestProductsSubtitleAr" TEXT,
    "LatestProductsCount" INTEGER NOT NULL DEFAULT 6,
    "LatestProductsBgType" TEXT DEFAULT 'color',
    "LatestProductsBgValue" TEXT DEFAULT '#f8f9fa',
    "ShowNewsletter" INTEGER NOT NULL DEFAULT 1,
    "NewsletterTitle" TEXT,
    "NewsletterTitleAr" TEXT,
    "NewsletterSubtitle" TEXT,
    "NewsletterSubtitleAr" TEXT,
    "CreatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Insert default settings
INSERT INTO "HomePageSettings" (
    "ShowSlideshow",
    "ShowCategories", 
    "CategoriesTitle", 
    "CategoriesTitleAr",
    "CategoriesSubtitle", 
    "CategoriesSubtitleAr",
    "CategoriesDisplayCount",
    "CategoriesBgType",
    "CategoriesBgValue",
    "ShowMission",
    "MissionTitle",
    "MissionTitleAr", 
    "MissionSubtitle",
    "MissionSubtitleAr",
    "MissionBgType",
    "MissionBgValue",
    "ShowLatestProducts",
    "LatestProductsTitle",
    "LatestProductsTitleAr",
    "LatestProductsSubtitle", 
    "LatestProductsSubtitleAr",
    "LatestProductsCount",
    "LatestProductsBgType",
    "LatestProductsBgValue",
    "ShowNewsletter",
    "NewsletterTitle",
    "NewsletterTitleAr",
    "NewsletterSubtitle",
    "NewsletterSubtitleAr",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    1, -- ShowSlideshow
    1, -- ShowCategories
    'Our Categories', -- CategoriesTitle
    'فئاتنا', -- CategoriesTitleAr
    'Explore our wide range of products', -- CategoriesSubtitle
    'اكتشف مجموعتنا الواسعة من المنتجات', -- CategoriesSubtitleAr
    8, -- CategoriesDisplayCount
    'color', -- CategoriesBgType
    '#f8f9fa', -- CategoriesBgValue
    1, -- ShowMission
    'Our Mission', -- MissionTitle
    'مهمتنا', -- MissionTitleAr
    'What drives us forward', -- MissionSubtitle
    'ما يدفعنا إلى الأمام', -- MissionSubtitleAr
    'color', -- MissionBgType
    '#ffffff', -- MissionBgValue
    1, -- ShowLatestProducts
    'Latest Products', -- LatestProductsTitle
    'أحدث المنتجات', -- LatestProductsTitleAr
    'Discover our newest arrivals', -- LatestProductsSubtitle
    'اكتشف أحدث وصولاتنا', -- LatestProductsSubtitleAr
    6, -- LatestProductsCount
    'color', -- LatestProductsBgType
    '#f8f9fa', -- LatestProductsBgValue
    1, -- ShowNewsletter
    'Stay Updated', -- NewsletterTitle
    'ابق على اطلاع', -- NewsletterTitleAr
    'Subscribe to get the latest news and offers', -- NewsletterSubtitle
    'اشترك للحصول على آخر الأخبار والعروض', -- NewsletterSubtitleAr
    datetime('now'), -- CreatedAt
    datetime('now')  -- UpdatedAt
);