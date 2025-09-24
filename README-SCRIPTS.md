# راهنمای اسکریپت‌های نشر / Publishing Scripts Guide

## اسکریپت‌های موجود در فایل package.json اصلی:

### اجرا و ساخت:
```bash
npm run start          # اجرای پروژه در حالت توسعه
npm run build          # ساخت پروژه در حالت Release
```

### نشر برای سیستم‌عامل‌های مختلف:
```bash
npm run publish:linux     # نشر برای لینوکس (x64)
npm run publish:windows   # نشر برای ویندوز (x64)
npm run publish:mac       # نشر برای مک (Intel)
npm run publish:mac-arm   # نشر برای مک (Apple Silicon)
npm run publish:all       # نشر برای همه سیستم‌عامل‌ها
npm run publish:portable  # نشر قابل حمل (نیاز به .NET Runtime)
```

## اسکریپت‌های موجود در SpirithubCofe.Web/package.json:

### CSS و استایل:
```bash
npm run build:css        # ساخت فایل CSS نهایی
npm run watch:css        # نظارت بر تغییرات CSS
```

### نشر (با ساخت CSS):
```bash
npm run publish:linux     # نشر برای لینوکس + ساخت CSS
npm run publish:windows   # نشر برای ویندوز + ساخت CSS
npm run publish:mac       # نشر برای مک Intel + ساخت CSS
npm run publish:mac-arm   # نشر برای مک Apple Silicon + ساخت CSS
npm run publish:all       # نشر برای همه سیستم‌عامل‌ها + ساخت CSS
npm run publish:portable  # نشر قابل حمل + ساخت CSS
npm run publish:docker    # ساخت تصویر Docker
```

## مثال‌های استفاده:

### 1. نشر برای لینوکس:
```bash
# از مجلد اصلی پروژه
npm run publish:linux

# یا از مجلد SpirithubCofe.Web
cd SpirithubCofe.Web
npm run publish:linux
```

### 2. نشر برای همه سیستم‌عامل‌ها:
```bash
npm run publish:all
```

### 3. نشر قابل حمل (کوچک‌تر، نیاز به .NET Runtime):
```bash
npm run publish:portable
```

## مجلدهای خروجی:

بعد از اجرای اسکریپت‌ها، فایل‌های نشر شده در این مجلدها قرار می‌گیرند:
- `./publish/linux-x64/` - برای لینوکس
- `./publish/windows-x64/` - برای ویندوز
- `./publish/osx-x64/` - برای مک Intel
- `./publish/osx-arm64/` - برای مک Apple Silicon
- `./publish/portable/` - نسخه قابل حمل

## تفاوت Self-Contained و Portable:

### Self-Contained (`--self-contained true`):
- ✅ همه چیز در کنار فایل اجرایی
- ✅ نیازی به نصب .NET نیست
- ❌ حجم بیشتر (حدود 70-100 مگابایت)

### Portable (`--self-contained false`):
- ✅ حجم کمتر (حدود 10-20 مگابایت)
- ❌ نیاز به نصب .NET Runtime روی سیستم مقصد

## نکات مهم:

1. **CSS**: اسکریپت‌های موجود در `SpirithubCofe.Web/package.json` قبل از نشر، CSS را هم می‌سازند
2. **دیتابیس**: فایل دیتابیس SQLite در مجلد `Data/` کپی می‌شود
3. **تنظیمات**: فایل‌های `appsettings.json` و `appsettings.Production.json` کپی می‌شوند
4. **استاتیک فایل‌ها**: تمام فایل‌های `wwwroot/` شامل تصاویر و CSS کپی می‌شوند

## اجرای فایل نشر شده:

### لینوکس/مک:
```bash
cd publish/linux-x64
./SpirithubCofe.Web
```

### ویندوز:
```cmd
cd publish\windows-x64
SpirithubCofe.Web.exe
```

### نسخه Portable:
```bash
cd publish/portable
dotnet SpirithubCofe.Web.dll
```