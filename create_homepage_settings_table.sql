-- Create HomePageSettings table
CREATE TABLE "HomePageSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HomePageSettings" PRIMARY KEY AUTOINCREMENT,
    "IsSlideShowEnabled" INTEGER NOT NULL DEFAULT 1,
    "IsCategoriesEnabled" INTEGER NOT NULL DEFAULT 1,
    "CategoriesTitle" TEXT,
    "CategoriesTitleAr" TEXT,
    "CategoriesSubtitle" TEXT,
    "CategoriesSubtitleAr" TEXT,
    "CategoriesDisplayCount" INTEGER NOT NULL DEFAULT 8,
    "CategoriesBackgroundType" TEXT DEFAULT 'default',
    "CategoriesBackgroundColor" TEXT,
    "CategoriesBackgroundImage" TEXT,
    "CategoriesBackgroundVideo" TEXT,
    "IsMissionEnabled" INTEGER NOT NULL DEFAULT 1,
    "MissionTitle" TEXT,
    "MissionTitleAr" TEXT,
    "MissionSubtitle" TEXT,
    "MissionSubtitleAr" TEXT,
    "MissionBackgroundType" TEXT DEFAULT 'default',
    "MissionBackgroundColor" TEXT,
    "MissionBackgroundImage" TEXT,
    "MissionBackgroundVideo" TEXT,
    "IsLatestProductsEnabled" INTEGER NOT NULL DEFAULT 1,
    "LatestProductsTitle" TEXT,
    "LatestProductsTitleAr" TEXT,
    "LatestProductsSubtitle" TEXT,
    "LatestProductsSubtitleAr" TEXT,
    "LatestProductsDisplayCount" INTEGER NOT NULL DEFAULT 6,
    "LatestProductsBackgroundType" TEXT DEFAULT 'default',
    "LatestProductsBackgroundColor" TEXT,
    "LatestProductsBackgroundImage" TEXT,
    "LatestProductsBackgroundVideo" TEXT,
    "IsNewsletterEnabled" INTEGER NOT NULL DEFAULT 1,
    "NewsletterTitle" TEXT,
    "NewsletterTitleAr" TEXT,
    "NewsletterSubtitle" TEXT,
    "NewsletterSubtitleAr" TEXT,
    "CreatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Insert default settings
INSERT INTO "HomePageSettings" (
    "IsSlideShowEnabled",
    "IsCategoriesEnabled", 
    "CategoriesTitle", 
    "CategoriesTitleAr",
    "CategoriesSubtitle", 
    "CategoriesSubtitleAr",
    "CategoriesDisplayCount",
    "IsMissionEnabled",
    "MissionTitle",
    "MissionTitleAr", 
    "MissionSubtitle",
    "MissionSubtitleAr",
    "IsLatestProductsEnabled",
    "LatestProductsTitle",
    "LatestProductsTitleAr",
    "LatestProductsSubtitle", 
    "LatestProductsSubtitleAr",
    "LatestProductsDisplayCount",
    "IsNewsletterEnabled",
    "NewsletterTitle",
    "NewsletterTitleAr",
    "NewsletterSubtitle",
    "NewsletterSubtitleAr",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    1, -- IsSlideShowEnabled
    1, -- IsCategoriesEnabled
    'Explore Our Categories', -- CategoriesTitle
    'استكشف فئاتنا', -- CategoriesTitleAr
    'Discover our premium coffee collection', -- CategoriesSubtitle
    'اكتشف مجموعة القهوة المميزة لدينا', -- CategoriesSubtitleAr
    8, -- CategoriesDisplayCount
    1, -- IsMissionEnabled
    'Why Choose Us', -- MissionTitle
    'لماذا تختارنا', -- MissionTitleAr
    'Perfect coffee experience awaits you', -- MissionSubtitle
    'تجربة قهوة مثالية في انتظارك', -- MissionSubtitleAr
    1, -- IsLatestProductsEnabled
    'Featured Coffee', -- LatestProductsTitle
    'القهوة المميزة', -- LatestProductsTitleAr
    'Discover our popular selections', -- LatestProductsSubtitle
    'اكتشف اختياراتنا الشعبية', -- LatestProductsSubtitleAr
    6, -- LatestProductsDisplayCount
    1, -- IsNewsletterEnabled
    'Stay Updated', -- NewsletterTitle
    'ابق على اطلاع', -- NewsletterTitleAr
    'Subscribe to get the latest news and offers', -- NewsletterSubtitle
    'اشترك للحصول على آخر الأخبار والعروض', -- NewsletterSubtitleAr
    datetime('now'), -- CreatedAt
    datetime('now')  -- UpdatedAt
);