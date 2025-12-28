# Docker Setup Guide

## Prerequisites

### Security Requirements
⚠️ **IMPORTANT**: Before starting, you MUST configure secure passwords in your `.env` file.

1. Copy the template:
   ```cmd
   copy .env.template .env
   ```

2. Edit `.env` and set secure passwords:
   - `MONGODB_ROOT_PASSWORD` - Must be a strong password (12+ chars, mixed case, numbers, symbols)
   - `ADMIN_PASSWORD` - Must be a strong password (12+ chars, mixed case, numbers, symbols)
   
   **Example secure password format**: `MyS3cur3P@ssw0rd!2024`

3. **NEVER commit your `.env` file** - it's in `.gitignore` for security

❌ **DO NOT** use the default `CHANGE_ME_SECURE_PASSWORD` values - the application will refuse to start!

## Quick Start - Local Development (Windows)

### Option 1: Using the Batch Script (Recommended)

```cmd
deploy.bat
```

### Option 2: Manual Docker Commands

```cmd
# 1. Create data directory
mkdir data\mongodb

# 2. Pull images
docker-compose pull

# 3. Start services
docker-compose up -d

# 4. Check status
docker-compose ps

# 5. View logs
docker-compose logs -f webapp

# 6. Test health endpoint
curl http://localhost:5000/health
```

### Option 3: Run with Live Logs

```cmd
# Start and watch logs in real-time
docker-compose up
```

Press `Ctrl+C` to stop when running in foreground mode.

## Verify Setup

1. **Check containers are running:**
   ```cmd
   docker-compose ps
   ```
   Both `webapp` and `mongodb` should show as "Up" and "healthy"

2. **Test health endpoint:**
   ```cmd
   curl http://localhost:5000/health
   ```
   Should return: `Healthy`

3. **Access application:**
   Open browser to: http://localhost:5000

## Troubleshooting

### Container won't start
```cmd
# View logs
docker-compose logs webapp
docker-compose logs mongodb

# Restart services
docker-compose restart
```

### MongoDB connection issues
```cmd
# Check MongoDB is healthy
docker-compose ps mongodb

# View MongoDB logs
docker-compose logs mongodb

# Verify environment variables
docker-compose config
```

### Port already in use
```cmd
# Check what's using port 5000
netstat -ano | findstr :5000

# Stop the process or change WEBAPP_PORT in .env
```

### Reset everything
```cmd
# Stop and remove containers, networks
docker-compose down

# Remove volumes (WARNING: deletes data)
docker-compose down -v

# Start fresh
docker-compose up -d
```

## Stopping Services

```cmd
# Stop containers (keeps data)
docker-compose stop

# Stop and remove containers (keeps data)
docker-compose down

# Stop and remove everything including data
docker-compose down -v
```

## Development Workflow

### Making code changes
1. Make your code changes
2. Rebuild the image:
   ```cmd
   docker-compose build webapp
   ```
3. Restart the container:
   ```cmd
   docker-compose up -d webapp
   ```

### Quick restart without rebuild
```cmd
docker-compose restart webapp
```

### View real-time logs
```cmd
docker-compose logs -f webapp
```

## Environment Variables

The `.env` file controls the deployment. Key variables:

- `ASPNETCORE_ENVIRONMENT` - Development, Staging, or Production
- `WEBAPP_PORT` - Port for web application (default: 5000)
- `MONGODB_PORT` - Port for MongoDB (default: 27017)
- `MONGODB_DATABASE` - Database name (default: playoff_pool)
- `MONGODB_ROOT_PASSWORD` - MongoDB admin password (CHANGE THIS!)
- `MONGODB_DATA_PATH` - Where MongoDB stores data (default: ./data/mongodb)

## Next Steps

Once local testing is successful:
1. Update CI/CD pipelines for automated builds
2. Configure Ubuntu VM on home server
3. Set up CloudFlare tunnel for external access
4. Deploy to production environment