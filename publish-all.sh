#!/bin/bash

# Complete Publishing Script for SpirithubCofe

echo "🚀 Starting complete publishing process for SpirithubCofe"
echo "=================================================="

# Clean previous publish folder
echo "🧹 Cleaning previous publish folders..."
rm -rf publish/

# Build project
echo "🔨 Building project..."
dotnet build SpirithubCofe.Web/SpirithubCofe.Web.csproj -c Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

# Build CSS (if node_modules exists)
if [ -d "SpirithubCofe.Web/node_modules" ]; then
    echo "🎨 Building CSS files..."
    cd SpirithubCofe.Web
    npm run build:css
    cd ..
fi

echo "📦 Starting publish for different platforms..."

# Publish for Linux
echo "🐧 Publishing for Linux x64..."
dotnet publish SpirithubCofe.Web/SpirithubCofe.Web.csproj \
    -c Release \
    -o ./publish/linux-x64 \
    --self-contained true \
    --runtime linux-x64 \
    -p:PublishSingleFile=false

# Copy additional important files
echo "📁 Copying additional files for Linux..."
cp -r SpirithubCofe.Web/wwwroot/js ./publish/linux-x64/wwwroot/ 2>/dev/null || true
cp -r SpirithubCofe.Web/wwwroot/uploads ./publish/linux-x64/wwwroot/ 2>/dev/null || true
mkdir -p ./publish/linux-x64/Data
cp SpirithubCofe.Web/Data/* ./publish/linux-x64/Data/ 2>/dev/null || true

# Publish for Windows
echo "🪟 Publishing for Windows x64..."
dotnet publish SpirithubCofe.Web/SpirithubCofe.Web.csproj \
    -c Release \
    -o ./publish/windows-x64 \
    --self-contained true \
    --runtime win-x64 \
    -p:PublishSingleFile=false

# Copy additional important files
echo "📁 Copying additional files for Windows..."
cp -r SpirithubCofe.Web/wwwroot/js ./publish/windows-x64/wwwroot/ 2>/dev/null || true
cp -r SpirithubCofe.Web/wwwroot/uploads ./publish/windows-x64/wwwroot/ 2>/dev/null || true
mkdir -p ./publish/windows-x64/Data
cp SpirithubCofe.Web/Data/* ./publish/windows-x64/Data/ 2>/dev/null || true

# Publish for macOS Intel
echo "🍎 Publishing for macOS x64 (Intel)..."
dotnet publish SpirithubCofe.Web/SpirithubCofe.Web.csproj \
    -c Release \
    -o ./publish/osx-x64 \
    --self-contained true \
    --runtime osx-x64 \
    -p:PublishSingleFile=false

# Copy additional important files
echo "📁 Copying additional files for macOS Intel..."
cp -r SpirithubCofe.Web/wwwroot/js ./publish/osx-x64/wwwroot/ 2>/dev/null || true
cp -r SpirithubCofe.Web/wwwroot/uploads ./publish/osx-x64/wwwroot/ 2>/dev/null || true
mkdir -p ./publish/osx-x64/Data
cp SpirithubCofe.Web/Data/* ./publish/osx-x64/Data/ 2>/dev/null || true

# Publish for macOS Apple Silicon
echo "🍎 Publishing for macOS ARM64 (Apple Silicon)..."
dotnet publish SpirithubCofe.Web/SpirithubCofe.Web.csproj \
    -c Release \
    -o ./publish/osx-arm64 \
    --self-contained true \
    --runtime osx-arm64 \
    -p:PublishSingleFile=false

# Copy additional important files
echo "📁 Copying additional files for macOS ARM..."
cp -r SpirithubCofe.Web/wwwroot/js ./publish/osx-arm64/wwwroot/ 2>/dev/null || true
cp -r SpirithubCofe.Web/wwwroot/uploads ./publish/osx-arm64/wwwroot/ 2>/dev/null || true
mkdir -p ./publish/osx-arm64/Data
cp SpirithubCofe.Web/Data/* ./publish/osx-arm64/Data/ 2>/dev/null || true

# Publish Portable version
echo "📱 Publishing Portable version..."
dotnet publish SpirithubCofe.Web/SpirithubCofe.Web.csproj \
    -c Release \
    -o ./publish/portable \
    --self-contained false

# Copy additional important files
echo "📁 Copying additional files for Portable version..."
cp -r SpirithubCofe.Web/wwwroot/js ./publish/portable/wwwroot/ 2>/dev/null || true
cp -r SpirithubCofe.Web/wwwroot/uploads ./publish/portable/wwwroot/ 2>/dev/null || true
mkdir -p ./publish/portable/Data
cp SpirithubCofe.Web/Data/* ./publish/portable/Data/ 2>/dev/null || true

# Create README files for each platform
echo "📝 Creating README files..."

# Create Linux README
cat > publish/linux-x64/README.txt << 'EOF'
SpirithubCofe - Linux Version
=============================

How to run:
./SpirithubCofe.Web

Default port: 5000
Browser URL: http://localhost:5000

Notes:
- This version includes all required files
- No need to install .NET
- Database file is located in Data/ folder

For support: info@spirithubcofe.com
EOF

# Create Windows README
cat > publish/windows-x64/README.txt << 'EOF'
SpirithubCofe - Windows Version
===============================

How to run:
SpirithubCofe.Web.exe

Default port: 5000
Browser URL: http://localhost:5000

Notes:
- This version includes all required files
- No need to install .NET
- Database file is located in Data\ folder

For support: info@spirithubcofe.com
EOF

# Create macOS Intel README
cat > publish/osx-x64/README.txt << 'EOF'
SpirithubCofe - macOS Version (Intel)
======================================

How to run:
./SpirithubCofe.Web

Default port: 5000
Browser URL: http://localhost:5000

Notes:
- This version includes all required files
- No need to install .NET
- Database file is located in Data/ folder

For support: info@spirithubcofe.com
EOF

# Create macOS ARM README
cat > publish/osx-arm64/README.txt << 'EOF'
SpirithubCofe - macOS Version (Apple Silicon)
===============================================

How to run:
./SpirithubCofe.Web

Default port: 5000
Browser URL: http://localhost:5000

Notes:
- This version includes all required files
- No need to install .NET
- Database file is located in Data/ folder

For support: info@spirithubcofe.com
EOF

# Create Portable README
cat > publish/portable/README.txt << 'EOF'
SpirithubCofe - Portable Version
=================================

Requirements:
- .NET 9.0 Runtime must be installed

How to run:
dotnet SpirithubCofe.Web.dll

Default port: 5000
Browser URL: http://localhost:5000

Notes:
- This version is smaller but requires .NET Runtime
- Database file is located in Data/ folder

For support: info@spirithubcofe.com
EOF

# Show final report
echo ""
echo "✅ Publishing process completed successfully!"
echo "=================================================="

echo "📊 File sizes report:"
for dir in publish/*/; do
    if [ -d "$dir" ]; then
        size=$(du -sh "$dir" | cut -f1)
        dirname=$(basename "$dir")
        echo "  $dirname: $size"
    fi
done

echo ""
echo "📁 Published folders in:"
echo "  ./publish/"
echo ""
echo "🎉 Ready for distribution!"