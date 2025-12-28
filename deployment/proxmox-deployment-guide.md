# NFL Playoff Pool - Proxmox Deployment Guide

## Overview

This guide walks through deploying the NFL Playoff Pool application on a Proxmox home server using:
- **Proxmox VE** - Hypervisor host
- **Ubuntu 22.04 LTS VM** - Application host
- **Docker & Docker Compose** - Application containerization
- **CloudFlare Tunnel** - SSL termination and external access (no port forwarding needed)

## Phase 1: Ubuntu VM Setup on Proxmox

### 1.1 Create Ubuntu VM

1. **Download Ubuntu Server 22.04 LTS ISO**
   - Go to Proxmox web interface
   - Upload ISO to local storage

2. **Create VM in Proxmox**
   ```
   VM ID: 100 (or your preference)
   Name: nfl-playoff-pool
   OS: Linux 6.x - 2.6 Kernel
   ISO: ubuntu-22.04-server-amd64.iso
   
   System:
   - Machine: q35
   - BIOS: OVMF (UEFI)
   - Add EFI Disk: Yes
   - SCSI Controller: VirtIO SCSI single
   
   Hard Disk:
   - Bus/Device: VirtIO Block
   - Storage: local-lvm (or your storage)
   - Disk size: 32 GB (minimum)
   - Cache: Write back
   
   CPU:
   - Sockets: 1
   - Cores: 2 (minimum)
   - Type: host
   
   Memory:
   - Memory: 4096 MB (4GB minimum)
   - Ballooning: Yes
   
   Network:
   - Bridge: vmbr0
   - Model: VirtIO (paravirtualized)
   - Firewall: Yes
   ```

3. **Start VM and Install Ubuntu**
   - Boot from ISO
   - Follow Ubuntu Server installation
   - Create user account (e.g., `nflpool`)
   - Install OpenSSH server when prompted
   - No additional packages needed initially

### 1.2 Initial Ubuntu Configuration

SSH into your new VM and run these commands:

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install essential packages
sudo apt install -y curl wget git unzip htop nano

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add user to docker group
sudo usermod -aG docker $USER

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Logout and login again for docker group to take effect
exit
```

### 1.3 Configure VM for Production

```bash
# Set timezone
sudo timedatectl set-timezone America/New_York  # Adjust for your timezone

# Configure automatic security updates
sudo apt install -y unattended-upgrades
sudo dpkg-reconfigure -plow unattended-upgrades

# Configure firewall (we'll only allow SSH and HTTP since CloudFlare handles external access)
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw --force enable

# Create application directory
sudo mkdir -p /opt/nflplayoffpool
sudo chown $USER:$USER /opt/nflplayoffpool
```

## Phase 2: Application Deployment

### 2.1 Deploy Application Code

```bash
# Clone repository to VM
cd /opt/nflplayoffpool
git clone <your-repo-url> .

# Or if transferring from development machine:
# scp -r /path/to/NflPlayoffPool user@vm-ip:/opt/nflplayoffpool/
```

### 2.2 Configure Production Environment

```bash
# Copy environment template
cp .env.template .env

# Edit environment file
nano .env
```

**Production .env Configuration:**
```bash
# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
WEBAPP_PORT=80

# MongoDB Configuration
MONGODB_PORT=27017
MONGODB_DATABASE=playoff_pool
MONGODB_ROOT_PASSWORD=YourSecureMongoPassword123!@#
MONGODB_DATA_PATH=/opt/nflplayoffpool/data/mongodb

# Admin Account Configuration
ADMIN_EMAIL=admin@yourdomain.com
ADMIN_PASSWORD=YourSecureAdminPassword123!@#
ADMIN_FIRST_NAME=Admin
ADMIN_LAST_NAME=User
```

### 2.3 Deploy Application

```bash
# Create data directory with proper permissions
mkdir -p /opt/nflplayoffpool/data/mongodb
sudo chown -R 999:999 /opt/nflplayoffpool/data/mongodb

# Deploy using the provided script
chmod +x deploy.sh
./deploy.sh
```

### 2.4 Verify Deployment

```bash
# Check containers are running
docker-compose ps

# Test health endpoint
curl http://localhost/health

# View logs
docker-compose logs -f webapp
```

## Phase 3: CloudFlare Tunnel Setup

### 3.1 Install CloudFlare Tunnel

```bash
# Download and install cloudflared
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared-linux-amd64.deb

# Authenticate with CloudFlare (this will open a browser)
cloudflared tunnel login
```

### 3.2 Create and Configure Tunnel

```bash
# Create tunnel
cloudflared tunnel create nfl-playoff-pool

# Note the tunnel ID from the output

# Create tunnel configuration
sudo mkdir -p /etc/cloudflared
sudo nano /etc/cloudflared/config.yml
```

**CloudFlare Tunnel Configuration (`/etc/cloudflared/config.yml`):**
```yaml
tunnel: <your-tunnel-id>
credentials-file: /home/nflpool/.cloudflared/<your-tunnel-id>.json

ingress:
  - hostname: nflpool.yourdomain.com
    service: http://localhost:80
  - service: http_status:404
```

### 3.3 Configure DNS and Start Tunnel

```bash
# Create DNS record (replace with your domain)
cloudflared tunnel route dns nfl-playoff-pool nflpool.yourdomain.com

# Test tunnel
cloudflared tunnel run nfl-playoff-pool

# If working, install as service
sudo cloudflared service install
sudo systemctl enable cloudflared
sudo systemctl start cloudflared
```

## Phase 4: Production Hardening

### 4.1 Create Systemd Service for Application

```bash
sudo nano /etc/systemd/system/nfl-playoff-pool.service
```

```ini
[Unit]
Description=NFL Playoff Pool Application
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/nflplayoffpool
ExecStart=/usr/local/bin/docker-compose up -d
ExecStop=/usr/local/bin/docker-compose down
TimeoutStartSec=0

[Install]
WantedBy=multi-user.target
```

```bash
# Enable service
sudo systemctl daemon-reload
sudo systemctl enable nfl-playoff-pool
```

### 4.2 Setup Automated Backups

```bash
# Create backup script
sudo nano /opt/nflplayoffpool/backup.sh
```

```bash
#!/bin/bash
# NFL Playoff Pool Backup Script

BACKUP_DIR="/opt/nflplayoffpool/backups"
DATE=$(date +%Y%m%d_%H%M%S)
CONTAINER_NAME="nflpool-mongodb"

# Create backup directory
mkdir -p $BACKUP_DIR

# Backup MongoDB
docker exec $CONTAINER_NAME mongodump --authenticationDatabase admin -u admin -p $MONGODB_ROOT_PASSWORD --out /tmp/backup_$DATE

# Copy backup from container
docker cp $CONTAINER_NAME:/tmp/backup_$DATE $BACKUP_DIR/

# Compress backup
cd $BACKUP_DIR
tar -czf mongodb_backup_$DATE.tar.gz backup_$DATE/
rm -rf backup_$DATE/

# Keep only last 7 days of backups
find $BACKUP_DIR -name "mongodb_backup_*.tar.gz" -mtime +7 -delete

echo "Backup completed: mongodb_backup_$DATE.tar.gz"
```

```bash
# Make executable
chmod +x /opt/nflplayoffpool/backup.sh

# Add to crontab (daily backup at 2 AM)
crontab -e
# Add line: 0 2 * * * /opt/nflplayoffpool/backup.sh >> /var/log/nfl-backup.log 2>&1
```

### 4.3 Setup Log Rotation

```bash
sudo nano /etc/logrotate.d/nfl-playoff-pool
```

```
/opt/nflplayoffpool/logs/*.log {
    daily
    missingok
    rotate 30
    compress
    delaycompress
    notifempty
    create 644 nflpool nflpool
}
```

## Phase 5: Monitoring and Maintenance

### 5.1 Health Check Script

```bash
nano /opt/nflplayoffpool/health-check.sh
```

```bash
#!/bin/bash
# Health check script

HEALTH_URL="http://localhost/health"
LOG_FILE="/var/log/nfl-health.log"

if curl -f $HEALTH_URL > /dev/null 2>&1; then
    echo "$(date): Application healthy" >> $LOG_FILE
else
    echo "$(date): Application unhealthy - restarting" >> $LOG_FILE
    cd /opt/nflplayoffpool
    docker-compose restart webapp
fi
```

```bash
chmod +x /opt/nflplayoffpool/health-check.sh

# Add to crontab (check every 5 minutes)
crontab -e
# Add line: */5 * * * * /opt/nflplayoffpool/health-check.sh
```

### 5.2 Update Script

```bash
nano /opt/nflplayoffpool/update.sh
```

```bash
#!/bin/bash
# Update script for NFL Playoff Pool

cd /opt/nflplayoffpool

echo "Pulling latest code..."
git pull origin main

echo "Backing up before update..."
./backup.sh

echo "Rebuilding and restarting application..."
docker-compose down
docker-compose build --no-cache
docker-compose up -d

echo "Waiting for application to start..."
sleep 30

if curl -f http://localhost/health > /dev/null 2>&1; then
    echo "Update successful!"
else
    echo "Update failed - check logs"
    docker-compose logs webapp
fi
```

```bash
chmod +x /opt/nflplayoffpool/update.sh
```

## Quick Reference Commands

```bash
# Application Management
cd /opt/nflplayoffpool
docker-compose ps                    # Check status
docker-compose logs -f webapp        # View logs
docker-compose restart webapp        # Restart app
./deploy.sh                         # Full redeploy

# System Management
sudo systemctl status cloudflared    # Check tunnel status
sudo systemctl restart nfl-playoff-pool  # Restart app service
df -h                               # Check disk space
htop                                # Check system resources

# Backup and Restore
./backup.sh                         # Manual backup
ls -la backups/                     # List backups
# Restore: docker exec -i nflpool-mongodb mongorestore --authenticationDatabase admin -u admin -p $MONGODB_ROOT_PASSWORD /path/to/backup
```

## Troubleshooting

### Application Won't Start
```bash
docker-compose logs webapp
docker-compose logs mongodb
sudo systemctl status docker
```

### CloudFlare Tunnel Issues
```bash
sudo systemctl status cloudflared
sudo journalctl -u cloudflared -f
cloudflared tunnel info nfl-playoff-pool
```

### Performance Issues
```bash
htop                    # Check CPU/Memory
df -h                   # Check disk space
docker stats            # Check container resources
```

This setup gives you a production-ready deployment with SSL handled by CloudFlare, automated backups, monitoring, and easy maintenance procedures.