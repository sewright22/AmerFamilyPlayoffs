# Infrastructure Design Plan - Infrastructure Modernization

## Design Checklist

- [x] Design Docker Compose service architecture with MongoDB and web application containers
- [x] Design Ubuntu VM infrastructure on Proxmox with resource allocation and networking
- [x] Design Azure DevOps agent installation and configuration for home server deployment
- [x] Design GitHub Container Registry integration with automated image building
- [x] Design CloudFlare tunnel architecture for secure external access
- [x] Design backup infrastructure with automated MongoDB backup and verification
- [x] Design deployment architecture with staging and production environments
- [x] Design rollback procedures and disaster recovery approach

## Infrastructure Design Questions

Based on the approved NFR design patterns, I need to clarify specific infrastructure implementation details:

### Docker Compose Infrastructure

**Q1.1**: Should the Docker Compose setup include development and production configurations?
A) Single docker-compose.yml for all environments
B) Separate files: docker-compose.yml (dev) and docker-compose.prod.yml (production)
C) Base file with override files for each environment
X) Other Docker Compose structure (please describe after [Answer]: tag)

[Answer]: A

**Q1.2**: How should container resource limits be configured?
A) No resource limits (use host defaults)
B) Conservative limits (1GB RAM, 1 CPU for web app, 2GB RAM for MongoDB)
C) Generous limits (2GB RAM, 2 CPU for web app, 4GB RAM for MongoDB)
X) Other resource allocation approach (please describe after [Answer]: tag)

[Answer]: B

### Ubuntu VM Infrastructure

**Q2.1**: What should be the VM resource allocation on Proxmox?
A) Minimal: 2 vCPU, 4GB RAM, 50GB storage
B) Standard: 4 vCPU, 8GB RAM, 100GB storage
C) Generous: 6 vCPU, 16GB RAM, 200GB storage
X) Other resource specification (please describe after [Answer]: tag)

[Answer]: A

**Q2.2**: How should the VM storage be configured?
A) Single disk with all data
B) Separate disks for OS and application data
C) LVM with logical volumes for different purposes
X) Other storage configuration (please describe after [Answer]: tag)

[Answer]: X Recommended proxmox setup

### Azure DevOps Agent Infrastructure

**Q3.1**: How should the Azure DevOps agent be installed on the Ubuntu VM?
A) Docker container running the agent
B) Native installation as systemd service
C) Manual installation for testing, then automate
X) Other agent installation approach (please describe after [Answer]: tag)

[Answer]: B

**Q3.2**: Should the agent run with dedicated user permissions?
A) Yes, create dedicated 'azureagent' user with Docker group membership
B) Yes, but use existing user account with appropriate permissions
C) No, run as root for simplicity
X) Other permission approach (please describe after [Answer]: tag)

[Answer]: A

### GitHub Container Registry Infrastructure

**Q4.1**: How should GitHub Container Registry authentication be configured?
A) Personal Access Token (PAT) stored in Azure DevOps variables
B) GitHub App with repository-specific permissions
C) Service account with minimal required permissions
X) Other authentication approach (please describe after [Answer]: tag)

[Answer]: A

**Q4.2**: Should container images include version tagging strategy?
A) Simple: latest + git commit SHA
B) Semantic: latest + version tags (v1.0.0) + git SHA
C) Environment-based: latest + environment tags (dev, test, prod) + git SHA
X) Other tagging strategy (please describe after [Answer]: tag)

[Answer]: B

### CloudFlare Tunnel Infrastructure

**Q5.1**: How should the CloudFlare tunnel be configured to avoid conflicts with existing setup?
A) Create new tunnel with different subdomain (app.fjapool.com)
B) Create new tunnel with test subdomain first (test.fjapool.com), then migrate
C) Use existing tunnel but add new service configuration
X) Other tunnel configuration approach (please describe after [Answer]: tag)

[Answer]: X My other tunnel is for a different domain

**Q5.2**: Should the tunnel include SSL/TLS termination configuration?
A) CloudFlare handles all SSL/TLS (Full SSL mode)
B) End-to-end encryption with self-signed certificates
C) End-to-end encryption with Let's Encrypt certificates
X) Other SSL/TLS approach (please describe after [Answer]: tag)

[Answer]: A: assuming free or cheap

### Backup Infrastructure

**Q6.1**: Where should MongoDB backups be stored?
A) Local filesystem on Ubuntu VM only
B) Local filesystem + cloud storage sync (Google Drive, OneDrive)
C) Local filesystem + separate backup server/NAS
X) Other backup storage approach (please describe after [Answer]: tag)

[Answer]: A

**Q6.2**: How should backup retention be managed?
A) Simple: Keep 30 daily backups, delete older
B) Tiered: 7 daily, 4 weekly, 12 monthly backups
C) Custom retention based on available storage
X) Other retention strategy (please describe after [Answer]: tag)

[Answer]: A

### Deployment Infrastructure

**Q7.1**: How should deployment environments be structured?
A) Single production environment on home server
B) Staging environment (same VM, different ports) + production
C) Separate VMs for staging and production
X) Other environment structure (please describe after [Answer]: tag)

[Answer]: B

**Q7.2**: Should deployment include automated rollback capabilities?
A) Yes, keep previous container images and quick rollback script
B) Yes, but manual rollback process with documented procedures
C) No, rely on backup restoration for rollback
X) Other rollback approach (please describe after [Answer]: tag)

[Answer]: B

### Monitoring and Logging Infrastructure

**Q8.1**: How should application and infrastructure logs be managed?
A) Docker logs with rotation, accessible via docker logs command
B) Centralized logging with log aggregation (ELK stack or similar)
C) File-based logging with log rotation and remote backup
X) Other logging approach (please describe after [Answer]: tag)

[Answer]: X: simplest approach

**Q8.2**: Should infrastructure monitoring be implemented?
A) Basic: Docker health checks and manual monitoring
B) Intermediate: Prometheus + Grafana for metrics and alerting
C) Advanced: Full monitoring stack with alerting and notifications
X) Other monitoring approach (please describe after [Answer]: tag)

[Answer]: A

---

**Instructions**: Please fill in all [Answer]: tags above. For multiple choice questions, simply put the letter (A, B, C, etc.) after the [Answer]: tag. For "Other" options, provide your specific requirements after the [Answer]: tag.