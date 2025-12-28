# Logical Components - Infrastructure Modernization

## Overview
This document defines the logical components and their relationships for the NFL Playoff Pool infrastructure modernization, based on the approved NFR design patterns.

## Component Architecture

### Application Tier Components

#### Web Application Container (webapp)
- **Purpose**: Hosts the ASP.NET Core MVC application
- **Technology**: .NET 8.0 in Docker container
- **Responsibilities**:
  - HTTP request handling and routing
  - Business logic execution
  - User interface rendering
  - Authentication and authorization
  - Database connectivity management
- **Dependencies**: MongoDB Container, Docker Secrets
- **Health Check**: HTTP endpoint at `/health`
- **Security**: Runs as non-root user (nflpool:1001)

#### Health Check Component
- **Purpose**: Application-level health monitoring
- **Implementation**: ASP.NET Core health check middleware
- **Endpoints**:
  - `/health` - Overall application health
  - `/health/ready` - Readiness probe (database connectivity)
  - `/health/live` - Liveness probe (application responsiveness)
- **Integration**: Docker health check system

### Data Tier Components

#### MongoDB Container (mongodb)
- **Purpose**: Primary data storage for application data
- **Technology**: MongoDB 7.0 in Docker container
- **Responsibilities**:
  - User data persistence
  - Bracket and pick storage
  - Season and game data management
  - Authentication data storage
- **Security**: 
  - Authentication enabled with dedicated user
  - Runs with restricted permissions
  - Data encryption at rest (future enhancement)
- **Persistence**: Hybrid volume strategy (data + config)

#### MongoDB Initialization Component
- **Purpose**: Database setup and user creation
- **Implementation**: JavaScript initialization script
- **Responsibilities**:
  - Create application database
  - Create application user with appropriate permissions
  - Set up initial indexes for performance
- **Execution**: Runs once during first container startup

### Infrastructure Components

#### Docker Compose Orchestrator
- **Purpose**: Container lifecycle management and service coordination
- **Responsibilities**:
  - Service dependency management
  - Network configuration and service discovery
  - Volume management and persistence
  - Health check coordination
  - Restart policy enforcement
- **Configuration**: Single docker-compose.yml file

#### Docker Network (default)
- **Purpose**: Inter-container communication
- **Type**: Bridge network with automatic service discovery
- **Security**: Container isolation with controlled communication
- **DNS**: Automatic service name resolution

### Security Components

#### Docker Secrets Manager
- **Purpose**: Secure credential storage and distribution
- **Implementation**: Docker Compose secrets
- **Scope**: Single secret file with all credentials
- **Access**: File-based secret mounting in containers
- **Rotation**: Manual process (future automation)

#### Application Security Component
- **Purpose**: Authentication and authorization
- **Implementation**: ASP.NET Core Identity with JWT
- **Integration**: MongoDB for user data storage
- **Features**:
  - User registration and login
  - JWT token generation and validation
  - Role-based authorization

### Backup and Recovery Components

#### Backup Automation Component
- **Purpose**: Automated database backup creation
- **Implementation**: Host-based cron job
- **Schedule**: Daily at 2 AM
- **Process**:
  1. Execute mongodump via Docker exec
  2. Copy backup files to host storage
  3. Verify backup integrity
  4. Clean up old backups (30-day retention)
- **Storage**: Local filesystem with future cloud sync option

#### Backup Verification Component
- **Purpose**: Ensure backup integrity and recoverability
- **Process**:
  1. Restore backup to temporary database
  2. Verify record counts match original
  3. Validate data structure integrity
  4. Clean up verification database
- **Integration**: Part of backup automation workflow

### Monitoring and Observability Components

#### Logging Component
- **Purpose**: Application and system log management
- **Implementation**: Docker native logging with rotation
- **Configuration**:
  - JSON file driver
  - 10MB max file size
  - 3 file rotation
- **Integration**: Docker log aggregation

#### Metrics Component (Future)
- **Purpose**: Performance and usage metrics collection
- **Implementation**: Planned for future enhancement
- **Scope**: Application metrics, database performance, container resources

### Configuration Management Components

#### Environment Configuration Component
- **Purpose**: Environment-specific configuration management
- **Implementation**: Environment variables set by deployment pipeline
- **Validation**: Startup configuration validation
- **Scope**: Database connections, application settings, feature flags

#### Configuration Validation Component
- **Purpose**: Ensure required configuration is present and valid
- **Implementation**: Startup validation in Program.cs
- **Behavior**: Fail fast on missing or invalid configuration
- **Coverage**: Database connections, authentication settings, required paths

## Component Interactions

### Startup Sequence
1. **Docker Compose** reads configuration and secrets
2. **MongoDB Container** starts with initialization script
3. **MongoDB Initialization** creates database and users
4. **Web Application Container** starts and validates configuration
5. **Health Check Component** verifies database connectivity
6. **Application** becomes ready to serve requests

### Request Flow
1. **Client** sends HTTP request to Web Application
2. **Web Application** authenticates request using Security Component
3. **Web Application** queries MongoDB Container for data
4. **MongoDB** returns requested data
5. **Web Application** renders response and returns to client

### Backup Flow
1. **Cron Job** triggers backup script on host
2. **Backup Automation** executes mongodump in MongoDB Container
3. **Backup files** copied to host filesystem
4. **Backup Verification** restores to temporary database
5. **Verification** confirms data integrity
6. **Cleanup** removes old backups and temporary data

### Health Check Flow
1. **Docker** executes health check command
2. **Health Check Component** tests database connectivity
3. **MongoDB** responds to connection test
4. **Health status** reported to Docker
5. **Docker** manages container lifecycle based on health

## Deployment Architecture

### Local Development
- **Components**: All components run on single Docker host
- **Network**: Default bridge network
- **Storage**: Local bind mounts and named volumes
- **Secrets**: Local file-based secrets

### Production (Home Server)
- **Components**: Same component structure as development
- **Network**: Docker bridge with CloudFlare tunnel integration
- **Storage**: Persistent host directories with backup automation
- **Secrets**: Secure file storage with restricted permissions
- **Monitoring**: Enhanced logging and health checks

## Security Boundaries

### Container Isolation
- **Web Application**: Isolated container with non-root user
- **MongoDB**: Isolated container with authentication required
- **Network**: Bridge network with controlled inter-container communication

### Data Protection
- **Secrets**: Docker secrets with file-based mounting
- **Database**: Authentication required, dedicated application user
- **Backups**: Encrypted storage (future enhancement)
- **Logs**: Rotation and retention policies

### Access Control
- **Application**: JWT-based authentication with role authorization
- **Database**: User-based access control with minimal privileges
- **Host**: Restricted file permissions for configuration and data

## Scalability Considerations

### Current Architecture
- **Single instance**: Optimized for personal use (few users)
- **Vertical scaling**: Increase container resources as needed
- **Connection pooling**: Optimized MongoDB connections

### Future Enhancements
- **Horizontal scaling**: Multiple web application containers
- **Load balancing**: Nginx or similar for request distribution
- **Database clustering**: MongoDB replica set for high availability
- **Caching**: Redis for session and data caching

## Maintenance and Operations

### Regular Maintenance
- **Backup verification**: Automated daily verification
- **Log rotation**: Automatic with size and time limits
- **Security updates**: Container image updates via CI/CD
- **Configuration updates**: Environment variable management

### Monitoring Points
- **Container health**: Docker health checks
- **Application health**: HTTP health endpoints
- **Database health**: Connection and query performance
- **Backup health**: Verification success/failure tracking

### Troubleshooting Components
- **Log aggregation**: Centralized Docker logs
- **Health endpoints**: Detailed status information
- **Database tools**: MongoDB shell access for debugging
- **Container inspection**: Docker exec for runtime debugging