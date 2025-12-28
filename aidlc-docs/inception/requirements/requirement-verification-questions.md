# Requirements Verification Questions

## Infrastructure Modernization Requirements

Based on your stated goals for containerizing MongoDB, updating deployment pipelines, and setting up CloudFlare tunnels, I need to clarify several details to ensure successful implementation.

### 1. Local Docker MongoDB Setup

**Q1.1**: What MongoDB version should be used in the Docker container?
A) MongoDB 7.0 (latest stable)
B) MongoDB 6.0 (LTS version)
C) MongoDB 5.0 (older stable)
X) Other (please specify version after [Answer]: tag below)

[Answer]: B

**Q1.2**: Do you need MongoDB data persistence between container restarts?
A) Yes, use Docker volumes for data persistence
B) No, ephemeral data is acceptable for local development
X) Other requirements (please describe after [Answer]: tag below)

[Answer]: A

**Q1.3**: Should MongoDB authentication be enabled in the containerized setup?
A) Yes, with username/password authentication
B) No, run without authentication for local development
C) Yes, but only for production deployment
X) Other authentication approach (please describe after [Answer]: tag below)

[Answer]: C

### 2. Docker Compose Configuration

**Q2.1**: Should the application and MongoDB run in the same Docker Compose stack?
A) Yes, single docker-compose.yml with both services
B) No, separate MongoDB container that application connects to
C) Depends on environment (local vs production)
X) Other approach (please describe after [Answer]: tag below)

[Answer]: X Best practice for simple maintenance and data backups.

**Q2.2**: What networking approach should be used between application and MongoDB containers?
A) Docker Compose default network
B) Custom Docker network
C) Host networking
X) Other networking setup (please describe after [Answer]: tag below)

[Answer]: A

### 3. Home Server Deployment

**Q3.1**: What type of home server environment are you deploying to?
A) Linux server (Ubuntu/Debian)
B) Windows server
C) Docker-capable NAS (Synology/QNAP)
D) Raspberry Pi or ARM-based device
X) Other (please specify after [Answer]: tag below)

[Answer]: X I have ProxMox, I am willing to install ubuntu server in a vm if that is the best practice for my setup.

**Q3.2**: How should the application be exposed on your home server?
A) Direct port exposure (5000/5001)
B) Behind reverse proxy (nginx/traefik)
C) Through CloudFlare tunnel only
X) Other approach (please describe after [Answer]: tag below)

[Answer]: X: Externally through cloudflare. Local can have port exposure.

**Q3.3**: Do you need SSL/HTTPS termination on the home server?
A) Yes, with Let's Encrypt certificates
B) Yes, with self-signed certificates
C) No, SSL handled by CloudFlare
D) Yes, with existing certificates
X) Other SSL approach (please describe after [Answer]: tag below)

[Answer]: C

### 4. Azure DevOps Pipeline Updates

**Q4.1**: Should the pipeline build and push Docker images to a registry?
A) Yes, to Docker Hub
B) Yes, to Azure Container Registry
C) Yes, to GitHub Container Registry
D) No, build and transfer image files directly
X) Other registry or approach (please describe after [Answer]: tag below)

[Answer]: X: Github assuming I can do it for free.

**Q4.2**: How should deployment to home server be triggered?
A) Automatic deployment on main branch commits
B) Manual approval required for production deployment
C) Separate pipelines for test and production
D) Deploy on release tags only
X) Other deployment trigger (please describe after [Answer]: tag below)

[Answer]: A: Production needs approval. Test does not.

**Q4.3**: How should the pipeline connect to your home server?
A) SSH with key-based authentication
B) Azure DevOps agent installed on home server
C) VPN connection to home network
D) Through CloudFlare tunnel
X) Other connection method (please describe after [Answer]: tag below)

[Answer]: B

### 5. CloudFlare Tunnel Configuration

**Q5.1**: Do you already have a CloudFlare account and domain configured?
A) Yes, domain already managed by CloudFlare
B) Yes, CloudFlare account but need to add domain
C) No, need to set up both account and domain
X) Other situation (please describe after [Answer]: tag below)

[Answer]: A

**Q5.2**: What subdomain/domain should the application be accessible from?
A) Use existing domain (please specify after [Answer]: tag)
B) Create new subdomain (please specify after [Answer]: tag)
C) Use CloudFlare's provided domain
X) Other domain approach (please describe after [Answer]: tag below)

[Answer]: A: fjapool.com (test should have subdomain test.fjapool.com)

**Q5.3**: Should the CloudFlare tunnel be the only way to access the application?
A) Yes, block all direct access to home server
B) No, allow both tunnel and direct access
C) Tunnel for external access, direct for internal network
X) Other access control (please describe after [Answer]: tag below)

[Answer]: A: I have other tunnels for HomeAssistant and a different domain. I don't want to interfere with existing tunnel. That tunnel goes to a different vm in Proxmox.

### 6. Environment and Configuration Management

**Q6.1**: How should environment-specific configuration be managed?
A) Environment variables in Docker Compose
B) Separate appsettings files for each environment
C) Azure DevOps variable groups
D) Configuration files on the server
X) Other configuration approach (please describe after [Answer]: tag below)

[Answer]: A

**Q6.2**: Should database connection strings be stored securely?
A) Yes, use Docker secrets
B) Yes, use environment variables
C) Yes, use Azure Key Vault
D) Store in configuration files on server
X) Other secure storage method (please describe after [Answer]: tag below)

[Answer]: A

### 7. Backup and Monitoring

**Q7.1**: Do you need automated MongoDB backups?
A) Yes, daily backups with retention policy
B) Yes, but manual backup process is acceptable
C) No, backups not needed for this application
X) Other backup requirements (please describe after [Answer]: tag below)

[Answer]: A

**Q7.2**: What level of monitoring/logging is needed?
A) Basic application logs only
B) Application logs + MongoDB logs
C) Full monitoring with health checks and alerts
D) No additional monitoring beyond current setup
X) Other monitoring requirements (please describe after [Answer]: tag below)

[Answer]: D: This will be addressed later

### 8. Migration and Rollback Strategy

**Q8.1**: How should existing data be migrated to the new MongoDB container?
A) Export from current MongoDB and import to container
B) Start fresh with empty database
C) Provide migration scripts for data transfer
X) Other migration approach (please describe after [Answer]: tag below)

[Answer]: B

**Q8.2**: What rollback strategy should be in place?
A) Keep previous deployment available for quick rollback
B) Database backup before each deployment
C) Blue-green deployment approach
D) Simple rollback to previous Docker image
X) Other rollback strategy (please describe after [Answer]: tag below)

[Answer]: D

---

**Instructions**: Please fill in all [Answer]: tags above. For multiple choice questions, simply put the letter (A, B, C, etc.) after the [Answer]: tag. For "Other" options, provide your specific requirements after the [Answer]: tag.

## Follow-up Questions

**Q2.1 (Missing Answer)**: Should the application and MongoDB run in the same Docker Compose stack?
A) Yes, single docker-compose.yml with both services
B) No, separate MongoDB container that application connects to
C) Depends on environment (local vs production)
X) Other approach (please describe after [Answer]: tag below)

[Answer]: Answered above

**Q9.1 (CloudFlare Tunnel Clarification)**: Since you mentioned you have existing tunnels for HomeAssistant on a different domain/VM, should the NFL Playoff Pool application:
A) Use a completely separate CloudFlare tunnel instance
B) Share the existing tunnel but route to different VM
C) Create new tunnel configuration within existing CloudFlare setup
X) Other approach (please describe after [Answer]: tag below)

[Answer]: C: I don't know best practice in this situation. I don't want to risk messing up existing tunnel.

**Q9.2 (GitHub Container Registry Clarification)**: You mentioned GitHub "assuming I can do it for free" - GitHub Container Registry (ghcr.io) is free for public repositories. Will this be:
A) Public repository (free GitHub Container Registry)
B) Private repository (may have usage limits)
C) Need to verify GitHub pricing before deciding
X) Other consideration (please describe after [Answer]: tag below)

[Answer]: A