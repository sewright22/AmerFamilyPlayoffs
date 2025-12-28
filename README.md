# NFL Playoff Pool Application

🏈 **A web application for managing NFL playoff bracket pools and competitions**

## Project Information

- **Azure DevOps Organization**: `sewright22`
- **Project**: `StevenCodesWright`
- **Repository**: `NflPlayoffPool`
- **Repository ID**: `35a2ba1f-873c-4861-96b0-2a1e29bd7655`
- **Web URL**: https://dev.azure.com/sewright22/StevenCodesWright/_git/NflPlayoffPool

## Technology Stack

- **Backend**: ASP.NET Core 8.0 with C#
- **Database**: MongoDB with Entity Framework Core
- **Frontend**: Razor Pages with Bootstrap
- **Authentication**: Cookie-based authentication
- **Containerization**: Docker with Docker Compose
- **Deployment**: Azure DevOps Pipelines to Ubuntu VMs

## Quick Start

### Test Server Access
- **URL**: http://192.168.68.105:5000
- **Admin**: Use credentials from your .env file
- **Deployment Guide**: See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

### Local Development

1. **Prerequisites**: .NET 8.0 SDK, Docker and Docker Compose, Git

2. **Setup**:
   ```bash
   git clone https://github.com/sewright22/AmerFamilyPlayoffs
   cd AmerFamilyPlayoffs
   cp .env.template .env
   # Edit .env with your settings
   ```

3. **Run**: `docker-compose up -d`

4. **Access**: http://localhost:5000

## Deployment Pipelines

- **Build Pipeline**: `.azuredevops/build.yml` - Builds and deploys to test/prod
- **VM Setup**: `.azuredevops/setup-vm.yml` - Configures Ubuntu VMs
- **Proxmox Deploy**: `.azuredevops/proxmox-deploy.yml` - Enhanced deployment

## Key Features

- User Management with Admin/Player roles
- Bracket Management and Scoring
- Responsive Design
- Health Monitoring
- Automated Deployments

## Development Guidelines

- Framework-First Approach (ASP.NET Core, EF Core)
- Direct DbContext Usage (no repository pattern)
- Pragmatic Architecture
- Security First (environment variables for secrets)

---

**Maintainer**: Steven Wright (sewright22@gmail.com)