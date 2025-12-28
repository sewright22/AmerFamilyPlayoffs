# Azure DevOps Setup Guide for NFL Playoff Pool

This guide explains how to configure Azure DevOps to automatically deploy to your Ubuntu VMs.

## Prerequisites

1. **Ubuntu VMs**: Set up using the `scripts/setup-ubuntu-vm.sh` script
2. **SSH Access**: Ensure your Azure DevOps agent can SSH to the VMs
3. **Azure DevOps Project**: With access to create pipelines and service connections

## Step 1: Create SSH Service Connections

In Azure DevOps, create SSH service connections for each environment:

### Test Environment Connection
1. Go to **Project Settings** → **Service connections**
2. Click **New service connection** → **SSH**
3. Configure:
   - **Connection name**: `NFL-VM-test`
   - **Host name**: `192.168.68.xxx` (your test VM IP)
   - **Port number**: `22`
   - **Username**: `testuser` (or your VM username)
   - **Authentication**: SSH key or password
   - **Private key**: Your SSH private key content
4. Test the connection and save

### Production Environment Connection
1. Create another SSH service connection
2. Configure:
   - **Connection name**: `NFL-VM-production`
   - **Host name**: `192.168.68.yyy` (your production VM IP)
   - **Port number**: `22`
   - **Username**: `produser` (or your VM username)
   - **Authentication**: SSH key or password
   - **Private key**: Your SSH private key content

## Step 2: Set Up VM Initial Configuration

Run the VM setup pipeline to configure your Ubuntu VMs:

### Option A: Use the Setup Pipeline
1. Create a new pipeline using `.azuredevops/setup-vm.yml`
2. Run with parameters:
   - **Environment**: `test`
   - **VM Host**: Your VM IP address
   - **VM User**: Your VM username

### Option B: Manual Setup
SSH into your VM and run:
```bash
# Transfer the setup script to your VM
scp scripts/setup-ubuntu-vm.sh user@vm-ip:/tmp/

# SSH into the VM
ssh user@vm-ip

# Run the setup script
chmod +x /tmp/setup-ubuntu-vm.sh
/tmp/setup-ubuntu-vm.sh test  # or 'production'
```

## Step 3: Configure Build Pipeline

The updated `build.yml` pipeline includes:

### Build Stage
- Builds Docker image with unique build ID
- Saves image as artifact for deployment

### Deploy to Test Stage
- Transfers application files to test VM
- Loads Docker image
- Runs deployment script
- Verifies application health

### Deploy to Production Stage
- Only runs on `main` branch
- Transfers files to production VM
- Deploys with production configuration

## Step 4: Environment Configuration

### Test Environment
The setup script creates `/opt/nfl-test/.env` with:
```bash
ASPNETCORE_ENVIRONMENT=Development
WEBAPP_PORT=8080
MONGODB_PORT=27018
# ... other test-specific settings
```

### Production Environment
The setup script creates `/opt/nfl-production/.env` with placeholders:
```bash
ASPNETCORE_ENVIRONMENT=Production
WEBAPP_PORT=5000
MONGODB_ROOT_PASSWORD=CHANGE_ME_SECURE_PASSWORD
ADMIN_PASSWORD=CHANGE_ME_SECURE_PASSWORD
# ... other production settings
```

**Important**: Update production `.env` file with secure values before deploying!

## Step 5: Pipeline Execution

### Automatic Deployment
- **Test**: Deploys on every commit to any branch
- **Production**: Only deploys on commits to `main` branch

### Manual Deployment
1. Go to **Pipelines** → **Builds**
2. Select your build pipeline
3. Click **Run pipeline**
4. Choose branch and run

## Step 6: Monitoring and Verification

### Application URLs
- **Test**: `http://test-vm-ip:8080`
- **Production**: `http://prod-vm-ip:5000`

### Health Checks
- **Test**: `http://test-vm-ip:8080/health`
- **Production**: `http://prod-vm-ip:5000/health`

### SSH Commands for Troubleshooting
```bash
# Check application status
sudo systemctl status nfl-test  # or nfl-production

# View container logs
cd /opt/nfl-test  # or /opt/nfl-production
docker-compose logs -f webapp

# Restart application
sudo systemctl restart nfl-test

# Manual deployment
cd /opt/nfl-test
./scripts/deploy-from-azdo.sh test latest
```

## Security Considerations

### SSH Keys
- Use dedicated SSH keys for Azure DevOps
- Restrict key permissions to deployment-only operations
- Rotate keys regularly

### Environment Variables
- Never commit production secrets to source control
- Use Azure DevOps variable groups for sensitive data
- Update default passwords before production deployment

### Network Security
- Configure firewall rules appropriately
- Use VPN or private networks when possible
- Monitor access logs regularly

## Troubleshooting

### Common Issues

**SSH Connection Failed**
- Verify VM IP address and SSH service
- Check SSH key format and permissions
- Ensure firewall allows SSH (port 22)

**Deployment Failed**
- Check VM disk space: `df -h`
- Verify Docker service: `sudo systemctl status docker`
- Check application logs: `docker-compose logs webapp`

**Application Not Responding**
- Verify port configuration in `.env` file
- Check firewall rules for application port
- Review container health: `docker-compose ps`

### Log Locations
- **Deployment logs**: `/opt/nfl-{env}/logs/deployments.log`
- **Health check logs**: `/opt/nfl-{env}/logs/health.log`
- **Backup logs**: `/opt/nfl-{env}/logs/backup.log`
- **System logs**: `/var/log/syslog`

## Next Steps

1. **Set up monitoring**: Consider adding Grafana/Prometheus for monitoring
2. **Configure backups**: The setup script includes automated MongoDB backups
3. **SSL certificates**: Set up Let's Encrypt or CloudFlare for HTTPS
4. **Load balancing**: Add nginx reverse proxy for multiple instances

This setup provides a robust CI/CD pipeline for your NFL Playoff Pool application with proper environment separation and automated deployments.