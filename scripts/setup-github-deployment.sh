#!/bin/bash

# GitHub Actions Deployment Setup Script
# Run this on your home server to prepare for automated deployment

set -e

echo "🚀 Setting up GitHub Actions deployment for NFL Playoff Pool"
echo "============================================================"

# Check if running as root
if [ "$EUID" -eq 0 ]; then
    echo "❌ Please don't run this script as root"
    echo "💡 Run as your regular user, sudo will be used when needed"
    exit 1
fi

# Get configuration
read -p "Enter your GitHub username: " GITHUB_USER
read -p "Enter your repository name: " REPO_NAME
read -p "Enter deployment user name (default: deploy): " DEPLOY_USER
DEPLOY_USER=${DEPLOY_USER:-deploy}

echo ""
echo "📋 Configuration Summary:"
echo "GitHub Repository: https://github.com/$GITHUB_USER/$REPO_NAME"
echo "Deployment User: $DEPLOY_USER"
echo "Application Path: /opt/nflplayoffpool"
echo ""

read -p "Continue with setup? (y/N): " CONFIRM
if [[ ! $CONFIRM =~ ^[Yy]$ ]]; then
    echo "Setup cancelled"
    exit 0
fi

echo ""
echo "🔧 Phase 1: Creating deployment user..."

# Create deployment user if it doesn't exist
if ! id "$DEPLOY_USER" &>/dev/null; then
    sudo adduser --disabled-password --gecos "" $DEPLOY_USER
    echo "✅ Created user: $DEPLOY_USER"
else
    echo "✅ User $DEPLOY_USER already exists"
fi

# Add to docker group
sudo usermod -aG docker $DEPLOY_USER
echo "✅ Added $DEPLOY_USER to docker group"

# Create SSH directory
sudo mkdir -p /home/$DEPLOY_USER/.ssh
sudo chown $DEPLOY_USER:$DEPLOY_USER /home/$DEPLOY_USER/.ssh
sudo chmod 700 /home/$DEPLOY_USER/.ssh
echo "✅ Created SSH directory"

echo ""
echo "🔧 Phase 2: Setting up application directory..."

# Create application directory
sudo mkdir -p /opt/nflplayoffpool
sudo chown -R $DEPLOY_USER:$DEPLOY_USER /opt/nflplayoffpool
echo "✅ Created /opt/nflplayoffpool"

# Clone repository if it doesn't exist
if [ ! -d "/opt/nflplayoffpool/.git" ]; then
    echo "📥 Cloning repository..."
    sudo -u $DEPLOY_USER git clone https://github.com/$GITHUB_USER/$REPO_NAME.git /tmp/repo-clone
    sudo -u $DEPLOY_USER cp -r /tmp/repo-clone/* /opt/nflplayoffpool/
    sudo -u $DEPLOY_USER cp -r /tmp/repo-clone/.git /opt/nflplayoffpool/
    sudo -u $DEPLOY_USER cp /tmp/repo-clone/.gitignore /opt/nflplayoffpool/ 2>/dev/null || true
    rm -rf /tmp/repo-clone
    echo "✅ Repository cloned"
else
    echo "✅ Repository already exists"
fi

# Set up git configuration
cd /opt/nflplayoffpool
sudo -u $DEPLOY_USER git config --global --add safe.directory /opt/nflplayoffpool
sudo -u $DEPLOY_USER git remote set-url origin https://github.com/$GITHUB_USER/$REPO_NAME.git
echo "✅ Git configuration updated"

echo ""
echo "🔧 Phase 3: Generating SSH keys..."

# Generate SSH key for GitHub Actions
SSH_KEY_PATH="/home/$DEPLOY_USER/.ssh/github-actions-deploy"
if [ ! -f "$SSH_KEY_PATH" ]; then
    sudo -u $DEPLOY_USER ssh-keygen -t ed25519 -C "github-actions-deploy" -f "$SSH_KEY_PATH" -N ""
    echo "✅ Generated SSH key pair"
else
    echo "✅ SSH key already exists"
fi

# Add public key to authorized_keys
sudo -u $DEPLOY_USER cat "$SSH_KEY_PATH.pub" >> /home/$DEPLOY_USER/.ssh/authorized_keys
sudo -u $DEPLOY_USER chmod 600 /home/$DEPLOY_USER/.ssh/authorized_keys
echo "✅ Added public key to authorized_keys"

echo ""
echo "🔧 Phase 4: Setting up directories and permissions..."

# Create data directory
sudo mkdir -p /opt/nflplayoffpool/data/mongodb
sudo chown -R 999:999 /opt/nflplayoffpool/data/mongodb
echo "✅ Created MongoDB data directory"

# Create backups directory
sudo mkdir -p /opt/nflplayoffpool/backups
sudo chown $DEPLOY_USER:$DEPLOY_USER /opt/nflplayoffpool/backups
echo "✅ Created backups directory"

# Make scripts executable
sudo chmod +x /opt/nflplayoffpool/deploy.sh 2>/dev/null || echo "⚠️ deploy.sh not found"
sudo chmod +x /opt/nflplayoffpool/scripts/*.sh 2>/dev/null || echo "⚠️ No scripts found"

echo ""
echo "🔧 Phase 5: Configuring firewall..."

# Configure UFW if installed
if command -v ufw &> /dev/null; then
    sudo ufw allow ssh
    sudo ufw allow 80/tcp
    sudo ufw allow 443/tcp
    sudo ufw --force enable
    echo "✅ Firewall configured"
else
    echo "⚠️ UFW not installed, skipping firewall configuration"
fi

echo ""
echo "🎉 Setup completed successfully!"
echo ""
echo "📋 Next Steps:"
echo "=============="
echo ""
echo "1. 🔑 Add this SSH private key to your GitHub repository secrets as 'HOME_SERVER_SSH_KEY':"
echo "   Copy the entire content including headers:"
echo ""
sudo -u $DEPLOY_USER cat "$SSH_KEY_PATH"
echo ""
echo "2. 🌐 Add these GitHub repository secrets:"
echo "   - HOME_SERVER_HOST: $(hostname -I | awk '{print $1}')"
echo "   - HOME_SERVER_USER: $DEPLOY_USER"
echo "   - MONGODB_ROOT_PASSWORD: (generate a secure password)"
echo "   - ADMIN_PASSWORD: (generate a secure password)"
echo "   - ADMIN_EMAIL: (your admin email)"
echo ""
echo "3. 🔒 Password Requirements:"
echo "   - Minimum 12 characters"
echo "   - Include uppercase, lowercase, numbers, and symbols"
echo "   - Example: MyS3cur3P@ssw0rd!2024"
echo ""
echo "4. 🚀 Test SSH connection from your local machine:"
echo "   ssh -i /path/to/private/key $DEPLOY_USER@$(hostname -I | awk '{print $1}')"
echo ""
echo "5. 📖 Follow the complete setup guide:"
echo "   deployment/github-actions-setup.md"
echo ""
echo "🔧 Server Information:"
echo "====================="
echo "Server IP: $(hostname -I | awk '{print $1}')"
echo "Deployment User: $DEPLOY_USER"
echo "Application Path: /opt/nflplayoffpool"
echo "SSH Key Path: $SSH_KEY_PATH"
echo ""
echo "✅ Your server is ready for GitHub Actions deployment!"