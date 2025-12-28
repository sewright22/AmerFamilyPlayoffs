#!/bin/bash

# Simple backup script for NFL Playoff Pool deployment
# This script creates a backup before deployment

set -e

BACKUP_DIR="/opt/nflplayoffpool/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_NAME="nflpool_backup_${TIMESTAMP}"

echo "🔄 Creating backup: ${BACKUP_NAME}"

# Create backup directory if it doesn't exist
mkdir -p "${BACKUP_DIR}"

# Create backup of current deployment
if [ -d "/opt/nflplayoffpool" ]; then
    # Backup current .env file
    if [ -f "/opt/nflplayoffpool/.env" ]; then
        cp "/opt/nflplayoffpool/.env" "${BACKUP_DIR}/${BACKUP_NAME}.env"
        echo "✅ Environment file backed up"
    fi
    
    # Backup MongoDB data if it exists
    if [ -d "/opt/nflplayoffpool/data/mongodb" ]; then
        echo "📦 Backing up MongoDB data..."
        tar -czf "${BACKUP_DIR}/${BACKUP_NAME}_mongodb.tar.gz" -C "/opt/nflplayoffpool/data" mongodb
        echo "✅ MongoDB data backed up"
    fi
    
    # Keep only last 5 backups
    cd "${BACKUP_DIR}"
    ls -t nflpool_backup_*.env 2>/dev/null | tail -n +6 | xargs -r rm
    ls -t nflpool_backup_*_mongodb.tar.gz 2>/dev/null | tail -n +6 | xargs -r rm
    
    echo "✅ Backup completed: ${BACKUP_NAME}"
else
    echo "⚠️ No existing deployment found, skipping backup"
fi

exit 0