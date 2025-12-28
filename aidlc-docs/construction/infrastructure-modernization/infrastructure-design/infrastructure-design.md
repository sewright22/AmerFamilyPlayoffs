# Infrastructure Design - Infrastructure Modernization

## Overview
This document defines the complete infrastructure architecture for the NFL Playoff Pool modernization project, focusing on containerized MongoDB, home server deployment, and CloudFlare tunnel integration.

## Infrastructure Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Internet                                  │
└─────────────────────┬───────────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────────┐
│                 CloudFlare                                       │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │  SSL Termination & DDoS Protection                          │ │
│  │  Domain: fjapool.com                                        │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────┬───────────────────────────────────────────┘
                      │ CloudFlare Tunnel
┌─────────────────────▼───────────────────────────────────────────┐
│                 Home Network                                     │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │              Proxmox Server                                 │ │
│  │  ┌─────────────────────────────────────────────────────────┐ │ │
│  │  │           Ubuntu VM (2 vCPU, 4GB RAM, 50GB)            │ │ │
│  │  │                                                         │ │ │
│  │  │  ┌─────────────────┐  ┌─────────────────────────────────┐ │ │ │
│  │  │  │ Production      │  │ Staging                         │ │ │ │
│  │  │  │ Port: 80        │  │ Port: 8080                      │ │ │ │
│  │  │  │                 │  │                                 │ │ │ │
│  │  │  │ ┌─────────────┐ │  │ ┌─────────────┐                 │ │ │ │
│  │  │  │ │ Web App     │ │  │ │ Web App     │                 │ │ │ │
│  │  │  │ │ Container   │ │  │ │ Container   │                 │ │ │ │
│  │  │  │ └─────────────┘ │  │ └─────────────┘                 │ │ │ │
│  │  │  │ ┌─────────────┐ │  │ ┌─────────────┐                 │ │ │ │
│  │  │  │ │ MongoDB     │ │  │ │ MongoDB     │                 │ │ │ │
│  │  │  │ │ Container   │ │  │ │ Container   │                 │ │ │ │
│  │  │  │ └─────────────┘ │  │ └─────────────┘                 │ │ │ │
│  │  │  └─────────────────┘  └─────────────────────────────────┘ │ │ │
│  │  │                                                         │ │ │
│  │  │  ┌─────────────────────────────────────────────────────┐ │ │ │
│  │  │  │ Azure DevOps Agent (systemd service)               │ │ │ │
│  │  │  │ User: azureagent                                    │ │ │ │
│  │  │  └─────────────────────────────────────────────────────┘ │ │ │
│  │  │                                                         │ │ │
│  │  │  ┌─────────────────────────────────────────────────────┐ │ │ │
│  │  │  │ CloudFlare Tunnel Daemon                            │ │ │ │
│  │  │  └─────────────────────────────────────────────────────┘ │ │ │
│  │  └─────────────────────────────────────────────────────────┘ │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Component Design Specifications

### Docker Compose Architecture

#### Single Configuration Approach
- **File Structure**: Single `docker-compose.yml` for all environments
- **Environment Differentiation**: Environment variables and port mapping
- **Rationale**: Simplified management for personal project with clear environment separation

#### Service Configuration
```yaml
# docker-compose.yml structure
services:
  webapp-prod:
    # Production web application (port 80)
  mongodb-prod:
    # Production MongoDB (port 27017)
  webapp-staging:
    # Staging web application (port 8080)  
  mongodb-staging:
    # Staging MongoDB (port 27018)
```

#### Resource Allocation
- **Web Application Containers**:
  - Memory: 1GB limit
  - CPU: 1 CPU limit
  - Rationale: Conservative allocation for personal use with few concurrent users
- **MongoDB Containers**:
  - Memory: 2GB limit
  - CPU: Shared (no specific limit)
  - Rationale: Database requires more memory for caching and operations

### Ubuntu VM Infrastructure

#### VM Specifications
- **CPU**: 2 vCPU cores
- **Memory**: 4GB RAM
- **Storage**: 50GB primary disk
- **OS**: Ubuntu Server 22.04 LTS
- **Rationale**: Minimal resources sufficient for containerized workload with room for growth

#### Storage Configuration (Proxmox Recommended)
- **Primary Disk**: 50GB on Proxmox local storage
- **File System**: ext4 with standard partitioning
- **Backup**: Proxmox VM backup integration
- **Directory Structure**:
  ```
  /opt/nflplayoffpool/
  ├── production/
  │   ├── data/           # MongoDB data (bind mount)
  │   ├── logs/           # Application logs
  │   └── backups/        # Database backups
  ├── staging/
  │   ├── data/           # MongoDB data (bind mount)
  │   └── logs/           # Application logs
  ├── config/
  │   ├── secrets/        # Docker secrets
  │   └── compose/        # Docker Compose files
  └── scripts/
      ├── backup.sh       # Backup automation
      ├── deploy.sh       # Deployment scripts
      └── rollback.sh     # Rollback procedures
  ```

#### Network Configuration
- **Static IP**: Assigned within home network range
- **Firewall**: UFW with minimal open ports (22, 80, 8080, CloudFlare tunnel)
- **SSH**: Key-based authentication only
- **Docker Networking**: Default bridge networks for container isolation

### Azure DevOps Agent Infrastructure

#### Installation Method
- **Type**: Native systemd service installation
- **Package**: Microsoft-provided .deb package
- **Service Management**: systemctl for start/stop/restart operations
- **Rationale**: Native installation provides better integration with host system and Docker

#### User and Permissions
- **Service User**: `azureagent` (dedicated system user)
- **Group Memberships**: `docker` (for container operations)
- **Home Directory**: `/home/azureagent`
- **Shell**: `/bin/bash`
- **Sudo Permissions**: Limited to Docker and deployment-related commands

#### Agent Configuration
```bash
# Agent capabilities
- Docker container operations
- File system access to /opt/nflplayoffpool/
- Network access for GitHub Container Registry
- Systemctl operations for service management
```

#### Security Configuration
- **Agent Pool**: Dedicated pool for home server agents
- **Agent Tags**: `home-server`, `docker`, `linux`
- **Pipeline Permissions**: Restricted to specific repositories and pipelines
- **Token Management**: Secure token storage with rotation capability

### GitHub Container Registry Integration

#### Authentication Configuration
- **Method**: Personal Access Token (PAT)
- **Scope**: `write:packages`, `read:packages`, `delete:packages`
- **Storage**: Azure DevOps pipeline variables (encrypted)
- **Rotation**: Manual process with documented procedures

#### Container Image Strategy
- **Registry**: `ghcr.io/[username]/nflplayoffpool`
- **Tagging Strategy**: Semantic versioning approach
  - `latest`: Most recent production release
  - `v1.0.0`: Semantic version tags for releases
  - `sha-[commit]`: Git commit SHA for traceability
  - `staging`: Latest staging build

#### Build and Push Process
```yaml
# Azure DevOps pipeline stages
1. Build application
2. Create Docker image
3. Tag with version and SHA
4. Push to GitHub Container Registry
5. Update deployment manifests
```

### CloudFlare Tunnel Infrastructure

#### Tunnel Configuration
- **Tunnel Type**: New dedicated tunnel (no conflicts with existing)
- **Domain**: `fjapool.com` (different from existing tunnel domain)
- **Subdomain Strategy**:
  - `fjapool.com` → Production environment (port 80)
  - `staging.fjapool.com` → Staging environment (port 8080)

#### SSL/TLS Configuration
- **Mode**: Full SSL (CloudFlare terminates SSL)
- **Certificate**: CloudFlare Universal SSL (free)
- **Security Level**: Medium (balanced security and performance)
- **Rationale**: Cost-effective solution with adequate security for personal project

#### Tunnel Service Configuration
```yaml
# cloudflared configuration
tunnel: [tunnel-id]
credentials-file: /etc/cloudflared/credentials.json
ingress:
  - hostname: fjapool.com
    service: http://localhost:80
  - hostname: staging.fjapool.com
    service: http://localhost:8080
  - service: http_status:404
```

#### Installation and Management
- **Installation**: CloudFlare tunnel daemon as systemd service
- **Configuration**: `/etc/cloudflared/config.yml`
- **Credentials**: Secure storage in `/etc/cloudflared/credentials.json`
- **Service Management**: systemctl for tunnel lifecycle

### Backup Infrastructure

#### Backup Strategy
- **Storage Location**: Local filesystem only (`/opt/nflplayoffpool/*/backups/`)
- **Backup Method**: MongoDB native `mongodump` utility
- **Frequency**: Daily automated backups via cron
- **Retention**: 30 days (simple retention policy)

#### Backup Process Design
```bash
# Daily backup workflow
1. Execute mongodump for each environment (production, staging)
2. Compress backup files with timestamp
3. Verify backup integrity by test restore
4. Store verified backup in designated directory
5. Clean up backups older than 30 days
6. Log backup status and any errors
```

#### Backup Verification
- **Method**: Restore to temporary database and verify record counts
- **Validation**: Compare original vs. restored data statistics
- **Cleanup**: Remove temporary verification database after validation
- **Alerting**: Log errors for manual review (no automated alerting)

#### Backup Script Architecture
```bash
# /opt/nflplayoffpool/scripts/backup.sh
- Environment detection (production/staging)
- MongoDB connection validation
- Backup creation with timestamp
- Integrity verification process
- Cleanup of old backups
- Logging and error handling
```

### Deployment Architecture

#### Environment Structure
- **Production Environment**:
  - Port: 80 (external via CloudFlare tunnel)
  - Database: MongoDB on port 27017
  - Data: `/opt/nflplayoffpool/production/data/`
  - Logs: `/opt/nflplayoffpool/production/logs/`

- **Staging Environment**:
  - Port: 8080 (external via CloudFlare tunnel)
  - Database: MongoDB on port 27018
  - Data: `/opt/nflplayoffpool/staging/data/`
  - Logs: `/opt/nflplayoffpool/staging/logs/`

#### Deployment Process
```yaml
# Azure DevOps deployment pipeline
1. Build and push container image to GitHub Container Registry
2. Connect to home server via Azure DevOps agent
3. Pull latest container image
4. Stop current containers (staging first, then production)
5. Start new containers with updated image
6. Verify health checks pass
7. Update service status and notify completion
```

#### Rollback Procedures
- **Method**: Manual rollback with documented procedures
- **Process**:
  1. Identify previous working container image tag
  2. Stop current containers
  3. Start containers with previous image tag
  4. Verify application functionality
  5. Document rollback reason and resolution
- **Documentation**: Step-by-step rollback procedures in operations manual

### Monitoring and Logging Infrastructure

#### Logging Strategy (Simplest Approach)
- **Method**: Docker native logging with JSON file driver
- **Configuration**:
  - Max file size: 10MB
  - Max files: 3 (30MB total per container)
  - Rotation: Automatic when size limit reached
- **Access**: `docker logs [container-name]` command
- **Persistence**: Logs stored in Docker's default location

#### Monitoring Approach
- **Level**: Basic monitoring with Docker health checks
- **Health Checks**:
  - Web Application: HTTP endpoint (`/health`)
  - MongoDB: Connection test via application
  - Container Status: Docker daemon monitoring
- **Manual Monitoring**: Periodic manual checks of service status
- **No Automated Alerting**: Manual review of logs and status

#### Operational Monitoring
```bash
# Manual monitoring commands
docker ps                          # Container status
docker logs webapp-prod           # Application logs
docker logs mongodb-prod          # Database logs
docker stats                      # Resource usage
systemctl status azureagent       # Agent status
systemctl status cloudflared      # Tunnel status
```

## Security Architecture

### Container Security
- **Non-Root Execution**: All containers run as non-root users
- **Network Isolation**: Docker bridge networks with controlled communication
- **Resource Limits**: Memory and CPU limits to prevent resource exhaustion
- **Image Security**: Regular base image updates via CI/CD pipeline

### Host Security
- **Firewall**: UFW with minimal open ports
- **SSH Access**: Key-based authentication only
- **User Isolation**: Dedicated service accounts with minimal permissions
- **File Permissions**: Restricted access to configuration and data directories

### Secrets Management
- **Docker Secrets**: File-based secrets mounted in containers
- **File Permissions**: 600 (owner read/write only) for secret files
- **Storage Location**: `/opt/nflplayoffpool/config/secrets/`
- **Rotation**: Manual process with documented procedures

### Network Security
- **CloudFlare Protection**: DDoS protection and Web Application Firewall
- **SSL/TLS**: Full SSL mode with CloudFlare certificate management
- **Internal Communication**: Container-to-container via Docker networks
- **External Access**: Only via CloudFlare tunnel (no direct port exposure)

## Scalability and Performance

### Current Capacity
- **Concurrent Users**: Optimized for 10-50 concurrent users
- **Database Size**: Suitable for moderate data growth (< 10GB)
- **Request Throughput**: Adequate for personal project usage patterns
- **Resource Headroom**: 50% resource utilization target for growth

### Performance Optimization
- **Database**: MongoDB connection pooling and indexing
- **Application**: ASP.NET Core performance optimizations
- **Caching**: Browser caching and static asset optimization
- **CDN**: CloudFlare CDN for static content delivery

### Future Scaling Options
- **Vertical Scaling**: Increase VM resources (CPU, memory)
- **Container Scaling**: Multiple web application containers with load balancing
- **Database Scaling**: MongoDB replica set for high availability
- **Caching Layer**: Redis for session and data caching

## Disaster Recovery

### Backup and Recovery
- **Recovery Time Objective (RTO)**: 4 hours (manual recovery process)
- **Recovery Point Objective (RPO)**: 24 hours (daily backups)
- **Recovery Process**:
  1. Restore VM from Proxmox backup (if needed)
  2. Restore MongoDB data from latest backup
  3. Redeploy application containers
  4. Verify functionality and data integrity

### High Availability Considerations
- **Single Point of Failure**: Accepted for personal project
- **Backup Verification**: Daily automated verification
- **Documentation**: Comprehensive recovery procedures
- **Testing**: Periodic recovery testing (quarterly)

## Operational Procedures

### Deployment Operations
- **Staging Deployment**: Automated via Azure DevOps pipeline
- **Production Deployment**: Manual approval required
- **Rollback**: Manual process with documented steps
- **Verification**: Health check validation post-deployment

### Maintenance Operations
- **System Updates**: Monthly Ubuntu security updates
- **Container Updates**: Automated via CI/CD pipeline
- **Backup Maintenance**: Automated cleanup and verification
- **Log Management**: Automatic rotation, manual review

### Troubleshooting Procedures
- **Service Issues**: Documented troubleshooting steps
- **Performance Issues**: Resource monitoring and optimization
- **Security Issues**: Incident response procedures
- **Data Issues**: Backup restoration procedures

## Implementation Timeline

### Phase 1: Local Development (Week 1)
- Docker Compose configuration
- Local MongoDB containerization
- Development workflow validation

### Phase 2: VM Setup (Week 1-2)
- Ubuntu VM creation on Proxmox
- Basic system configuration
- Azure DevOps agent installation

### Phase 3: CI/CD Integration (Week 2)
- GitHub Container Registry setup
- Pipeline configuration
- Automated deployment testing

### Phase 4: CloudFlare Tunnel (Week 2-3)
- Tunnel creation and configuration
- SSL/TLS setup
- External access validation

### Phase 5: Production Deployment (Week 3)
- Production environment setup
- Backup automation implementation
- End-to-end testing and validation