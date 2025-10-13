#!/usr/bin/env python3
import sqlite3
import os
from datetime import datetime

def parse_translations_file():
    """Parse the hardcoded_texts.txt file and extract translations"""
    translations = []
    current_category = "General"
    
    with open('/home/milad/Documents/GitHub/spirithubcafe/webserver/hardcoded_texts.txt', 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    for line in lines:
        line = line.strip()
        
        # Skip empty lines and header lines
        if not line or line.startswith('HARDCODED TEXTS') or line.startswith('=====') or line.startswith('Format:'):
            continue
            
        # Check for category headers (lines starting with #)
        if line.startswith('#'):
            # Extract category name from comment
            if 'ExternalLogins' in line:
                current_category = "ExternalLogins"
            elif 'GenerateRecoveryCodes' in line:
                current_category = "GenerateRecoveryCodes"
            elif 'PersonalData' in line:
                current_category = "PersonalData"
            elif 'SetPassword' in line:
                current_category = "SetPassword"
            elif 'Disable2FA' in line:
                current_category = "Disable2FA"
            elif 'Email' in line:
                current_category = "Email"
            elif 'ResetAuthenticator' in line:
                current_category = "ResetAuthenticator"
            elif 'ChangePassword' in line:
                current_category = "ChangePassword"
            elif 'DeletePersonalData' in line:
                current_category = "DeletePersonalData"
            else:
                current_category = "General"
            continue
        
        # Parse translation lines in format: Key|English|Arabic
        if '|' in line and not line.startswith('#'):
            parts = line.split('|')
            if len(parts) >= 3:
                key = parts[0].strip()
                english = parts[1].strip()
                arabic = parts[2].strip()
                
                # Skip empty keys
                if key:
                    translations.append({
                        'key': key,
                        'english': english,
                        'arabic': arabic,
                        'category': current_category
                    })
    
    return translations

def insert_translations_to_db():
    """Insert translations into the SQLite database"""
    # Database path
    db_path = '/home/milad/Documents/GitHub/spirithubcafe/webserver/SpirithubCafe.Web/Data/app.db'
    
    if not os.path.exists(db_path):
        print(f"Database file not found at {db_path}")
        return False
    
    # Parse translations
    translations = parse_translations_file()
    
    if not translations:
        print("No translations found to insert")
        return False
    
    print(f"Found {len(translations)} translations to insert")
    
    # Connect to database
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        # Get current timestamp
        current_time = datetime.utcnow().isoformat()
        
        # Check existing translations to avoid duplicates
        existing_keys = set()
        cursor.execute("SELECT Key FROM Translations")
        for row in cursor.fetchall():
            existing_keys.add(row[0])
        
        # Insert or update translations
        inserted_count = 0
        updated_count = 0
        
        for translation in translations:
            key = translation['key']
            english = translation['english']
            arabic = translation['arabic']
            category = translation['category']
            
            # Use INSERT OR REPLACE to handle duplicates
            cursor.execute("""
                INSERT OR REPLACE INTO Translations (Key, ValueEn, ValueAr, Category, CreatedAt, UpdatedAt)
                VALUES (?, ?, ?, ?, 
                    COALESCE((SELECT CreatedAt FROM Translations WHERE Key = ?), ?),
                    ?)
            """, (key, english, arabic, category, key, current_time, current_time))
            
            if key in existing_keys:
                updated_count += 1
            else:
                inserted_count += 1
        
        # Commit changes
        conn.commit()
        print(f"✅ Successfully inserted {inserted_count} new translations")
        print(f"✅ Successfully updated {updated_count} existing translations")
        print(f"📊 Total translations processed: {len(translations)}")
        
        return True
        
    except Exception as e:
        print(f"❌ Error inserting translations: {e}")
        conn.rollback()
        return False
    finally:
        conn.close()

def verify_translations():
    """Verify translations were inserted correctly"""
    db_path = '/home/milad/Documents/GitHub/spirithubcafe/webserver/SpirithubCafe.Web/Data/app.db'
    
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    try:
        # Count total translations
        cursor.execute("SELECT COUNT(*) FROM Translations")
        total_count = cursor.fetchone()[0]
        print(f"📊 Total translations in database: {total_count}")
        
        # Count by category
        cursor.execute("SELECT Category, COUNT(*) FROM Translations GROUP BY Category ORDER BY Category")
        categories = cursor.fetchall()
        
        print("\n📂 Translations by category:")
        for category, count in categories:
            print(f"   {category}: {count} translations")
        
        # Show sample translations
        print("\n🔍 Sample translations:")
        cursor.execute("SELECT Key, ValueEn, ValueAr FROM Translations LIMIT 5")
        samples = cursor.fetchall()
        
        for key, en, ar in samples:
            print(f"   {key}: '{en}' | '{ar}'")
            
    except Exception as e:
        print(f"❌ Error verifying translations: {e}")
    finally:
        conn.close()

if __name__ == "__main__":
    print("🚀 Starting translation import process...")
    
    if insert_translations_to_db():
        print("\n🔍 Verifying translations...")
        verify_translations()
        print("\n✅ Translation import completed successfully!")
    else:
        print("\n❌ Translation import failed!")