#!/bin/bash

# NFL Playoff Pool - Ubuntu VM Setup Script
# This script configures a fresh Ubuntu VM for running the NFL Playoff Pool application
# Usage: ./setup-ubuntu-vm.sh [environment]
# Environment: test (default) | staging | production

set -e

ENVIRONMENT=${1:-test}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"

echo "🚀 Setting up Ubuntu VM for NFL Playoff Pool ($ENVIRONMENT environment)"
echo "📁 Project directory: $PROJECT_DIR"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if running as root
if [[ $EUID -eq 0 ]]; then
   log_error "This script should not be run as root"
   exit 1
fi

# Update system packages
log_info "Updating system packages..."
sudo apt update && sudo apt upgrade -y
log_success "System packages updated"

# Install essential packages
log_info "Installing essential packages..."
sudo apt install -y \
    curl \
    wget \
    git \
    unzip \
    htop \
    nano \
    vim \
    net-tools \
    ufw \
    fail2ban \
    logrotate \
    cron
log_success "Essential packages installed"

# Install Docker
if ! command -v docker &> /dev/null; then
    log_info "Installing Docker..."
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
    log_success "Docker installed"
else
    log_success "Docker already installed"
fi

# Install Docker Compose
if ! command -v docker-compose &> /dev/null; then
    log_info "Installing Docker Compose..."
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    log_success "Docker Compose installed"
else
    log_success "Docker Compose already installed"
fi

# Configure firewall
log_info "Configuring firewall..."
sudo ufw --force reset
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Allow specific ports based on environment
case $ENVIRONMENT in
    "test")
        sudo ufw allow 8080/tcp  # Test webapp port
        sudo ufw allow 27018/tcp # Test MongoDB port
        ;;
    "staging")
        sudo ufw allow 8081/tcp  # Staging webapp port
        sudo ufw allow 27019/tcp # Staging MongoDB port
        ;;
    "production")
        sudo ufw allow 5000/tcp  # Production webapp port
        # MongoDB port not exposed externally in production
        ;;
esac

sudo ufw --force enable
log_success "Firewall configured for $ENVIRONMENT environment"

# Configure fail2ban
log_info "Configuring fail2ban..."
sudo systemctl enable fail2ban
sudo systemctl start fail2ban
log_success "Fail2ban configured"

# Set timezone
log_info "Setting timezone to America/New_York..."
sudo timedatectl set-timezone America/New_York
log_success "Timezone set"

# Configure automatic security updates
log_info "Configuring automatic security updates..."
sudo apt install -y unattended-upgrades
echo 'Unattended-Upgrade::Automatic-Reboot "false";' | sudo tee -a /etc/apt/apt.conf.d/50unattended-upgrades
sudo systemctl enable unattended-upgrades
log_success "Automatic security updates configured"

# Create application directory
APP_DIR="/opt/nfl-$ENVIRONMENT"
log_info "Creating application directory: $APP_DIR"
sudo mkdir -p "$APP_DIR"
sudo chown $USER:$USER "$APP_DIR"
log_success "Application directory created"

# Create data directory with proper permissions
DATA_DIR="$APP_DIR/data/mongodb"
log_info "Creating data directory: $DATA_DIR"
mkdir -p "$DATA_DIR"
# MongoDB container runs as user 999
sudo chown -R 999:999 "$DATA_DIR"
log_success "Data directory created with proper permissions"

# Create logs directory
LOGS_DIR="$APP_DIR/logs"
log_info "Creating logs directory: $LOGS_DIR"
mkdir -p "$LOGS_DIR"
log_success "Logs directory created"

# Create environment-specific .env file
ENV_FILE="$APP_DIR/.env"
log_info "Creating environment file: $ENV_FILE"

case $ENVIRONMENT in
    "test")
        cat > "$ENV_FILE" << EOF
# NFL Playoff Pool - Test Environment Configuration
ASPNETCORE_ENVIRONMENT=Development
WEBAPP_PORT=8080

# MongoDB Configuration
MONGODB_PORT=27018
MONGODB_DATABASE=playoff_pool_test
MONGODB_ROOT_PASSWORD=TestMongo123!@#
MONGODB_DATA_PATH=$DATA_DIR

# Admin Account Configuration
ADMIN_EMAIL=admin@test.local
ADMIN_PASSWORD=TestAdmin123!@#
ADMIN_FIRST_NAME=Test
ADMIN_LAST_NAME=Admin
EOF
        ;;
    "staging")
        cat > "$ENV_FILE" << EOF
# NFL Playoff Pool - Staging Environment Configuration
ASPNETCORE_ENVIRONMENT=Staging
WEBAPP_PORT=8081

# MongoDB Configuration
MONGODB_PORT=27019
MONGODB_DATABASE=playoff_pool_staging
MONGODB_ROOT_PASSWORD=StagingMongo123!@#
MONGODB_DATA_PATH=$DATA_DIR

# Admin Account Configuration
ADMIN_EMAIL=admin@staging.local
ADMIN_PASSWORD=StagingAdmin123!@#
ADMIN_FIRST_NAME=Staging
ADMIN_LAST_NAME=Admin
EOF
        ;;
    "production")
        cat > "$ENV_FILE" << EOF
# NFL Playoff Pool - Production Environment Configuration
ASPNETCORE_ENVIRONMENT=Production
WEBAPP_PORT=5000

# MongoDB Configuration
MONGODB_PORT=27017
MONGODB_DATABASE=playoff_pool
MONGODB_ROOT_PASSWORD=CHANGE_ME_SECURE_PASSWORD
MONGODB_DATA_PATH=$DATA_DIR

# Admin Account Configuration
ADMIN_EMAIL=CHANGE_ME_ADMIN_EMAIL
ADMIN_PASSWORD=CHANGE_ME_SECURE_PASSWORD
ADMIN_FIRST_NAME=CHANGE_ME
ADMIN_LAST_NAME=CHANGE_ME
EOF
        log_warning "Production environment created with placeholder values"
        log_warning "Please update $ENV_FILE with secure production values"
        ;;
esac

log_success "Environment file created"

# Create systemd service for the application
SERVICE_FILE="/etc/systemd/system/nfl-$ENVIRONMENT.service"
log_info "Creating systemd service: $SERVICE_FILE"

sudo tee "$SERVICE_FILE" > /dev/null << EOF
[Unit]
Description=NFL Playoff Pool ($ENVIRONMENT)
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=$APP_DIR
ExecStart=/usr/local/bin/docker-compose up -d
ExecStop=/usr/local/bin/docker-compose down
TimeoutStartSec=0
User=$USER
Group=$USER

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload
sudo systemctl enable "nfl-$ENVIRONMENT"
log_success "Systemd service created and enabled"

# Create backup script
BACKUP_SCRIPT="$APP_DIR/backup.sh"
log_info "Creating backup script: $BACKUP_SCRIPT"

cat > "$BACKUP_SCRIPT" << 'EOF'
#!/bin/bash
# NFL Playoff Pool Backup Script

BACKUP_DIR="$APP_DIR/backups"
DATE=$(date +%Y%m%d_%H%M%S)
CONTAINER_NAME="nflpool-mongodb"

# Load environment variables
set -a
source "$APP_DIR/.env"
set +a

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Backup MongoDB
docker exec "$CONTAINER_NAME" mongodump --authenticationDatabase admin -u admin -p "$MONGODB_ROOT_PASSWORD" --out /tmp/backup_$DATE

# Copy backup from container
docker cp "$CONTAINER_NAME:/tmp/backup_$DATE" "$BACKUP_DIR/"

# Compress backup
cd "$BACKUP_DIR"
tar -czf "mongodb_backup_$DATE.tar.gz" "backup_$DATE/"
rm -rf "backup_$DATE/"

# Keep only last 7 days of backups
find "$BACKUP_DIR" -name "mongodb_backup_*.tar.gz" -mtime +7 -delete

echo "Backup completed: mongodb_backup_$DATE.tar.gz"
EOF

chmod +x "$BACKUP_SCRIPT"
log_success "Backup script created"

# Create health check script
HEALTH_SCRIPT="$APP_DIR/health-check.sh"
log_info "Creating health check script: $HEALTH_SCRIPT"

cat > "$HEALTH_SCRIPT" << EOF
#!/bin/bash
# Health check script for NFL Playoff Pool ($ENVIRONMENT)

# Load environment variables
set -a
source "$APP_DIR/.env"
set +a

HEALTH_URL="http://localhost:\$WEBAPP_PORT/health"
LOG_FILE="$LOGS_DIR/health.log"

if curl -f "\$HEALTH_URL" > /dev/null 2>&1; then
    echo "\$(date): Application healthy" >> "\$LOG_FILE"
else
    echo "\$(date): Application unhealthy - restarting" >> "\$LOG_FILE"
    cd "$APP_DIR"
    docker-compose restart webapp
fi
EOF

chmod +x "$HEALTH_SCRIPT"
log_success "Health check script created"

# Setup log rotation
LOG_ROTATE_FILE="/etc/logrotate.d/nfl-$ENVIRONMENT"
log_info "Setting up log rotation: $LOG_ROTATE_FILE"

sudo tee "$LOG_ROTATE_FILE" > /dev/null << EOF
$LOGS_DIR/*.log {
    daily
    missingok
    rotate 30
    compress
    delaycompress
    notifempty
    create 644 $USER $USER
}
EOF

log_success "Log rotation configured"

# Setup cron jobs
log_info "Setting up cron jobs..."
(crontab -l 2>/dev/null; echo "0 2 * * * $BACKUP_SCRIPT >> $LOGS_DIR/backup.log 2>&1") | crontab -
(crontab -l 2>/dev/null; echo "*/5 * * * * $HEALTH_SCRIPT") | crontab -
log_success "Cron jobs configured"

# Display system information
log_info "System Information:"
echo "  OS: $(lsb_release -d | cut -f2)"
echo "  Kernel: $(uname -r)"
echo "  Docker: $(docker --version)"
echo "  Docker Compose: $(docker-compose --version)"
echo "  Application Directory: $APP_DIR"
echo "  Environment: $ENVIRONMENT"

# Display next steps
log_success "Ubuntu VM setup completed successfully!"
echo ""
log_info "Next Steps:"
echo "1. Copy your application files to: $APP_DIR"
echo "2. Update environment variables in: $ENV_FILE"
echo "3. Deploy the application: cd $APP_DIR && ./deploy.sh"
echo "4. Check service status: sudo systemctl status nfl-$ENVIRONMENT"
echo ""
log_info "Useful Commands:"
echo "  View logs: docker-compose logs -f"
echo "  Restart app: sudo systemctl restart nfl-$ENVIRONMENT"
echo "  Manual backup: $BACKUP_SCRIPT"
echo "  Health check: $HEALTH_SCRIPT"

# Logout message for docker group
log_warning "Please logout and login again for Docker group membership to take effect"