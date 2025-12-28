# GitHub Secrets Setup Guide

This guide helps you set up the required GitHub repository secrets for automated deployment.

## Required Secrets

### Production Deployment Secrets

Navigate to your GitHub repository → Settings → Secrets and variables → Actions → New repository secret

| Secret Name | Description | Example/Notes |
|-------------|-------------|---------------|
| `HOME_SERVER_HOST` | Your home server IP address | `192.168.1.100` |
| `HOME_SERVER_USER` | SSH username for deployment | `deploy` |
| `HOME_SERVER_SSH_KEY` | Private SSH key for server access | Copy from setup script output |
| `MONGODB_ROOT_PASSWORD` | Secure MongoDB root password | `MyS3cur3M0ng0P@ssw0rd!2024` |
| `ADMIN_PASSWORD` | Secure admin account password | `MyS3cur3@dm1nP@ssw0rd!2024` |
| `ADMIN_EMAIL` | Admin email address | `admin@yourdomain.com` |

### Optional: Staging Environment Secrets

If you want a staging environment, add these additional secrets:

| Secret Name | Description |
|-------------|-------------|
| `STAGING_SERVER_HOST` | Staging server IP |
| `STAGING_SERVER_USER` | Staging SSH username |
| `STAGING_SERVER_SSH_KEY` | Staging SSH private key |
| `STAGING_MONGODB_PASSWORD` | Staging MongoDB password |
| `STAGING_ADMIN_PASSWORD` | Staging admin password |
| `STAGING_ADMIN_EMAIL` | Staging admin email |

## Password Security Requirements

All passwords MUST meet these requirements:
- ✅ Minimum 12 characters
- ✅ Include uppercase letters (A-Z)
- ✅ Include lowercase letters (a-z)
- ✅ Include numbers (0-9)
- ✅ Include symbols (!@#$%^&*)

### Good Password Examples:
- `MyS3cur3P@ssw0rd!2024`
- `N3wM0ng0DB#P@ssw0rd!`
- `@dm1nU$er!S3cur3P@ss`

### ❌ Bad Passwords (Don't Use):
- `password123` (too simple)
- `Admin123!` (too short)
- `CHANGE_ME_SECURE_PASSWORD` (default value)

## Step-by-Step Secret Setup

### 1. Get SSH Key from Setup Script

After running `./scripts/setup-github-deployment.sh` on your server, copy the private key:

```bash
# The setup script will output something like this:
cat /home/deploy/.ssh/github-actions-deploy
```

Copy the ENTIRE output including the headers:
```
-----BEGIN OPENSSH PRIVATE KEY-----
b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAFwAAAAdzc2gtcn
[... many lines of key data ...]
-----END OPENSSH PRIVATE KEY-----
```

### 2. Add Secrets to GitHub

1. Go to your repository on GitHub
2. Click **Settings** tab
3. Click **Secrets and variables** → **Actions**
4. Click **New repository secret**
5. Add each secret from the table above

### 3. Generate Secure Passwords

Use a password manager or generate secure passwords:

```bash
# Generate secure passwords (Linux/Mac)
openssl rand -base64 32 | tr -d "=+/" | cut -c1-16

# Or use online generators (ensure they meet requirements)
# https://passwordsgenerator.net/
```

### 4. Test Your Setup

After adding all secrets:

1. **Push a change** to your main branch
2. **Go to Actions tab** in GitHub
3. **Watch the deployment** workflow run
4. **Check your server** - the app should be deployed!

## Troubleshooting

### SSH Connection Issues
```bash
# Test SSH connection manually
ssh -i /path/to/private/key deploy@YOUR_SERVER_IP

# Check SSH key format (should have headers)
cat ~/.ssh/github-actions-deploy
```

### Password Validation Errors
- Ensure passwords meet all security requirements
- Check for special characters that might need escaping
- Verify no trailing spaces in secret values

### Deployment Failures
- Check GitHub Actions logs for specific errors
- Verify all required secrets are set
- Ensure server is accessible and running

## Security Notes

- 🔒 **Never commit secrets** to your repository
- 🔑 **Use unique passwords** for each environment
- 🔄 **Rotate passwords regularly** (every 90 days)
- 👥 **Limit access** to production secrets
- 📝 **Document** who has access to what secrets

## Quick Verification Checklist

- [ ] All required secrets added to GitHub
- [ ] Passwords meet security requirements
- [ ] SSH key copied correctly (with headers)
- [ ] Server IP address is correct
- [ ] Deploy user exists on server
- [ ] SSH connection works manually
- [ ] First deployment workflow completed successfully

Once all secrets are configured, your automated deployment will work seamlessly!