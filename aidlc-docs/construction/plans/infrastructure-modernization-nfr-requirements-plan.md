# NFR Requirements Assessment Plan - Infrastructure Modernization

## Assessment Checklist

- [ ] Analyze scalability requirements for containerized setup
- [ ] Determine performance requirements for Docker and MongoDB
- [ ] Assess availability and reliability requirements
- [ ] Evaluate security requirements for production deployment
- [ ] Determine tech stack choices for infrastructure components
- [ ] Assess monitoring and observability needs
- [ ] Evaluate backup and disaster recovery requirements
- [ ] Determine operational and maintainability requirements

## NFR Requirements Questions

### Scalability Requirements

**Q1.1**: What is the expected user load for the NFL Playoff Pool application?
A) Small (< 50 concurrent users)
B) Medium (50-500 concurrent users)
C) Large (500-5000 concurrent users)
D) Very Large (> 5000 concurrent users)
X) Other or variable load (please describe after [Answer]: tag)

[Answer]: A

**Q1.2**: Do you anticipate significant traffic spikes during specific periods (e.g., playoff season)?
A) Yes, during NFL playoff season (January-February)
B) Yes, but manageable with current setup
C) No, traffic is relatively consistent
X) Other pattern (please describe after [Answer]: tag)

[Answer]: B

**Q1.3**: Should the containerized setup support horizontal scaling (multiple application instances)?
A) Yes, design for horizontal scaling from the start
B) No, single instance is sufficient for now
C) Maybe in the future, but not initially required
X) Other scaling approach (please describe after [Answer]: tag)

[Answer]: B 

### Performance Requirements

**Q2.1**: What are acceptable response time requirements for the application?
A) < 200ms for page loads
B) < 500ms for page loads
C) < 1 second for page loads
D) < 2 seconds for page loads
X) Other performance target (please describe after [Answer]: tag)

[Answer]: D

**Q2.2**: What are acceptable MongoDB query performance requirements?
A) < 50ms for typical queries
B) < 100ms for typical queries
C) < 500ms for typical queries
D) No specific requirement, current performance is acceptable
X) Other performance target (please describe after [Answer]: tag)

[Answer]: D

**Q2.3**: Should Docker resource limits be configured for the containers?
A) Yes, set specific CPU and memory limits
B) No, let containers use available resources
C) Set limits only for production, not for local development
X) Other resource management approach (please describe after [Answer]: tag)

[Answer]: B 

### Availability Requirements

**Q3.1**: What is the acceptable downtime for planned maintenance?
A) Zero downtime required (blue-green deployment)
B) Brief downtime acceptable (< 5 minutes)
C) Moderate downtime acceptable (< 30 minutes)
D) Flexible, can schedule maintenance windows
X) Other availability requirement (please describe after [Answer]: tag)

[Answer]: D

**Q3.2**: What is the target uptime percentage for the production environment?
A) 99.9% (< 9 hours downtime per year)
B) 99% (< 4 days downtime per year)
C) 95% (< 18 days downtime per year)
D) Best effort, no specific SLA
X) Other uptime target (please describe after [Answer]: tag)

[Answer]: D

**Q3.3**: Should the system include health checks and automatic restart capabilities?
A) Yes, Docker health checks with automatic container restart
B) Yes, but manual intervention for restarts
C) No, manual monitoring is sufficient
X) Other health check approach (please describe after [Answer]: tag)

[Answer]: A 

### Security Requirements

**Q4.1**: What level of network isolation is required between containers?
A) Strict isolation with custom Docker networks
B) Default Docker Compose networking is sufficient
C) No specific isolation requirements
X) Other network security approach (please describe after [Answer]: tag)

[Answer]: B

**Q4.2**: Should MongoDB be accessible from outside the Docker network?
A) No, only accessible from application container
B) Yes, for administrative access from host machine
C) Yes, for backup tools and monitoring
X) Other access pattern (please describe after [Answer]: tag)

[Answer]: A

**Q4.3**: What authentication mechanism should be used for MongoDB in production?
A) Username/password with strong password policy
B) Username/password with certificate-based authentication
C) SCRAM-SHA-256 authentication (MongoDB default)
D) No authentication (rely on network isolation)
X) Other authentication approach (please describe after [Answer]: tag)

[Answer]: C

**Q4.4**: Should Docker secrets be rotated periodically?
A) Yes, implement secret rotation policy
B) No, set once during initial setup
C) Manual rotation when needed
X) Other secret management approach (please describe after [Answer]: tag)

[Answer]: B

**Q4.5**: Should the CloudFlare tunnel use additional authentication beyond the tunnel itself?
A) Yes, add application-level authentication
B) No, CloudFlare tunnel security is sufficient
C) Use CloudFlare Access for additional security layer
X) Other security approach (please describe after [Answer]: tag)

[Answer]: B 

### Tech Stack Choices

**Q5.1**: Which MongoDB Docker image should be used?
A) Official MongoDB image (mongo:6.0)
B) Bitnami MongoDB image (more security features)
C) Custom MongoDB image with specific configurations
X) Other MongoDB image (please specify after [Answer]: tag)

[Answer]: A

**Q5.2**: Should Docker Compose profiles be used for different environments?
A) Yes, separate profiles for local/test/production
B) No, use environment variables only
C) Use separate docker-compose files for each environment
X) Other environment management approach (please describe after [Answer]: tag)

[Answer]: B

**Q5.3**: Which CloudFlare tunnel client should be used?
A) cloudflared (official CloudFlare tunnel daemon)
B) CloudFlare Zero Trust dashboard configuration
C) Terraform for infrastructure-as-code tunnel setup
X) Other tunnel setup approach (please describe after [Answer]: tag)

[Answer]: A

**Q5.4**: Should the backup solution use a specific tool or custom scripts?
A) mongodump/mongorestore (MongoDB native tools)
B) Docker volume backup solution
C) Third-party backup tool (e.g., Percona Backup for MongoDB)
D) Custom backup scripts
X) Other backup approach (please describe after [Answer]: tag)

[Answer]: A 

### Monitoring and Observability

**Q6.1**: What level of application logging is required?
A) Detailed logging (Debug level in development, Info in production)
B) Standard logging (Info level)
C) Minimal logging (Warning and Error only)
D) Current logging is sufficient
X) Other logging requirement (please describe after [Answer]: tag)

[Answer]: D

**Q6.2**: Should container logs be centralized or aggregated?
A) Yes, use centralized logging solution (e.g., ELK stack)
B) Yes, but simple aggregation (e.g., Docker logging driver)
C) No, view logs directly from containers
X) Other logging approach (please describe after [Answer]: tag)

[Answer]: C

**Q6.3**: Should MongoDB performance metrics be monitored?
A) Yes, use MongoDB monitoring tools (e.g., MongoDB Compass, Ops Manager)
B) Yes, but basic metrics only (connection count, query performance)
C) No, monitoring will be addressed in future phase
X) Other monitoring approach (please describe after [Answer]: tag)

[Answer]: C 

### Backup and Disaster Recovery

**Q7.1**: What is the acceptable Recovery Point Objective (RPO) - maximum acceptable data loss?
A) Near-zero (continuous backup or replication)
B) < 1 hour (hourly backups)
C) < 24 hours (daily backups)
D) < 1 week (weekly backups)
X) Other RPO requirement (please describe after [Answer]: tag)

[Answer]: C

**Q7.2**: What is the acceptable Recovery Time Objective (RTO) - maximum acceptable downtime for recovery?
A) < 1 hour (rapid recovery required)
B) < 4 hours (same-day recovery)
C) < 24 hours (next-day recovery)
D) Flexible, no specific RTO
X) Other RTO requirement (please describe after [Answer]: tag)

[Answer]: D

**Q7.3**: Where should backups be stored?
A) Same server (different volume)
B) Different server/NAS on local network
C) Cloud storage (e.g., AWS S3, Azure Blob)
D) Multiple locations (local + cloud)
X) Other backup storage location (please describe after [Answer]: tag)

[Answer]: A

**Q7.4**: How long should backups be retained?
A) 7 days (1 week)
B) 30 days (1 month)
C) 90 days (3 months)
D) 1 year or longer
X) Other retention policy (please describe after [Answer]: tag)

[Answer]: B 

### Operational Requirements

**Q8.1**: Should the deployment process include automated testing before production?
A) Yes, run automated tests in test environment before production deployment
B) Yes, but manual testing is acceptable
C) No, deploy directly to production with manual approval
X) Other testing approach (please describe after [Answer]: tag)

[Answer]: C

**Q8.2**: Should there be a staging/test environment that mirrors production?
A) Yes, identical configuration to production
B) Yes, but simplified version of production
C) No, test environment can be different from production
X) Other environment strategy (please describe after [Answer]: tag)

[Answer]: B

**Q8.3**: What level of documentation is required for operational procedures?
A) Comprehensive documentation (setup, deployment, troubleshooting, recovery)
B) Basic documentation (setup and deployment only)
C) Minimal documentation (README with key commands)
D) No additional documentation needed
X) Other documentation requirement (please describe after [Answer]: tag)

[Answer]: C 

---

**Instructions**: Please fill in all [Answer]: tags above. For multiple choice questions, simply put the letter (A, B, C, etc.) after the [Answer]: tag. For "Other" options, provide your specific requirements after the [Answer]: tag.