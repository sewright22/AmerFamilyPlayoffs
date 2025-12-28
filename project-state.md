# NFL Playoff Pool - Project State

## Project Overview
A web application for managing NFL playoff bracket pools where users can create brackets, make picks, and compete with friends during the NFL playoffs.

## Current Status
- **Phase**: Unit Testing Implementation ✅
- **Local Development**: ✅ Working with Docker Compose + MongoDB
- **Admin Account**: ✅ Default admin user seeded automatically
- **Testing Infrastructure**: ✅ Unit test projects and shared test infrastructure
- **Next Phase**: Expand test coverage and continue feature development

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 (C#)
- **Database**: MongoDB (containerized)
- **ORM**: Entity Framework Core with MongoDB provider
- **Authentication**: Cookie-based authentication

### Frontend
- **Framework**: ASP.NET Core MVC with Razor views
- **Styling**: Bootstrap (existing)
- **JavaScript**: jQuery (existing)

### Infrastructure
- **Containerization**: Docker Compose
- **Database**: MongoDB in Docker container with admin authentication
- **Database Seeding**: Automatic admin user creation with secure password validation
- **Deployment**: Planned for home server with CloudFlare tunnel

## Project Structure

```
src/
├── NflPlayoffPool.Data/           # Data layer
│   ├── Models/                    # Entity models (User, Bracket, Season, etc.)
│   ├── PlayoffPoolContext.cs      # EF Core DbContext
│   └── MFlixDbContext.cs          # Legacy context (to be removed)
├── NflPlayoffPool.Web/            # Web application
│   ├── Controllers/               # MVC controllers
│   ├── Extensions/                # Extension methods (MongoDB, etc.)
│   ├── Models/                    # View models and DTOs
│   ├── Services/                  # Business services
│   ├── Views/                     # Razor views
│   └── Areas/Admin/               # Admin area
├── NflPlayoffPool.WebTests/       # Web layer unit tests
├── NflPlayoffPool.Data.Tests/     # Data layer unit tests
└── NflPlayoffPool.TestCommon/     # Shared test infrastructure
```

## Key Components

### Data Models
- **User**: User accounts with roles (Admin, Player)
- **Bracket**: User's playoff bracket predictions
- **Season**: NFL season configuration
- **Team**: NFL teams and playoff participants
- **Game**: Individual playoff games

### Authentication & Authorization
- Cookie-based authentication
- Role-based authorization (Admin, Player)
- User registration and login functionality

### Core Features (Existing)
- User registration and authentication
- Admin user management
- Bracket creation and management
- Season management
- Super Grid import functionality

## Development Standards
- **Architecture**: Framework-first approach, direct DbContext usage
- **Code Style**: Follows personal coding standards in `.kiro/steering/`
- **Testing**: Pragmatic testing approach focusing on business logic
- **Documentation**: Inline code documentation with XML comments

## Environment Configuration
- **Local**: Docker Compose with `.env` file
- **Database**: MongoDB with admin authentication
- **Health Checks**: Built-in ASP.NET Core health checks
- **Admin Seeding**: Automatic default admin account creation on startup

### Testing Infrastructure
- **Unit Tests**: Comprehensive test coverage for complex business logic
- **Test Projects**: One-to-one mapping (Web → Web.Tests, Data → Data.Tests)
- **Shared Infrastructure**: Common test builders and utilities in TestCommon project
- **Focus Areas**: Complex scoring calculations, bracket logic, elimination rules

## Recent Infrastructure Work
- ✅ Containerized MongoDB setup
- ✅ Docker Compose configuration
- ✅ Environment variable management
- ✅ Health check implementation
- ✅ Deployment script creation
- ✅ Database seeding with secure admin account
- ✅ Password security validation (12+ chars, complexity requirements)
- ✅ Security standards documentation
- ✅ Unit testing infrastructure with shared test builders
- ✅ Comprehensive tests for complex bracket scoring logic

## Immediate Next Steps
1. ✅ Database seeding with default admin account - **COMPLETE**
2. ✅ Unit testing infrastructure for complex business logic - **COMPLETE**
3. Expand test coverage to additional business logic areas
4. Application feature development and enhancements
5. Home server deployment pipeline
6. CloudFlare tunnel configuration

## Documentation
- **Setup**: `DOCKER_SETUP.md` - Local development setup
- **Audit**: `aidlc-docs/audit.md` - Complete development history
- **Standards**: `.kiro/steering/personal-coding-standards.md`
- **Security**: `.kiro/steering/security-standards.md` - Password and security requirements

---
*Last Updated: 2025-12-20*
*Status: Unit testing infrastructure complete, comprehensive tests for bracket scoring logic implemented*