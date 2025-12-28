# NFR Design Patterns - Infrastructure Modernization

## Overview
This document defines the specific design patterns and implementation approaches for the non-functional requirements of the NFL Playoff Pool infrastructure modernization project.

## Container Architecture Patterns

### Docker Compose Network Pattern
- **Pattern**: Default Bridge Network with Service Discovery
- **Implementation**: Use Docker Compose default networking for simplicity
- **Rationale**: Simplest configuration while providing adequate container isolation
- **Configuration**:
  ```yaml
  # No explicit network configuration needed
  # Docker Compose creates default network automatically
  services:
    webapp:
      # Service discovery via service name
    mongodb:
      # Accessible via 'mongodb' hostname from webapp
  ```

### Health Check Pattern
- **Pattern**: HTTP Health Check Endpoint
- **Implementation**: Application exposes `/health` endpoint for container health monitoring
- **Rationale**: Provides application-level health verification beyond simple port checks
- **Configuration**:
  ```yaml
  webapp:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
  ```

### Container Restart Pattern
- **Pattern**: Automatic Restart on Failure
- **Implementation**: Docker Compose restart policies with health check integration
- **Configuration**:
  ```yaml
  services:
    webapp:
      restart: unless-stopped
      depends_on:
        mongodb:
          condition: service_healthy
    mongodb:
      restart: unless-stopped
  ```

## Data Persistence Patterns

### Hybrid Volume Pattern
- **Pattern**: Mixed Persistence Strategy
- **Implementation**: 
  - MongoDB data: Bind mount to host directory for direct access
  - MongoDB config: Named volume for Docker management
- **Rationale**: Combines direct data access with managed configuration
- **Configuration**:
  ```yaml
  mongodb:
    volumes:
      - /opt/nflplayoffpool/data:/data/db  # Bind mount for data
      - mongodb_config:/data/configdb      # Named volume for config
  
  volumes:
    mongodb_config:
      driver: local
  ```

### Data Directory Structure
```
/opt/nflplayoffpool/
├── data/           # MongoDB data (bind mount)
├── backups/        # Backup storage
├── logs/           # Application logs
└── config/         # Configuration files
```

## Security Patterns

### Unified Secret Management Pattern
- **Pattern**: Single Secret File with All Credentials
- **Implementation**: One Docker secret containing all sensitive configuration
- **Rationale**: Simplified secret management for personal project
- **Configuration**:
  ```yaml
  secrets:
    app_secrets:
      file: ./secrets/app_secrets.txt
  
  services:
    webapp:
      secrets:
        - app_secrets
    mongodb:
      secrets:
        - app_secrets
  ```

### Secret File Format
```bash
# app_secrets.txt
MONGODB_USERNAME=nflpool_user
MONGODB_PASSWORD=secure_random_password_here
MONGODB_DATABASE=playoff_pool
JWT_SECRET=jwt_signing_key_here
```

### Non-Root Container Pattern
- **Pattern**: Dedicated Application User
- **Implementation**: Create non-root user in application container
- **Rationale**: Reduces security risk by avoiding root privileges
- **Dockerfile Configuration**:
  ```dockerfile
  # Create application user
  RUN addgroup --system --gid 1001 nflpool
  RUN adduser --system --uid 1001 --ingroup nflpool nflpool
  
  # Switch to application user
  USER nflpool
  ```

### MongoDB Authentication Pattern
- **Pattern**: Environment-Based Initialization
- **Implementation**: MongoDB init script using environment variables from secrets
- **Configuration**:
  ```yaml
  mongodb:
    environment:
      MONGO_INITDB_ROOT_USERNAME_FILE: /run/secrets/app_secrets
      MONGO_INITDB_ROOT_PASSWORD_FILE: /run/secrets/app_secrets
      MONGO_INITDB_DATABASE: playoff_pool
    volumes:
      - ./init-mongo.js:/docker-entrypoint-initdb.d/init-mongo.js:ro
  ```

## Backup and Recovery Patterns

### Host-Based Cron Backup Pattern
- **Pattern**: Scheduled Host System Backup
- **Implementation**: Cron job on Ubuntu server executing backup scripts
- **Rationale**: Simple, reliable, and independent of container lifecycle
- **Cron Configuration**:
  ```bash
  # Daily backup at 2 AM
  0 2 * * * /opt/nflplayoffpool/scripts/backup.sh
  ```

### Backup Verification Pattern
- **Pattern**: Restore-and-Verify Backup Integrity
- **Implementation**: Each backup includes verification step
- **Process**:
  1. Create MongoDB dump using mongodump
  2. Restore dump to temporary database
  3. Verify data integrity and record count
  4. Clean up temporary database
  5. Store verified backup with timestamp

### Backup Script Pattern
```bash
#!/bin/bash
# backup.sh

BACKUP_DIR="/opt/nflplayoffpool/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="nflpool_backup_${TIMESTAMP}"

# Create backup
docker exec mongodb mongodump --db playoff_pool --out /tmp/${BACKUP_FILE}

# Copy backup from container
docker cp mongodb:/tmp/${BACKUP_FILE} ${BACKUP_DIR}/

# Verify backup (restore to temp DB)
docker exec mongodb mongorestore --db playoff_pool_verify /tmp/${BACKUP_FILE}/playoff_pool

# Verify record count matches
ORIGINAL_COUNT=$(docker exec mongodb mongo playoff_pool --eval "db.brackets.count()" --quiet)
VERIFY_COUNT=$(docker exec mongodb mongo playoff_pool_verify --eval "db.brackets.count()" --quiet)

if [ "$ORIGINAL_COUNT" -eq "$VERIFY_COUNT" ]; then
    echo "Backup verified successfully"
    # Clean up verification database
    docker exec mongodb mongo playoff_pool_verify --eval "db.dropDatabase()"
else
    echo "Backup verification failed"
    exit 1
fi

# Cleanup old backups (keep 30 days)
find ${BACKUP_DIR} -name "nflpool_backup_*" -mtime +30 -delete
```

## Configuration Management Patterns

### Deployment-Managed Environment Pattern
- **Pattern**: Environment Variables Set by Deployment System
- **Implementation**: Azure DevOps pipeline sets environment variables during deployment
- **Rationale**: Centralized configuration management with environment-specific values
- **Pipeline Configuration**:
  ```yaml
  # Azure DevOps pipeline variables
  variables:
    - group: nflpool-local-vars    # Local development
    - group: nflpool-test-vars     # Test environment  
    - group: nflpool-prod-vars     # Production environment
  ```

### Configuration Validation Pattern
- **Pattern**: Startup Environment Validation
- **Implementation**: Application validates required environment variables on startup
- **Rationale**: Fail fast if configuration is incomplete or invalid
- **Implementation**:
  ```csharp
  // Program.cs - Configuration validation
  public static void Main(string[] args)
  {
      var builder = WebApplication.CreateBuilder(args);
      
      // Validate required configuration
      ValidateConfiguration(builder.Configuration);
      
      // Continue with application setup...
  }
  
  private static void ValidateConfiguration(IConfiguration config)
  {
      var requiredSettings = new[]
      {
          "ConnectionStrings:MongoDb",
          "MONGODB_USERNAME",
          "MONGODB_PASSWORD"
      };
      
      foreach (var setting in requiredSettings)
      {
          if (string.IsNullOrEmpty(config[setting]))
          {
              throw new InvalidOperationException($"Required configuration '{setting}' is missing");
          }
      }
  }
  ```

## Monitoring and Observability Patterns

### Docker Native Logging Pattern
- **Pattern**: Standard Docker Logging with Log Rotation
- **Implementation**: Use Docker's built-in logging with size and rotation limits
- **Configuration**:
  ```yaml
  services:
    webapp:
      logging:
        driver: "json-file"
        options:
          max-size: "10m"
          max-file: "3"
    mongodb:
      logging:
        driver: "json-file"
        options:
          max-size: "10m"
          max-file: "3"
  ```

### Health Check Integration Pattern
- **Pattern**: Multi-Level Health Monitoring
- **Implementation**: 
  - Docker health checks for container-level monitoring
  - Application health endpoint for service-level monitoring
  - MongoDB connection check for database-level monitoring
- **Health Endpoint Implementation**:
  ```csharp
  [ApiController]
  [Route("[controller]")]
  public class HealthController : ControllerBase
  {
      private readonly PlayoffPoolContext _context;
      
      public HealthController(PlayoffPoolContext context)
      {
          _context = context;
      }
      
      [HttpGet]
      public async Task<IActionResult> Get()
      {
          try
          {
              // Test database connectivity
              await _context.Database.CanConnectAsync();
              return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
          }
          catch (Exception ex)
          {
              return StatusCode(503, new { status = "unhealthy", error = ex.Message });
          }
      }
  }
  ```

## Deployment Patterns

### Blue-Green Deployment Simulation Pattern
- **Pattern**: Container Replacement with Health Check Validation
- **Implementation**: 
  1. Deploy new container alongside existing
  2. Health check new container
  3. Switch traffic to new container
  4. Remove old container
- **Azure DevOps Implementation**:
  ```bash
  # Deployment script
  # Pull new image
  docker pull ghcr.io/username/nflplayoffpool:${BUILD_ID}
  
  # Start new container with temporary name
  docker-compose -f docker-compose.prod.yml up -d --scale webapp=2
  
  # Health check new container
  docker exec nflplayoffpool_webapp_2 curl -f http://localhost:5000/health
  
  # If healthy, remove old container
  docker stop nflplayoffpool_webapp_1
  docker rm nflplayoffpool_webapp_1
  
  # Rename new container
  docker rename nflplayoffpool_webapp_2 nflplayoffpool_webapp_1
  ```

## Error Handling and Resilience Patterns

### Graceful Degradation Pattern
- **Pattern**: Application Continues with Reduced Functionality
- **Implementation**: Handle MongoDB connection failures gracefully
- **Example**:
  ```csharp
  public async Task<IActionResult> Index()
  {
      try
      {
          var brackets = await _context.Brackets.ToListAsync();
          return View(brackets);
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Database connection failed");
          return View("DatabaseUnavailable");
      }
  }
  ```

### Circuit Breaker Pattern (Simplified)
- **Pattern**: Fail Fast on Repeated Database Failures
- **Implementation**: Track consecutive failures and temporarily disable database operations
- **Rationale**: Prevent cascading failures and improve user experience

## Performance Optimization Patterns

### Connection Pooling Pattern
- **Pattern**: MongoDB Connection Pool Optimization
- **Configuration**:
  ```csharp
  builder.Services.AddDbContext<PlayoffPoolContext>(options =>
  {
      options.UseMongoDB(connectionString, databaseName, mongoOptions =>
      {
          mongoOptions.MaxConnectionPoolSize = 10;
          mongoOptions.MinConnectionPoolSize = 2;
          mongoOptions.MaxConnectionIdleTime = TimeSpan.FromMinutes(10);
      });
  });
  ```

### Caching Pattern (Future Enhancement)
- **Pattern**: In-Memory Caching for Static Data
- **Implementation**: Cache season data, team information, and leaderboards
- **Note**: Not implemented initially but designed for easy addition