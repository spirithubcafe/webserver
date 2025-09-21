# Database Management Scripts

These scripts provide reliable and safe database migration management for the SpirithubCofe project.

## 🚀 Quick Start

Use the main script for all database operations:

```bash
# Show all available commands
./scripts/db.sh

# Add a new migration
./scripts/db.sh add "AddNewFeature"

# Apply pending migrations
./scripts/db.sh update

# Check database status
./scripts/db.sh check
```

## 📦 Available Scripts

### Main Script: `db.sh`
The master script that provides easy access to all database operations.

```bash
./scripts/db.sh [command]
```

**Commands:**
- `add <name>` - Add a new migration
- `update` - Apply pending migrations
- `remove` - Remove the last migration
- `reset [name]` - Reset database to specific migration
- `check` - Check database and migration status
- `list` - List all migrations

### Individual Scripts

#### 1. Add Migration: `add-migration.sh`
```bash
./scripts/add-migration.sh "MigrationName"
```
- ✅ Validates project structure
- ✅ Builds solution before adding migration
- ✅ Uses correct project references
- ✅ Creates migration in proper directory

#### 2. Update Database: `update-database.sh`
```bash
./scripts/update-database.sh
```
- ✅ Creates automatic backup before update
- ✅ Checks for pending migrations
- ✅ Applies migrations safely
- ✅ Restores backup if update fails

#### 3. Remove Migration: `remove-migration.sh`
```bash
./scripts/remove-migration.sh
```
- ✅ Shows current migrations before removal
- ✅ Asks for confirmation
- ✅ Safely removes last migration

#### 4. Reset Database: `reset-database.sh`
```bash
./scripts/reset-database.sh [migration-name]
```
- ✅ Creates backup before reset
- ✅ Shows available migrations
- ✅ Resets to specific migration or empty database
- ✅ Restores backup if reset fails

#### 5. Check Database: `check-database.sh`
```bash
./scripts/check-database.sh
```
- ✅ Shows database file information
- ✅ Lists all migrations and their status
- ✅ Identifies pending migrations
- ✅ Checks EF Core tools installation

## 🔧 Project Configuration

These scripts are configured for:
- **Infrastructure Project:** `SpirithubCofe.Infrastructure` (Domain & Services)
- **Web Project:** `SpirithubCofe.Web` (Contains DbContext & Migrations)
- **Database:** SQLite at `SpirithubCofe.Web/Data/app.db`
- **Migrations Directory:** `SpirithubCofe.Web/Data/Migrations`

## 📝 Examples

### Adding a New Feature

```bash
# 1. Add migration for new feature
./scripts/db.sh add "AddUserPreferences"

# 2. Review the generated migration files
# 3. Apply the migration
./scripts/db.sh update

# 4. Verify the update
./scripts/db.sh check
```

### Fixing a Migration Error

```bash
# 1. Remove the problematic migration
./scripts/db.sh remove

# 2. Fix your entity models
# 3. Add the migration again
./scripts/db.sh add "FixedMigrationName"

# 4. Apply the corrected migration
./scripts/db.sh update
```

### Starting Fresh

```bash
# Reset to empty database
./scripts/db.sh reset 0

# Or reset to a specific migration
./scripts/db.sh reset "InitialCreate"
```

## 🛡️ Safety Features

- **Automatic Backups:** Database backups are created before any destructive operations
- **Build Validation:** Solution is built before migration operations
- **Error Handling:** Scripts exit on first error to prevent cascading issues
- **Confirmation Prompts:** Destructive operations require user confirmation
- **Rollback Support:** Failed operations restore from backup when possible

## 🐛 Troubleshooting

### Build Errors
If you get build errors:
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### EF Tools Not Found
If EF Core tools are not installed:
```bash
dotnet tool install --global dotnet-ef
```

### Database Locked
If database is locked:
```bash
# Stop the application first, then retry
./scripts/db.sh check
```

### Migration Conflicts
If you have migration conflicts:
```bash
# Check current status
./scripts/db.sh list

# Remove problematic migration
./scripts/db.sh remove

# Fix your models and try again
./scripts/db.sh add "FixedMigration"
```

## 📁 Script Locations

All scripts are located in the `scripts/` directory:
```
scripts/
├── db.sh                    # Main script (use this)
├── add-migration.sh         # Add new migration
├── update-database.sh       # Apply migrations
├── remove-migration.sh      # Remove last migration
├── reset-database.sh        # Reset to specific migration
├── check-database.sh        # Check status
└── README.md               # This file
```

## 🎯 Best Practices

1. **Always use the main script:** `./scripts/db.sh` for consistency
2. **Check status before changes:** Use `./scripts/db.sh check` first
3. **Use descriptive migration names:** e.g., "AddUserPreferences", "UpdateProductSchema"
4. **Review migrations before applying:** Check generated files in `Data/Migrations/`
5. **Keep backups:** Scripts create automatic backups, but manual backups are also good
6. **Test migrations:** Apply on development database before production

## 🔄 Workflow Integration

### During Development
```bash
# Daily workflow
./scripts/db.sh check           # Check current status
./scripts/db.sh add "NewFeature" # Add your changes
./scripts/db.sh update          # Apply changes
```

### Before Deployment
```bash
# Pre-deployment checklist
./scripts/db.sh check           # Verify clean state
./scripts/db.sh list            # Review all migrations
# Deploy with confidence
```