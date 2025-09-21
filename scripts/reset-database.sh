#!/bin/bash

# Script to reset database to a specific migration
# Usage: ./scripts/reset-database.sh [migration-name]

set -e  # Exit on any error

PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"
INFRASTRUCTURE_PROJECT="SpirithubCofe.Infrastructure"
WEB_PROJECT="SpirithubCofe.Web"
DB_PATH="$WEB_PROJECT/Data/app.db"

echo "🔄 Resetting database to specific migration"
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

# Build the solution first
echo "🔨 Building solution..."
dotnet build --configuration Debug --verbosity minimal

if [ $? -ne 0 ]; then
    echo "❌ Error: Build failed. Please fix build errors before resetting database."
    exit 1
fi

# Show available migrations
echo "📋 Available migrations:"
dotnet ef migrations list \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --no-build

echo ""

# Get target migration
if [ -z "$1" ]; then
    read -p "🎯 Enter migration name to reset to (or '0' for empty database): " TARGET_MIGRATION
else
    TARGET_MIGRATION="$1"
fi

if [ -z "$TARGET_MIGRATION" ]; then
    echo "❌ Error: Migration name is required"
    exit 1
fi

# Create backup of existing database if it exists
if [ -f "$DB_PATH" ]; then
    BACKUP_PATH="$DB_PATH.backup.$(date +%Y%m%d_%H%M%S)"
    echo "💾 Creating database backup: $BACKUP_PATH"
    cp "$DB_PATH" "$BACKUP_PATH"
fi

echo ""
echo "⚠️  WARNING: This will reset your database to migration: $TARGET_MIGRATION"
echo "💾 Database backup created at: $BACKUP_PATH"
read -p "❓ Are you sure you want to continue? (y/N): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Operation cancelled"
    exit 1
fi

# Reset database to specific migration
echo "🚀 Resetting database to migration: $TARGET_MIGRATION"
dotnet ef database update "$TARGET_MIGRATION" \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --verbose

if [ $? -eq 0 ]; then
    echo "✅ Database reset successfully to migration: $TARGET_MIGRATION"
    echo "📊 Database location: $DB_PATH"
    
    # Show current migration status
    echo ""
    echo "📋 Current migration status:"
    dotnet ef migrations list \
        --project "$WEB_PROJECT" \
        --startup-project "$WEB_PROJECT" \
        --no-build
else
    echo "❌ Error: Failed to reset database"
    
    # Restore backup if it exists
    if [ -f "$BACKUP_PATH" ]; then
        echo "🔄 Restoring database backup..."
        cp "$BACKUP_PATH" "$DB_PATH"
        echo "✅ Database restored from backup"
    fi
    
    exit 1
fi