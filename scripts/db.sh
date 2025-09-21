#!/bin/bash

# Main database management script
# Usage: ./scripts/db.sh [command]

PROJECT_ROOT="/home/milad/Documents/GitHub/spirithubcafe/webserver"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

show_help() {
    echo -e "${CYAN}🗄️  SpirithubCofe Database Management Scripts${NC}"
    echo ""
    echo -e "${YELLOW}Available commands:${NC}"
    echo ""
    echo -e "${GREEN}📦 Migration Management:${NC}"
    echo "  add <name>        Add a new migration"
    echo "  update           Apply pending migrations to database"
    echo "  remove           Remove the last migration"
    echo "  reset [name]     Reset database to specific migration"
    echo ""
    echo -e "${BLUE}🔍 Information:${NC}"
    echo "  check            Check database and migration status"
    echo "  list             List all migrations"
    echo ""
    echo -e "${PURPLE}Examples:${NC}"
    echo "  ./scripts/db.sh add \"AddUserPreferences\""
    echo "  ./scripts/db.sh update"
    echo "  ./scripts/db.sh check"
    echo "  ./scripts/db.sh reset InitialCreate"
    echo ""
    echo -e "${YELLOW}Individual scripts (if you prefer):${NC}"
    echo "  ./scripts/add-migration.sh \"MigrationName\""
    echo "  ./scripts/update-database.sh"
    echo "  ./scripts/remove-migration.sh"
    echo "  ./scripts/reset-database.sh [migration]"
    echo "  ./scripts/check-database.sh"
}

# Change to project root
cd "$PROJECT_ROOT"

case "$1" in
    "add")
        if [ -z "$2" ]; then
            echo -e "${RED}❌ Error: Migration name is required${NC}"
            echo "Usage: ./scripts/db.sh add \"MigrationName\""
            exit 1
        fi
        ./scripts/add-migration.sh "$2"
        ;;
    "update")
        ./scripts/update-database.sh
        ;;
    "remove")
        ./scripts/remove-migration.sh
        ;;
    "reset")
        ./scripts/reset-database.sh "$2"
        ;;
    "check")
        ./scripts/check-database.sh
        ;;
    "list")
        echo -e "${CYAN}📋 Listing all migrations...${NC}"
        dotnet ef migrations list \
            --project SpirithubCofe.Web \
            --startup-project SpirithubCofe.Web \
            --no-build 2>/dev/null || {
                echo -e "${YELLOW}⚠️  Building solution first...${NC}"
                dotnet build --verbosity minimal
                dotnet ef migrations list \
                    --project SpirithubCofe.Web \
                    --startup-project SpirithubCofe.Web \
                    --no-build
            }
        ;;
    "help"|"-h"|"--help"|"")
        show_help
        ;;
    *)
        echo -e "${RED}❌ Unknown command: $1${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac