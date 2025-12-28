# NFL Playoff Pool - Deployment Guide

## Test Server Deployment

### Quick Deployment Commands

**SSH into test server:**
```bash
ssh -i ~/.ssh/github-actions-deploy deploy@192.168.68.105
```

**Deploy/Update Application:**
```bash
cd /opt/nflplayoffpool

# Pull latest code
git pull origin main

# Deploy with Docker Compose
docker-compose down
docker-compose up -d

# Check status
docker-compose ps
docker-compose logs webapp
```

**Access Application:**
- **URL**: http://192.168.68.105:5000
- **Admin Login**: Use credentials from your .env file

### Environment Configuration

**Test Environment (.env file):**
```bash
# Application Configuration - TEST ENVIRONMENT
ASPNETCORE_ENVIRONMENT=Development
WEBAPP_PORT=5000

# MongoDB Configuration
MONGODB_PORT=27017
MONGODB_DATABASE=playoff_pool_test
MONGODB_ROOT_PASSWORD=YOUR_SECURE_MONGODB_PASSWORD
MONGODB_DATA_PATH=./data/mongodb

# Admin Account Configuration
ADMIN_EMAIL=admin@test.local
ADMIN_PASSWORD=YOUR_SECURE_ADMIN_PASSWORD
ADMIN_FIRST_NAME=Test
ADMIN_LAST_NAME=Admin
```

**⚠️ SECURITY WARNING**: 
- Never commit actual passwords to the repository
- Use secure passwords (12+ characters, mixed case, numbers, symbols)
- The .env file is in .gitignore and should never be committed

### Troubleshooting Commands

**Check container status:**
```bash
docker-compose ps
docker-compose logs webapp
docker-compose logs mongodb
```

**Restart services:**
```bash
docker-compose restart webapp
docker-compose restart mongodb
```

**Clean deployment (removes data):**
```bash
docker-compose down -v
docker-compose up -d
```

**Check application health:**
```bash
curl http://localhost:5000/health
curl http://192.168.68.105:5000/health
```

### GitHub Actions Deployment

**Manual trigger:**
1. Go to: https://github.com/sewright22/AmerFamilyPlayoffs/actions
2. Select "Deploy to Test Server" workflow
3. Click "Run workflow"
4. Select environment: `test`

**Required GitHub Secrets (Environment: test):**
- `HOME_SERVER_SSH_KEY` - Private SSH key for deployment
- `HOME_SERVER_HOST` - `192.168.68.105`
- `HOME_SERVER_USER` - `deploy`
- `MONGODB_ROOT_PASSWORD` - Your secure MongoDB password
- `ADMIN_PASSWORD` - Your secure admin account password
- `ADMIN_EMAIL` - Your admin email address

### Development Workflow

**For code changes:**
1. Make changes locally
2. Commit and push to main branch
3. GitHub Actions will automatically deploy to test server
4. Test at http://192.168.68.105:5000

**For manual deployment:**
1. SSH to server: `ssh -i ~/.ssh/github-actions-deploy deploy@192.168.68.105`
2. Navigate: `cd /opt/nflplayoffpool`
3. Update: `git pull origin main`
4. Deploy: `docker-compose up -d`

### Server Information

**Test Server Details:**
- **IP**: 192.168.68.105
- **User**: deploy
- **SSH Key**: ~/.ssh/github-actions-deploy
- **App Directory**: /opt/nflplayoffpool
- **Port**: 5000

**Docker Services:**
- **webapp**: NFL Playoff Pool application (port 5000)
- **mongodb**: MongoDB database (port 27017)

### Database Access

**Connect to MongoDB from local machine:**

**Connection String Template:**
```
mongodb://admin:YOUR_MONGODB_PASSWORD@192.168.68.105:27017/playoff_pool_test?authSource=admin
```

**MongoDB Compass:**
- **Host**: 192.168.68.105
- **Port**: 27017
- **Username**: admin
- **Password**: [Use your MongoDB password from .env]
- **Authentication Database**: admin
- **Database**: playoff_pool_test

**MongoDB Shell (mongosh):**
```bash
mongosh "mongodb://admin:YOUR_MONGODB_PASSWORD@192.168.68.105:27017/playoff_pool_test?authSource=admin"
```

**Visual Studio Code (MongoDB Extension):**
```json
{
  "name": "NFL Playoff Pool Test",
  "connectionString": "mongodb://admin:YOUR_MONGODB_PASSWORD@192.168.68.105:27017/playoff_pool_test?authSource=admin"
}
```

**⚠️ SECURITY NOTE**: Replace `YOUR_MONGODB_PASSWORD` with the actual password from your .env file

**Common MongoDB Commands:**
```javascript
// Show databases
show dbs

// Use the playoff pool database
use playoff_pool_test

// Show collections
show collections

// Find all users
db.Users.find()

// Find all brackets
db.Brackets.find()

// Count documents
db.Users.countDocuments()
```

### Backup and Recovery

**Manual backup:**
```bash
cd /opt/nflplayoffpool
./backup.sh
```

**Backup location:** `/opt/nflplayoffpool/backups/`

**Restore from backup:**
```bash
# Stop services
docker-compose down

# Restore MongoDB data (replace TIMESTAMP with actual backup)
tar -xzf /opt/nflplayoffpool/backups/nflpool_backup_TIMESTAMP_mongodb.tar.gz -C ./data/

# Restore environment file
cp /opt/nflplayoffpool/backups/nflpool_backup_TIMESTAMP.env .env

# Start services
docker-compose up -d
```

---

## Security Best Practices

### Credential Management
- **Never commit credentials** to the repository
- Use `.env` files for local configuration (already in .gitignore)
- Store production secrets in GitHub Secrets or secure vault
- Use strong passwords (12+ characters, mixed case, numbers, symbols)

### Environment Files
- `.env` files are automatically ignored by git
- Always copy from `.env.template` and update with real values
- Never share .env files in chat, email, or documentation

### Production Considerations
- Use different credentials for each environment
- Rotate passwords regularly
- Restrict database access to necessary IPs only
- Use SSH tunneling for database connections in production

---

**Last Updated**: 2025-12-28
**Environment**: Test Server (192.168.68.105:5000)