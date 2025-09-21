#!/bin/bash

# Script to remove the last migration safely
# Usage: ./scripts/remove-migration.sh

set -e  # Exit on any error

PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"
INFRASTRUCTURE_PROJECT="SpirithubCofe.Infrastructure"
WEB_PROJECT="SpirithubCofe.Web"

echo "🗑️  Removing last migration"
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
    echo "❌ Error: Build failed. Please fix build errors before removing migration."
    exit 1
fi

# Show current migrations
echo "📋 Current migrations:"
dotnet ef migrations list \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --no-build

echo ""
read -p "❓ Are you sure you want to remove the last migration? (y/N): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Operation cancelled"
    exit 1
fi

# Remove last migration
echo "🗑️  Removing last migration..."
dotnet ef migrations remove \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --force

if [ $? -eq 0 ]; then
    echo "✅ Last migration removed successfully!"
    
    echo ""
    echo "📋 Remaining migrations:"
    dotnet ef migrations list \
        --project "$WEB_PROJECT" \
        --startup-project "$WEB_PROJECT" \
        --no-build
else
    echo "❌ Error: Failed to remove migration"
    exit 1
fi