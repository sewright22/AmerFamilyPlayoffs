@echo off
REM NFL Playoff Pool Deployment Script for Windows
REM Usage: deploy.bat

echo 🚀 Deploying NFL Playoff Pool...

REM Check for .env file
if not exist ".env" (
    echo ❌ Environment file not found: .env
    echo 💡 Copy .env.template to .env and configure your environment
    exit /b 1
)

echo 📋 Using environment file: .env

REM Create data directory if it doesn't exist
if not exist "data\mongodb" (
    echo 📁 Creating data directory: data\mongodb
    mkdir data\mongodb
)

REM Pull latest images
echo 📦 Pulling latest container images...
docker-compose pull

REM Stop existing containers
echo 🛑 Stopping existing containers...
docker-compose down

REM Start services
echo 🔄 Starting services...
docker-compose up -d

REM Wait for services to be healthy
echo ⏳ Waiting for services to be healthy...
timeout /t 30 /nobreak > nul

REM Check if containers are running
echo 🏥 Checking service health...
docker-compose ps

REM Verify application is responding
echo 🔍 Verifying application response...
curl -f "http://localhost:5000/health" > nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Application is responding on port 5000
) else (
    echo ❌ Application is not responding on port 5000
    echo 📋 Container logs:
    docker-compose logs webapp
    exit /b 1
)

echo 🎉 Deployment completed successfully!
echo 🌐 Application available at: http://localhost:5000

REM Show running containers
echo 📊 Running containers:
docker-compose ps