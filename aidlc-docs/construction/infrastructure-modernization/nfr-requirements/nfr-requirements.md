# NFR Requirements - Infrastructure Modernization

## Overview
This document defines the non-functional requirements for the NFL Playoff Pool infrastructure modernization project, optimized for a small personal project with minimal complexity and operational overhead.

## Scalability Requirements

### SR1: User Load Capacity
- **Requirement**: Support small user load (< 50 concurrent users)
- **Rationale**: Personal project with limited user base
- **Implementation**: Single application instance sufficient

### SR2: Traffic Pattern Management
- **Requirement**: Handle manageable traffic spikes during NFL playoff season
- **Rationale**: Seasonal usage pattern with predictable increases
- **Implementation**: Current single-instance setup can handle expected load

### SR3: Horizontal Scaling
- **Requirement**: No horizontal scaling required initially
- **Rationale**: Single instance sufficient for current and projected usage
- **Implementation**: Design allows for future scaling if needed

## Performance Requirements

### PR1: Response Time
- **Requirement**: Page loads under 2 seconds acceptable
- **Rationale**: Personal project with relaxed performance expectations
- **Implementation**: Standard Docker and MongoDB performance sufficient

### PR2: Database Performance
- **Requirement**: Current MongoDB performance acceptable
- **Rationale**: Small dataset and user base, no specific performance targets needed
- **Implementation**: Standard MongoDB 6.0 configuration

### PR3: Resource Management
- **Requirement**: No specific Docker resource limits
- **Rationale**: Single-user server environment, containers can use available resources
- **Implementation**: Let Docker manage resources dynamically

## Availability Requirements

### AR1: Maintenance Windows
- **Requirement**: Flexible maintenance scheduling acceptable
- **Rationale**: Personal project with no strict uptime requirements
- **Implementation**: Can schedule maintenance during low-usage periods

### AR2: Uptime Target
- **Requirement**: Best effort uptime, no specific SLA
- **Rationale**: Personal project without business-critical requirements
- **Implementation**: Standard Docker reliability sufficient

### AR3: Health Monitoring
- **Requirement**: Docker health checks with automatic container restart
- **Rationale**: Basic reliability without manual intervention
- **Implementation**: Docker Compose health checks and restart policies

## Security Requirements

### SEC1: Network Isolation
- **Requirement**: Default Docker Compose networking sufficient
- **Rationale**: Simple setup with adequate isolation for personal use
- **Implementation**: Use Docker Compose default network

### SEC2: Database Access
- **Requirement**: MongoDB accessible only from application container
- **Rationale**: Minimize attack surface, no external database access needed
- **Implementation**: No port exposure for MongoDB container

### SEC3: Authentication
- **Requirement**: SCRAM-SHA-256 authentication (MongoDB default) for production
- **Rationale**: Standard MongoDB security without additional complexity
- **Implementation**: Enable authentication in production environment only

### SEC4: Secret Management
- **Requirement**: Set Docker secrets once during initial setup
- **Rationale**: Personal project with minimal secret rotation needs
- **Implementation**: Docker secrets for database credentials, no rotation policy

### SEC5: External Access Security
- **Requirement**: CloudFlare tunnel security sufficient
- **Rationale**: CloudFlare provides adequate security for personal project
- **Implementation**: No additional authentication layers needed

## Technology Stack Decisions

### TS1: MongoDB Image
- **Decision**: Official MongoDB image (mongo:6.0)
- **Rationale**: Standard, well-supported, sufficient for requirements
- **Implementation**: Use mongo:6.0 Docker image

### TS2: Environment Management
- **Decision**: Use environment variables only (no Docker Compose profiles)
- **Rationale**: Simpler configuration management for small project
- **Implementation**: Environment-specific variables in Docker Compose

### TS3: CloudFlare Tunnel
- **Decision**: cloudflared (official CloudFlare tunnel daemon)
- **Rationale**: Standard, reliable, well-documented solution
- **Implementation**: Install cloudflared on Ubuntu server

### TS4: Backup Solution
- **Decision**: mongodump/mongorestore (MongoDB native tools)
- **Rationale**: Simple, reliable, no additional dependencies
- **Implementation**: Automated scripts using MongoDB native tools

## Monitoring and Observability Requirements

### MO1: Application Logging
- **Requirement**: Current logging configuration sufficient
- **Rationale**: Existing logging meets needs for personal project
- **Implementation**: No changes to application logging

### MO2: Log Management
- **Requirement**: View logs directly from containers
- **Rationale**: Simple approach suitable for single-user environment
- **Implementation**: Use docker logs command for troubleshooting

### MO3: Database Monitoring
- **Requirement**: No specific MongoDB monitoring initially
- **Rationale**: Will be addressed in future phase if needed
- **Implementation**: Basic Docker health checks sufficient

## Backup and Disaster Recovery Requirements

### BDR1: Recovery Point Objective (RPO)
- **Requirement**: Daily backups acceptable (< 24 hours data loss)
- **Rationale**: Personal project with acceptable daily backup frequency
- **Implementation**: Automated daily MongoDB dumps

### BDR2: Recovery Time Objective (RTO)
- **Requirement**: Flexible recovery time, no specific target
- **Rationale**: Personal project without time-critical recovery needs
- **Implementation**: Manual recovery process acceptable

### BDR3: Backup Storage
- **Requirement**: Store backups on same server (different volume)
- **Rationale**: Simple solution for personal project
- **Implementation**: Dedicated backup volume on Ubuntu server

### BDR4: Backup Retention
- **Requirement**: 30-day backup retention
- **Rationale**: Reasonable retention period for personal project
- **Implementation**: Automated cleanup of backups older than 30 days

## Operational Requirements

### OP1: Deployment Testing
- **Requirement**: Deploy directly to production with manual approval
- **Rationale**: Simple deployment process for personal project
- **Implementation**: Manual approval gate in Azure DevOps pipeline

### OP2: Environment Strategy
- **Requirement**: Simplified test environment (different from production)
- **Rationale**: Cost-effective approach for personal project
- **Implementation**: Test environment with reduced resources

### OP3: Documentation
- **Requirement**: Minimal documentation (README with key commands)
- **Rationale**: Personal project with single operator
- **Implementation**: README with setup, deployment, and basic troubleshooting

## Quality Attributes Summary

| Attribute | Target | Implementation |
|-----------|--------|----------------|
| Scalability | < 50 users | Single instance |
| Performance | < 2s page load | Standard setup |
| Availability | Best effort | Docker health checks |
| Security | Basic protection | Default + authentication |
| Maintainability | Simple operations | Minimal configuration |
| Reliability | Standard Docker | Health checks + backups |

## Constraints and Assumptions

### Constraints
- Single Ubuntu server deployment
- Personal project budget (minimal cost)
- Single operator (no team coordination needed)
- Existing CloudFlare account and domain

### Assumptions
- User load remains small (< 50 users)
- No business-critical uptime requirements
- Manual intervention acceptable for issues
- Standard Docker and MongoDB reliability sufficient