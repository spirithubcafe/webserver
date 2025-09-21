-- Sample slides for testing the slideshow functionality
INSERT INTO Slides (Title, TitleAr, Subtitle, SubtitleAr, ImagePath, ButtonText, ButtonTextAr, ButtonUrl, DisplayOrder, IsActive, BackgroundColor, TextColor, CreatedAt, UpdatedAt) VALUES 

-- Slide 1: Welcome to SpirithubCofe
('Welcome to SpirithubCofe', 'مرحبا بكم في سبيريت هب كافيه', 
 'Discover the finest coffee experience in Oman', 'اكتشف أفضل تجربة قهوة في عمان',
 '/images/slides/coffee-shop-hero.jpg', 
 'Explore Our Menu', 'استكشف قائمتنا',
 '/products', 1, 1, 'bg-gradient-to-r from-amber-600 to-amber-800', 'text-white',
 datetime('now'), datetime('now')),

-- Slide 2: Premium Coffee Beans
('Premium Coffee Beans', 'حبوب القهوة المميزة',
 'Sourced from the best coffee farms around the world', 'مصدرها أفضل مزارع القهوة حول العالم',
 '/images/slides/coffee-beans.jpg',
 'Shop Coffee', 'تسوق القهوة',
 '/products/coffee', 2, 1, 'bg-gradient-to-br from-orange-500 to-red-600', 'text-white',
 datetime('now'), datetime('now')),

-- Slide 3: Artisan Brewing
('Artisan Brewing Methods', 'طرق التحضير الحرفية',
 'Experience the art of coffee making with our expert baristas', 'اختبر فن صنع القهوة مع خبرائنا',
 '/images/slides/barista-brewing.jpg',
 'Learn More', 'اعرف المزيد',
 '/about', 3, 1, 'bg-gradient-to-l from-gray-800 to-gray-900', 'text-white',
 datetime('now'), datetime('now')),

-- Slide 4: Special Offers
('Special Coffee Blends', 'خلطات القهوة الخاصة',
 'Try our signature blends crafted for the perfect taste', 'جرب خلطاتنا المميزة المصممة للطعم المثالي',
 '/images/slides/coffee-blend.jpg',
 'View Offers', 'شاهد العروض',
 '/offers', 4, 1, 'bg-gradient-to-tr from-purple-600 to-blue-600', 'text-white',
 datetime('now'), datetime('now'));