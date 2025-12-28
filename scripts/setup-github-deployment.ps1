# GitHub Actions Deployment Setup Script (PowerShell)
# Run this script to generate the SSH keys and configuration for GitHub Actions

param(
    [Parameter(Mandatory=$true)]
    [string]$GitHubUser,
    
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    
    [Parameter(Mandatory=$true)]
    [string]$HomeServerIP,
    
    [string]$DeployUser = "deploy"
)

Write-Host "🚀 GitHub Actions Deployment Setup" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "GitHub Repository: https://github.com/$GitHubUser/$RepoName"
Write-Host "Home Server IP: $HomeServerIP"
Write-Host "Deploy User: $DeployUser"
Write-Host ""

# Generate SSH key pair
$sshKeyPath = "$env:USERPROFILE\.ssh\github-actions-deploy"
$sshKeyDir = Split-Path $sshKeyPath -Parent

# Create .ssh directory if it doesn't exist
if (!(Test-Path $sshKeyDir)) {
    New-Item -ItemType Directory -Path $sshKeyDir -Force | Out-Null
    Write-Host "✅ Created .ssh directory" -ForegroundColor Green
}

# Generate SSH key if it doesn't exist
if (!(Test-Path $sshKeyPath)) {
    Write-Host "🔑 Generating SSH key pair..." -ForegroundColor Yellow
    ssh-keygen -t ed25519 -C "github-actions-deploy" -f $sshKeyPath -N '""'
    Write-Host "✅ SSH key pair generated" -ForegroundColor Green
} else {
    Write-Host "✅ SSH key already exists" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎉 Setup completed!" -ForegroundColor Green
Write-Host ""

Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host "=============="
Write-Host ""

Write-Host "1. 📤 Copy the public key to your home server:" -ForegroundColor Cyan
Write-Host "   Run this command on your home server:"
Write-Host "   ssh-copy-id -i ~/.ssh/github-actions-deploy.pub $DeployUser@$HomeServerIP" -ForegroundColor Gray
Write-Host ""
Write-Host "   Or manually add this public key to ~/.ssh/authorized_keys on your server:"
Get-Content "$sshKeyPath.pub" | Write-Host -ForegroundColor Gray
Write-Host ""

Write-Host "2. 🔑 Add this private key to GitHub repository secrets as 'HOME_SERVER_SSH_KEY':" -ForegroundColor Cyan
Write-Host "   Copy the entire content including headers:"
Write-Host ""
Get-Content $sshKeyPath | Write-Host -ForegroundColor Gray
Write-Host ""

Write-Host "3. 🌐 Add these GitHub repository secrets:" -ForegroundColor Cyan
Write-Host "   - HOME_SERVER_HOST: $HomeServerIP"
Write-Host "   - HOME_SERVER_USER: $DeployUser"
Write-Host "   - MONGODB_ROOT_PASSWORD: (generate a secure password)"
Write-Host "   - ADMIN_PASSWORD: (generate a secure password)"
Write-Host "   - ADMIN_EMAIL: (your admin email)"
Write-Host ""

Write-Host "4. 🔒 Password Requirements:" -ForegroundColor Cyan
Write-Host "   - Minimum 12 characters"
Write-Host "   - Include uppercase, lowercase, numbers, and symbols"
Write-Host "   - Example: MyS3cur3P@ssw0rd!2024"
Write-Host ""

Write-Host "5. 🧪 Test SSH connection:" -ForegroundColor Cyan
Write-Host "   ssh -i $sshKeyPath $DeployUser@$HomeServerIP"
Write-Host ""

Write-Host "6. 🏠 Run the setup script on your home server:" -ForegroundColor Cyan
Write-Host "   ./scripts/setup-github-deployment.sh"
Write-Host ""

Write-Host "📖 For complete instructions, see: deployment/github-actions-setup.md" -ForegroundColor Yellow
Write-Host ""
Write-Host "✅ Your local machine is ready for GitHub Actions deployment!" -ForegroundColor Green