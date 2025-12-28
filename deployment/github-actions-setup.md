# GitHub Actions Automated Deployment Setup

This guide will help you set up automated deployment from GitHub to your home server using GitHub Actions.

## Prerequisites

1. **Home Server Setup**: Follow the [Proxmox Deployment Guide](./proxmox-deployment-guide.md) first
2. **GitHub Repository**: Your code should be in a GitHub repository
3. **SSH Access**: SSH key-based authentication to your home server

## Phase 1: Home Server Preparation

### 1.1 Create Deployment User (Recommended)

```bash
# On your home server, create a dedicated deployment user
sudo adduser deploy
sudo usermod -aG docker deploy
sudo mkdir -p /home/deploy/.ssh

# Set up directory permissions for the application
sudo chown -R deploy:deploy /opt/nflplayoffpool
```

### 1.2 Generate SSH Key for GitHub Actions

```bash
# On your local machine or server, generate SSH key pair
ssh-keygen -t ed25519 -C "github-actions-deploy" -f ~/.ssh/github-actions-deploy

# Copy public key to your home server
ssh-copy-id -i ~/.ssh/github-actions-deploy.pub deploy@YOUR_HOME_SERVER_IP

# Test SSH connection
ssh -i ~/.ssh/github-actions-deploy deploy@YOUR_HOME_SERVER_IP
```

### 1.3 Prepare Application Directory

```bash
# On your home server, ensure git is configured
cd /opt/nflplayoffpool
git remote set-url origin https://github.com/YOUR_USERNAME/YOUR_REPO.git

# Make sure deploy user can access the directory
sudo chown -R deploy:deploy /opt/nflplayoffpool
```

## Phase 2: GitHub Repository Configuration

### 2.1 Required GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions, and add these secrets:

| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `HOME_SERVER_HOST` | IP address or hostname of your server | `192.168.1.100` |
| `HOME_SERVER_USER` | SSH username for deployment | `deploy` |
| `HOME_SERVER_SSH_KEY` | Private SSH key content | Contents of `~/.ssh/github-actions-deploy` |
| `MONGODB_ROOT_PASSWORD` | Secure MongoDB password | `MyS3cur3M0ng0P@ssw0rd!2024` |
| `ADMIN_PASSWORD` | Secure admin account password | `MyS3cur3@dm1nP@ssw0rd!2024` |
| `ADMIN_EMAIL` | Admin email address | `admin@yourdomain.com` |

### 2.2 Setting Up Secrets

1. **HOME_SERVER_SSH_KEY**: Copy the entire content of your private key file:
   ```bash
   cat ~/.ssh/github-actions-deploy
   ```
   Copy everything including `-----BEGIN OPENSSH PRIVATE KEY-----` and `-----END OPENSSH PRIVATE KEY-----`

2. **Password Requirements**: Follow the security standards:
   - Minimum 12 characters
   - Include uppercase, lowercase, numbers, and symbols
   - Never use default or weak passwords

### 2.3 Enable GitHub Container Registry

The workflow uses GitHub Container Registry (ghcr.io) to store Docker images:

1. Go to your GitHub repository → Settings → Actions → General
2. Under "Workflow permissions", select "Read and write permissions"
3. Check "Allow GitHub Actions to create and approve pull requests"

## Phase 3: Workflow Configuration

### 3.1 Understanding the Workflow

The GitHub Actions workflow (`deploy-to-home-server.yml`) does:

1. **Build Phase**:
   - Builds Docker image from your code
   - Pushes image to GitHub Container Registry
   - Tags with branch name and commit SHA

2. **Deploy Phase**:
   - Connects to your home server via SSH
   - Pulls latest code
   - Creates backup of current deployment
   - Updates environment configuration
   - Pulls and deploys new Docker image
   - Verifies deployment health

3. **Notification Phase**:
   - Reports deployment success/failure

### 3.2 Triggering Deployments

**Automatic Deployment**:
- Every push to `main` branch triggers deployment

**Manual Deployment**:
- Go to Actions tab in GitHub
- Select "Deploy to Home Server" workflow
- Click "Run workflow"
- Choose environment (production/staging)

## Phase 4: Testing Your Setup

### 4.1 Initial Test Deployment

1. **Commit and Push**: Make a small change and push to main branch
2. **Monitor Workflow**: Go to Actions tab and watch the deployment
3. **Check Logs**: Review both build and deploy job logs
4. **Verify Application**: Check your application is accessible

### 4.2 Troubleshooting Common Issues

**SSH Connection Failed**:
```bash
# Test SSH connection manually
ssh -i ~/.ssh/github-actions-deploy deploy@YOUR_HOME_SERVER_IP

# Check SSH key format in GitHub secrets (should include headers)
```

**Docker Permission Denied**:
```bash
# On home server, ensure deploy user is in docker group
sudo usermod -aG docker deploy
# User needs to log out and back in for group changes
```

**Application Not Responding**:
```bash
# On home server, check container status
cd /opt/nflplayoffpool
docker-compose ps
docker-compose logs webapp

# Check if port 80 is available
sudo netstat -tlnp | grep :80
```

## Phase 5: Advanced Configuration

### 5.1 Environment-Specific Deployments

Create different environments in GitHub:

1. Go to Settings → Environments
2. Create "production" and "staging" environments
3. Add environment-specific secrets
4. Configure protection rules (require reviews, etc.)

### 5.2 Rollback Strategy

If deployment fails, you can rollback:

```bash
# On home server, rollback to previous version
cd /opt/nflplayoffpool
git log --oneline -5  # See recent commits
git reset --hard PREVIOUS_COMMIT_SHA
./deploy.sh
```

### 5.3 Monitoring and Alerts

Add monitoring to your workflow:

```yaml
# Add to workflow for Slack/Discord notifications
- name: Notify Slack
  if: failure()
  uses: 8398a7/action-slack@v3
  with:
    status: failure
    webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

## Phase 6: Security Best Practices

### 6.1 SSH Security

```bash
# On home server, harden SSH configuration
sudo nano /etc/ssh/sshd_config

# Add these settings:
PermitRootLogin no
PasswordAuthentication no
PubkeyAuthentication yes
AllowUsers deploy

sudo systemctl restart sshd
```

### 6.2 Firewall Configuration

```bash
# Only allow necessary ports
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw deny 27017/tcp  # Block direct MongoDB access
sudo ufw --force enable
```

### 6.3 Regular Security Updates

```bash
# Set up automatic security updates
sudo apt install unattended-upgrades
sudo dpkg-reconfigure unattended-upgrades

# Configure in /etc/apt/apt.conf.d/50unattended-upgrades
```

## Quick Reference Commands

### GitHub Actions
```bash
# View workflow runs
gh run list

# View specific run logs
gh run view RUN_ID --log

# Trigger manual deployment
gh workflow run deploy-to-home-server.yml
```

### Home Server Management
```bash
# Check deployment status
cd /opt/nflplayoffpool && docker-compose ps

# View application logs
docker-compose logs -f webapp

# Manual deployment
./deploy.sh

# Rollback deployment
git reset --hard HEAD~1 && ./deploy.sh
```

### Monitoring
```bash
# Check system resources
htop
df -h

# Check application health
curl http://localhost/health

# View deployment history
git log --oneline -10
```

This setup provides you with:
- ✅ Automated deployment on every push to main
- ✅ Manual deployment triggers
- ✅ Secure SSH-based deployment
- ✅ Docker image caching via GitHub Container Registry
- ✅ Automatic backups before deployment
- ✅ Health checks and rollback capability
- ✅ Environment-specific configurations

Your home server will now automatically deploy whenever you push code to GitHub!