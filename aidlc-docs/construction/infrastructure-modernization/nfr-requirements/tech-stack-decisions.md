# Technology Stack Decisions - Infrastructure Modernization

## Overview
This document captures the technology stack decisions made for the NFL Playoff Pool infrastructure modernization, optimized for simplicity and minimal operational overhead.

## Core Infrastructure Stack

### Container Platform
- **Technology**: Docker + Docker Compose
- **Version**: Latest stable versions
- **Rationale**: 
  - Simple container orchestration for single-server deployment
  - Well-established technology with extensive documentation
  - Suitable for small-scale personal projects
- **Alternatives Considered**: Kubernetes (rejected - too complex for single server)

### Database
- **Technology**: MongoDB 6.0 LTS
- **Image**: Official mongo:6.0 Docker image
- **Rationale**:
  - LTS version provides stability and long-term support
  - Official image is well-maintained and secure
  - Existing application already uses MongoDB
- **Configuration**: Standard configuration with authentication enabled in production

### Web Server
- **Technology**: Kestrel (ASP.NET Core built-in)
- **Configuration**: HTTP only (SSL terminated by CloudFlare)
- **Rationale**:
  - Built into ASP.NET Core framework
  - Sufficient performance for small user base
  - Simplified configuration with CloudFlare handling SSL

## Deployment and CI/CD Stack

### Container Registry
- **Technology**: GitHub Container Registry (ghcr.io)
- **Rationale**:
  - Free for public repositories
  - Integrated with GitHub source control
  - Reliable and well-supported
- **Configuration**: Public repository for cost optimization

### CI/CD Platform
- **Technology**: Azure DevOps Pipelines
- **Rationale**:
  - Existing setup and familiarity
  - Good integration with GitHub
  - Supports local agent deployment
- **Configuration**: Local agent pool on home server

### Deployment Target
- **Technology**: Ubuntu Server LTS (latest)
- **Platform**: Proxmox VM
- **Rationale**:
  - Stable, well-supported Linux distribution
  - Excellent Docker support
  - Familiar environment for containerized applications
- **Configuration**: Minimal Ubuntu Server installation with Docker

## Security and Access Stack

### External Access
- **Technology**: CloudFlare Tunnel (cloudflared)
- **Rationale**:
  - Secure access without exposing home network
  - No need for port forwarding or VPN
  - Integrated with existing CloudFlare setup
- **Configuration**: Separate tunnel from existing HomeAssistant setup

### Secret Management
- **Technology**: Docker Secrets
- **Rationale**:
  - Built into Docker Compose
  - Sufficient security for personal project
  - Simple to implement and maintain
- **Configuration**: Database credentials stored as Docker secrets

### Authentication
- **Technology**: MongoDB SCRAM-SHA-256 (default)
- **Rationale**:
  - Standard MongoDB authentication mechanism
  - Adequate security for containerized deployment
  - No additional complexity or dependencies
- **Configuration**: Enabled in production, disabled in local development

## Backup and Monitoring Stack

### Backup Solution
- **Technology**: mongodump/mongorestore
- **Rationale**:
  - Native MongoDB tools
  - Reliable and well-tested
  - No additional dependencies
- **Configuration**: Automated daily backups with 30-day retention

### Monitoring
- **Technology**: Docker health checks + basic logging
- **Rationale**:
  - Built into Docker platform
  - Sufficient for personal project scale
  - No additional infrastructure required
- **Configuration**: Health checks for automatic restart, standard Docker logging

## Development and Configuration Stack

### Environment Management
- **Technology**: Docker Compose environment variables
- **Rationale**:
  - Simple configuration management
  - No need for complex profile systems
  - Easy to understand and maintain
- **Configuration**: Environment-specific variables in .env files

### Documentation
- **Technology**: Markdown (README files)
- **Rationale**:
  - Simple, version-controlled documentation
  - Suitable for single-operator project
  - Easy to maintain and update
- **Configuration**: README files with setup and operational procedures

## Network Architecture

### Container Networking
- **Technology**: Docker Compose default networking
- **Rationale**:
  - Adequate isolation for personal project
  - Simple configuration and troubleshooting
  - No complex networking requirements
- **Configuration**: Default bridge network with service discovery

### External Connectivity
- **Technology**: CloudFlare Tunnel + Local port exposure
- **Rationale**:
  - External access via secure tunnel
  - Local network access via direct ports
  - Flexible access patterns for different use cases
- **Configuration**: 
  - External: CloudFlare tunnel to fjapool.com/test.fjapool.com
  - Internal: Direct port access (5000/5001)

## Decision Matrix

| Component | Chosen Technology | Alternative | Decision Rationale |
|-----------|------------------|-------------|-------------------|
| Container Platform | Docker Compose | Kubernetes | Simplicity for single server |
| Database | MongoDB 6.0 | PostgreSQL | Existing application compatibility |
| Container Registry | GitHub (ghcr.io) | Docker Hub | Free tier and GitHub integration |
| CI/CD | Azure DevOps | GitHub Actions | Existing setup and local agent support |
| Server OS | Ubuntu Server | CentOS/Debian | Stability and Docker support |
| External Access | CloudFlare Tunnel | VPN/Port Forward | Security and ease of setup |
| Backup | mongodump | Third-party tools | Simplicity and reliability |
| Monitoring | Docker health checks | Prometheus/Grafana | Minimal complexity for personal use |

## Technology Constraints

### Must Use
- Docker (containerization requirement)
- MongoDB (existing application dependency)
- Azure DevOps (existing CI/CD setup)
- CloudFlare (existing domain and account)
- Proxmox (existing virtualization platform)

### Must Avoid
- Complex orchestration platforms (Kubernetes, Docker Swarm)
- Enterprise monitoring solutions (too complex for personal use)
- Cloud-hosted services (cost and complexity)
- Custom authentication systems (unnecessary complexity)

## Future Technology Considerations

### Potential Upgrades
- **Monitoring**: Could add Prometheus/Grafana if monitoring needs grow
- **Backup**: Could add cloud backup storage for additional redundancy
- **Security**: Could add CloudFlare Access for additional authentication layer
- **Scaling**: Could migrate to Kubernetes if horizontal scaling becomes necessary

### Technology Debt
- **SSL Termination**: Currently relies on CloudFlare, could add local SSL if needed
- **Database Clustering**: Single MongoDB instance, could add replica set for high availability
- **Load Balancing**: Single application instance, could add load balancer for scaling

## Implementation Priority

### Phase 1 (Immediate)
1. Docker Compose configuration
2. MongoDB containerization
3. Local development setup

### Phase 2 (CI/CD)
1. GitHub Container Registry setup
2. Azure DevOps pipeline updates
3. Automated build and push

### Phase 3 (Deployment)
1. Ubuntu Server VM setup
2. Azure DevOps agent installation
3. Deployment automation

### Phase 4 (External Access)
1. CloudFlare tunnel configuration
2. Domain setup (fjapool.com, test.fjapool.com)
3. End-to-end testing

### Phase 5 (Operations)
1. Backup automation
2. Health check configuration
3. Documentation completion