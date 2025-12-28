#!/bin/bash

# NFL Playoff Pool - Azure DevOps Deployment Script
# This script is designed to be run from Azure DevOps pipelines
# Usage: ./deploy-from-azdo.sh [environment] [build-id]

set -e

ENVIRONMENT=${1:-test}
BUILD_ID=${2:-latest}
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
APP_DIR="/opt/nfl-$ENVIRONMENT"

echo "🚀 Deploying NFL Playoff Pool from Azure DevOps"
echo "📋 Environment: $ENVIRONMENT"
echo "🏗️  Build ID: $BUILD_ID"
echo "📁 Application Directory: $APP_DIR"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Ensure we're in the application directory
if [ ! -d "$APP_DIR" ]; then
    log_error "Application directory not found: $APP_DIR"
    log_error "Please run the VM setup script first"
    exit 1
fi

cd "$APP_DIR"

# Check for .env file
if [ ! -f ".env" ]; then
    log_error "Environment file not found: $APP_DIR/.env"
    log_error "Please ensure the VM setup script has been run"
    exit 1
fi

log_info "Using environment file: $APP_DIR/.env"

# Load environment variables
set -a
source .env
set +a

# Validate required environment variables
if [ -z "$MONGODB_ROOT_PASSWORD" ] || [ "$MONGODB_ROOT_PASSWORD" = "CHANGE_ME_SECURE_PASSWORD" ]; then
    log_error "MONGODB_ROOT_PASSWORD is missing or using default value"
    log_error "Please update your .env file with a secure password"
    exit 1
fi

if [ -z "$ADMIN_PASSWORD" ] || [ "$ADMIN_PASSWORD" = "CHANGE_ME_SECURE_PASSWORD" ]; then
    log_error "ADMIN_PASSWORD is missing or using default value"
    log_error "Please update your .env file with a secure password"
    exit 1
fi

# Create data directory if it doesn't exist
log_info "Ensuring data directory exists: ${MONGODB_DATA_PATH:-./data/mongodb}"
mkdir -p "${MONGODB_DATA_PATH:-./data/mongodb}"

# Set proper permissions for MongoDB data directory
if [ "$ASPNETCORE_ENVIRONMENT" = "Production" ] || [ "$ASPNETCORE_ENVIRONMENT" = "Staging" ]; then
    sudo chown -R 999:999 "${MONGODB_DATA_PATH:-./data/mongodb}"  # MongoDB container user
fi

# Stop existing containers gracefully
log_info "Stopping existing containers..."
docker-compose down --timeout 30 || true

# Remove old containers and images to free space
log_info "Cleaning up old containers and images..."
docker container prune -f || true
docker image prune -f || true

# Build new image with build ID tag
log_info "Building application image..."
if [ "$BUILD_ID" != "latest" ]; then
    docker build --no-cache -t "nflplayoffpool:$BUILD_ID" -f Dockerfile .
    
    # Update docker-compose.yml to use the new image
    if [ -f "docker-compose.yml" ]; then
        sed -i.bak "s|build:|# build:|g" docker-compose.yml
        sed -i "s|# image: nflplayoffpool:.*|image: nflplayoffpool:$BUILD_ID|g" docker-compose.yml
        
        # If no image line exists, add it
        if ! grep -q "image: nflplayoffpool:" docker-compose.yml; then
            sed -i "/# build:/a\\    image: nflplayoffpool:$BUILD_ID" docker-compose.yml
        fi
    fi
else
    # Use build context for latest
    log_info "Building from source (latest)..."
fi

# Pull latest base images
log_info "Pulling latest base images..."
docker-compose pull mongodb || true

# Start services
log_info "Starting services..."
docker-compose up -d

# Wait for services to be healthy
log_info "Waiting for services to be healthy..."
TIMEOUT=120
ELAPSED=0
INTERVAL=5

while [ $ELAPSED -lt $TIMEOUT ]; do
    if docker-compose ps | grep -q "unhealthy"; then
        log_warning "Some services are still starting... (${ELAPSED}s/${TIMEOUT}s)"
    elif docker-compose ps webapp | grep -q "Up"; then
        log_success "Services are running"
        break
    else
        log_warning "Waiting for services to start... (${ELAPSED}s/${TIMEOUT}s)"
    fi
    
    sleep $INTERVAL
    ELAPSED=$((ELAPSED + INTERVAL))
done

# Check if we timed out
if [ $ELAPSED -ge $TIMEOUT ]; then
    log_error "Services failed to start within $TIMEOUT seconds"
    log_error "Container status:"
    docker-compose ps
    log_error "Container logs:"
    docker-compose logs
    exit 1
fi

# Verify application is responding
WEBAPP_PORT=${WEBAPP_PORT:-5000}
log_info "Verifying application response on port $WEBAPP_PORT..."

# Wait a bit more for the application to fully initialize
sleep 15

HEALTH_TIMEOUT=60
HEALTH_ELAPSED=0

while [ $HEALTH_ELAPSED -lt $HEALTH_TIMEOUT ]; do
    if curl -f "http://localhost:$WEBAPP_PORT/health" > /dev/null 2>&1; then
        log_success "Application is responding on port $WEBAPP_PORT"
        break
    else
        log_warning "Waiting for application to respond... (${HEALTH_ELAPSED}s/${HEALTH_TIMEOUT}s)"
        sleep 5
        HEALTH_ELAPSED=$((HEALTH_ELAPSED + 5))
    fi
done

if [ $HEALTH_ELAPSED -ge $HEALTH_TIMEOUT ]; then
    log_error "Application is not responding on port $WEBAPP_PORT"
    log_error "Container logs:"
    docker-compose logs webapp
    exit 1
fi

# Show deployment summary
log_success "Deployment completed successfully!"
echo ""
log_info "Deployment Summary:"
echo "  Environment: $ENVIRONMENT"
echo "  Build ID: $BUILD_ID"
echo "  Application URL: http://$(hostname -I | awk '{print $1}'):$WEBAPP_PORT"
echo "  Health Endpoint: http://$(hostname -I | awk '{print $1}'):$WEBAPP_PORT/health"
echo ""

# Show running containers
log_info "Running containers:"
docker-compose ps

# Show resource usage
log_info "Resource usage:"
docker stats --no-stream

# Create deployment log entry
DEPLOY_LOG="$APP_DIR/logs/deployments.log"
mkdir -p "$(dirname "$DEPLOY_LOG")"
echo "$(date '+%Y-%m-%d %H:%M:%S') - Deployment completed - Environment: $ENVIRONMENT, Build: $BUILD_ID" >> "$DEPLOY_LOG"

log_success "Deployment logged to: $DEPLOY_LOG"