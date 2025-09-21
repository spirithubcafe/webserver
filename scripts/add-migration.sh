#!/bin/bash

# Script to add EF Core migrations safely
# Usage: ./scripts/add-migration.sh "MigrationName"

set -e  # Exit on any error

# Check if migration name is provided
if [ -z "$1" ]; then
    echo "❌ Error: Migration name is required"
    echo "Usage: ./scripts/add-migration.sh \"MigrationName\""
    exit 1
fi

MIGRATION_NAME="$1"
PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"
INFRASTRUCTURE_PROJECT="SpirithubCofe.Infrastructure"
WEB_PROJECT="SpirithubCofe.Web"

echo "🔧 Adding migration: $MIGRATION_NAME"
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
    echo "❌ Error: Build failed. Please fix build errors before adding migration."
    exit 1
fi

# Add migration
echo "📦 Adding migration..."
dotnet ef migrations add "$MIGRATION_NAME" \
    --project "$WEB_PROJECT" \
    --startup-project "$WEB_PROJECT" \
    --output-dir "Data/Migrations" \
    --verbose

if [ $? -eq 0 ]; then
    echo "✅ Migration '$MIGRATION_NAME' added successfully!"
    echo "📝 Migration files created in $WEB_PROJECT/Data/Migrations/"
    echo ""
    echo "Next steps:"
    echo "1. Review the generated migration files"
    echo "2. Run: ./scripts/update-database.sh"
else
    echo "❌ Error: Failed to add migration"
    exit 1
fi