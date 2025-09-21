#!/bin/bash

# Script to check database and migration status
# Usage: ./scripts/check-database.sh

set -e  # Exit on any error

PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"
INFRASTRUCTURE_PROJECT="SpirithubCofe.Infrastructure"
WEB_PROJECT="SpirithubCofe.Web"
DB_PATH="$WEB_PROJECT/Data/app.db"

echo "🔍 Checking database and migration status"
echo "📁 Working directory: $PROJECT_ROOT"

# Change to project root
cd "$PROJECT_ROOT"

# Check if projects exist
if [ ! -f "$INFRASTRUCTURE_PROJECT/$INFRASTRUCTURE_PROJECT.csproj" ]; then
    echo "❌ Error: Infrastructure project not found at $INFRASTRUCTURE_PROJECT"
    exit 1
fi

if [ ! -f "$WEB_PROJECT/$WEB_PROJECT.csproj" ]; then
    echo "❌ Error: Web project not found at $WEB_PROJECT"
    exit 1
fi

# Check database file
echo "📊 Database Information:"
if [ -f "$DB_PATH" ]; then
    echo "✅ Database exists: $DB_PATH"
    echo "📏 Database size: $(du -h "$DB_PATH" | cut -f1)"
    echo "📅 Last modified: $(stat -c %y "$DB_PATH")"
else
    echo "❌ Database not found: $DB_PATH"
fi

echo ""

# Build the solution first
echo "🔨 Building solution..."
dotnet build --configuration Debug --verbosity minimal

if [ $? -ne 0 ]; then
    echo "❌ Error: Build failed. Cannot check migration status."
    exit 1
fi

echo ""

# Show all migrations
echo "📋 All Migrations:"
dotnet ef migrations list \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --no-build

echo ""

# Check for pending migrations
echo "🔍 Checking for pending migrations..."
PENDING_MIGRATIONS=$(dotnet ef migrations list \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --no-build 2>/dev/null | grep "Applied: No" || true)

if [ -z "$PENDING_MIGRATIONS" ]; then
    echo "✅ No pending migrations. Database is up to date."
else
    echo "⚠️  Pending migrations found:"
    echo "$PENDING_MIGRATIONS"
    echo ""
    echo "💡 Run: ./scripts/update-database.sh to apply pending migrations"
fi

echo ""

# Show database connection info
echo "🔗 Connection String Information:"
echo "Database Provider: SQLite"
echo "Database Path: $DB_PATH"

# Check if Entity Framework tools are installed
echo ""
echo "🛠️  EF Core Tools Status:"
if command -v dotnet-ef &> /dev/null; then
    EF_VERSION=$(dotnet ef --version 2>/dev/null || echo "Unknown")
    echo "✅ EF Core CLI Tools installed: $EF_VERSION"
else
    echo "❌ EF Core CLI Tools not installed"
    echo "💡 Install with: dotnet tool install --global dotnet-ef"
fi