#!/bin/bash

# NFL Playoff Pool Deployment Script
# Usage: ./deploy.sh

set -e

echo "🚀 Deploying NFL Playoff Pool..."

# Check for .env file
if [ ! -f ".env" ]; then
    echo "❌ Environment file not found: .env"
    echo "💡 Copy .env.template to .env and configure your environment"
    exit 1
fi

echo "📋 Using environment file: .env"

# Load environment variables
set -a
source .env
set +a

# Validate required environment variables
if [ -z "$MONGODB_ROOT_PASSWORD" ] || [ "$MONGODB_ROOT_PASSWORD" = "CHANGE_ME_SECURE_PASSWORD" ]; then
    echo "❌ MONGODB_ROOT_PASSWORD is missing or using default value"
    echo "💡 Please update your .env file with a secure password"
    exit 1
fi

# Create data directory if it doesn't exist
echo "📁 Creating data directory: ${MONGODB_DATA_PATH:-./data/mongodb}"
mkdir -p "${MONGODB_DATA_PATH:-./data/mongodb}"

# For production, set proper permissions
if [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
    sudo chown -R 999:999 "${MONGODB_DATA_PATH:-./data/mongodb}"  # MongoDB container user
fi

# Pull latest images
echo "📦 Pulling latest container images..."
docker-compose pull

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Start services
echo "🔄 Starting services..."
docker-compose up -d

# Wait for services to be healthy
echo "⏳ Waiting for services to be healthy..."
sleep 30

# Check health
echo "🏥 Checking service health..."
if docker-compose ps | grep -q "unhealthy"; then
    echo "❌ Some services are unhealthy"
    docker-compose ps
    exit 1
fi

# Verify application is responding
WEBAPP_PORT=${WEBAPP_PORT:-5000}
echo "🔍 Verifying application response..."
if curl -f "http://localhost:$WEBAPP_PORT/health" > /dev/null 2>&1; then
    echo "✅ Application is responding on port $WEBAPP_PORT"
else
    echo "❌ Application is not responding on port $WEBAPP_PORT"
    echo "📋 Container logs:"
    docker-compose logs webapp
    exit 1
fi

echo "🎉 Deployment completed successfully!"
echo "🌐 Application available at: http://localhost:$WEBAPP_PORT"

# Show running containers
echo "📊 Running containers:"
docker-compose ps