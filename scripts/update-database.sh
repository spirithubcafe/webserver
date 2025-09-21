#!/bin/bash

# Script to update database with pending migrations safely
# Usage: ./scripts/update-database.sh

set -e  # Exit on any error

PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"
INFRASTRUCTURE_PROJECT="SpirithubCofe.Infrastructure"
WEB_PROJECT="SpirithubCofe.Web"
DB_PATH="$WEB_PROJECT/Data/app.db"

echo "🔄 Updating database with pending migrations"
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
    echo "❌ Error: Build failed. Please fix build errors before updating database."
    exit 1
fi

# Create backup of existing database if it exists
if [ -f "$DB_PATH" ]; then
    BACKUP_PATH="$DB_PATH.backup.$(date +%Y%m%d_%H%M%S)"
    echo "💾 Creating database backup: $BACKUP_PATH"
    cp "$DB_PATH" "$BACKUP_PATH"
fi

# Check for pending migrations
echo "🔍 Checking for pending migrations..."
PENDING_MIGRATIONS=$(dotnet ef migrations list \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --no-build 2>/dev/null | grep "Applied: No" || true)

if [ -z "$PENDING_MIGRATIONS" ]; then
    echo "✅ No pending migrations found. Database is up to date."
    exit 0
fi

echo "📦 Found pending migrations:"
echo "$PENDING_MIGRATIONS"
echo ""

# Update database
echo "🚀 Applying migrations to database..."
dotnet ef database update \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --verbose

if [ $? -eq 0 ]; then
    echo "✅ Database updated successfully!"
    echo "📊 Database location: $DB_PATH"
    
    # Show applied migrations
    echo ""
    echo "📋 All applied migrations:"
    dotnet ef migrations list \
        --project "$WEB_PROJECT" \
        --startup-project "$WEB_PROJECT" \
        --no-build | grep "Applied: Yes" || true
else
    echo "❌ Error: Failed to update database"
    
    # Restore backup if it exists
    if [ -f "$BACKUP_PATH" ]; then
        echo "🔄 Restoring database backup..."
        cp "$BACKUP_PATH" "$DB_PATH"
        echo "✅ Database restored from backup"
    fi
    
    exit 1
fi