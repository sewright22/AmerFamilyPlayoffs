# NFL Playoff Pool - Deployment Checklist

Use this checklist to track your deployment progress.

## Pre-Deployment Preparation

- [ ] Proxmox server is accessible
- [ ] Ubuntu 22.04 LTS ISO downloaded to Proxmox
- [ ] CloudFlare account created
- [ ] Domain name configured in CloudFlare
- [ ] Secure passwords generated for:
  - [ ] MongoDB root password
  - [ ] Admin account password
  - [ ] Ubuntu user password

## Phase 1: VM Setup (30 minutes)

- [ ] Create Ubuntu VM in Proxmox (VM ID: _______)
  - [ ] 2 CPU cores minimum
  - [ ] 4GB RAM minimum
  - [ ] 32GB disk minimum
  - [ ] VirtIO network adapter
- [ ] Install Ubuntu Server 22.04 LTS
- [ ] Configure SSH access
- [ ] Note VM IP address: ___________________
- [ ] SSH into VM successfully

## Phase 2: System Configuration (20 minutes)

- [ ] Update system packages (`sudo apt update && sudo apt upgrade -y`)
- [ ] Install Docker
- [ ] Install Docker Compose
- [ ] Add user to docker group
- [ ] Configure timezone
- [ ] Setup firewall (UFW)
- [ ] Create `/opt/nflplayoffpool` directory
- [ ] Set directory ownership

## Phase 3: Application Deployment (15 minutes)

- [ ] Transfer/clone application code to VM
- [ ] Create `.env` file from template
- [ ] Configure production environment variables:
  - [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
  - [ ] Set `WEBAPP_PORT=80`
  - [ ] Set secure `MONGODB_ROOT_PASSWORD`
  - [ ] Set secure `ADMIN_PASSWORD`
  - [ ] Configure admin email
- [ ] Create MongoDB data directory
- [ ] Set proper permissions on data directory
- [ ] Run `./deploy.sh`
- [ ] Verify containers are running (`docker-compose ps`)
- [ ] Test health endpoint (`curl http://localhost/health`)
- [ ] Access application locally (http://VM-IP)

## Phase 4: CloudFlare Tunnel (20 minutes)

- [ ] Install cloudflared on VM
- [ ] Authenticate with CloudFlare (`cloudflared tunnel login`)
- [ ] Create tunnel (`cloudflared tunnel create nfl-playoff-pool`)
- [ ] Note tunnel ID: ___________________
- [ ] Create tunnel configuration file
- [ ] Configure DNS record
- [ ] Test tunnel manually
- [ ] Install tunnel as systemd service
- [ ] Enable and start cloudflared service
- [ ] Verify external access via domain

## Phase 5: Production Hardening (30 minutes)

- [ ] Create systemd service for application
- [ ] Enable application service
- [ ] Create backup script
- [ ] Configure automated daily backups (cron)
- [ ] Setup log rotation
- [ ] Create health check script
- [ ] Configure health check monitoring (cron)
- [ ] Create update script
- [ ] Test backup script manually
- [ ] Test update script manually

## Phase 6: Testing and Validation (15 minutes)

- [ ] Access application via CloudFlare domain
- [ ] Login with admin credentials
- [ ] Create test season
- [ ] Verify database persistence
- [ ] Test application restart
- [ ] Verify backup creation
- [ ] Check CloudFlare tunnel status
- [ ] Review application logs
- [ ] Monitor system resources

## Post-Deployment

- [ ] Document VM IP address
- [ ] Document CloudFlare tunnel ID
- [ ] Save backup of `.env` file (securely)
- [ ] Schedule regular maintenance window
- [ ] Set up monitoring alerts (optional)
- [ ] Create runbook for common operations
- [ ] Train other admins (if applicable)

## Maintenance Schedule

### Daily (Automated)
- [ ] Automated backups (2 AM)
- [ ] Health checks (every 5 minutes)

### Weekly
- [ ] Review application logs
- [ ] Check disk space
- [ ] Verify backups are working

### Monthly
- [ ] Update system packages
- [ ] Review and clean old backups
- [ ] Test restore procedure
- [ ] Review CloudFlare analytics

### As Needed
- [ ] Application updates
- [ ] Security patches
- [ ] Configuration changes

## Emergency Contacts

- Proxmox Admin: ___________________
- CloudFlare Account: ___________________
- Domain Registrar: ___________________
- Application Repository: ___________________

## Important Paths

- Application: `/opt/nflplayoffpool`
- Backups: `/opt/nflplayoffpool/backups`
- Logs: `/var/log/nfl-*.log`
- CloudFlare Config: `/etc/cloudflared/config.yml`
- Environment: `/opt/nflplayoffpool/.env`

## Quick Commands Reference

```bash
# Check application status
cd /opt/nflplayoffpool && docker-compose ps

# View logs
docker-compose logs -f webapp

# Restart application
docker-compose restart webapp

# Full redeploy
./deploy.sh

# Manual backup
./backup.sh

# Update application
./update.sh

# Check CloudFlare tunnel
sudo systemctl status cloudflared
```