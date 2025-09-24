#!/bin/bash

# SpirithubCofe Packaging Script

echo "📦 Starting SpirithubCofe packaging"
echo "================================="

# Check if publish folder exists
if [ ! -d "publish" ]; then
    echo "❌ Publish folder not found. Run publish-all.sh first"
    exit 1
fi

# Create packages folder
mkdir -p packages
rm -f packages/*.zip

echo "🗜️ Packaging different versions..."

# Package Linux version
if [ -d "publish/linux-x64" ]; then
    echo "🐧 Packaging Linux version..."
    cd publish
    zip -r ../packages/SpirithubCofe-Linux-x64.zip linux-x64/
    cd ..
fi

# Package Windows version
if [ -d "publish/windows-x64" ]; then
    echo "🪟 Packaging Windows version..."
    cd publish
    zip -r ../packages/SpirithubCofe-Windows-x64.zip windows-x64/
    cd ..
fi

# Package macOS Intel version
if [ -d "publish/osx-x64" ]; then
    echo "🍎 Packaging macOS Intel version..."
    cd publish
    zip -r ../packages/SpirithubCofe-macOS-x64.zip osx-x64/
    cd ..
fi

# Package macOS ARM version
if [ -d "publish/osx-arm64" ]; then
    echo "🍎 Packaging macOS ARM version..."
    cd publish
    zip -r ../packages/SpirithubCofe-macOS-ARM64.zip osx-arm64/
    cd ..
fi

# Package Portable version
if [ -d "publish/portable" ]; then
    echo "📱 Packaging Portable version..."
    cd publish
    zip -r ../packages/SpirithubCofe-Portable.zip portable/
    cd ..
fi

# Create Release Notes
cat > packages/RELEASE-NOTES.txt << EOF
SpirithubCofe Release
====================

Version: 1.0.0
Date: $(date +"%Y-%m-%d")

Features:
- Online coffee shop platform
- Arabic and English language support
- Product and category management
- User system with roles
- Complete admin panel

Available Versions:

1. SpirithubCofe-Linux-x64.zip
   - For Linux servers
   - Includes all required files
   - Run: ./SpirithubCofe.Web

2. SpirithubCofe-Windows-x64.zip
   - For Windows servers
   - Includes all required files
   - Run: SpirithubCofe.Web.exe

3. SpirithubCofe-macOS-x64.zip
   - For Intel Macs
   - Includes all required files
   - Run: ./SpirithubCofe.Web

4. SpirithubCofe-macOS-ARM64.zip
   - For Apple Silicon Macs
   - Includes all required files
   - Run: ./SpirithubCofe.Web

5. SpirithubCofe-Portable.zip
   - Small version (requires .NET 9 Runtime)
   - Run: dotnet SpirithubCofe.Web.dll

Requirements:
- Self-Contained versions: No requirements
- Portable version: .NET 9.0 Runtime

Default URL: http://localhost:5000

Default Admin Account:
Email: admin@spirithubcofe.com
Password: Admin@123456

Support:
Email: info@spirithubcofe.com
Website: https://spirithubcofe.com

Quick Start:
1. Download the appropriate ZIP file for your OS
2. Extract the file
3. Run the executable
4. Open browser to http://localhost:5000
5. Login with admin account

Congratulations! SpirithubCofe is ready to use! 🎉
EOF

echo ""
echo "✅ Packaging completed!"
echo "========================"

echo "📦 Ready packages:"
ls -lh packages/

echo ""
echo "📊 Package sizes:"
for file in packages/*.zip; do
    if [ -f "$file" ]; then
        echo "  $(basename "$file"): $(du -h "$file" | cut -f1)"
    fi
done

echo ""
echo "🎉 Everything ready for distribution!"