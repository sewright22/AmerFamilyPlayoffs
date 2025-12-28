# Technology Stack

## Programming Languages
- **C#** - 12.0 (.NET 8.0) - Primary application development language for web application and data access layer

## Frameworks
- **ASP.NET Core** - 8.0 - Web application framework providing MVC, authentication, and HTTP handling
- **Entity Framework Core** - 8.0 - Object-relational mapping framework for database operations
- **MongoDB.EntityFrameworkCore** - 8.2.0 - MongoDB provider for Entity Framework Core

## Infrastructure
- **Docker** - Containerization platform for application deployment
- **MongoDB** - NoSQL document database for data persistence
- **Azure DevOps** - CI/CD pipeline and source control management
- **Local Agent Pool** - Self-hosted build and deployment agents

## Build Tools
- **.NET SDK** - 8.0 - Primary build and compilation toolchain
- **Docker** - Multi-stage container builds
- **MSBuild** - Project compilation and packaging

## Testing Tools
- **MSTest/xUnit** - Unit testing framework (inferred from test project structure)
- **.NET Test SDK** - Test execution and reporting

## Development Tools
- **StyleCop** - Code style analysis and enforcement
- **Visual Studio** - Primary IDE (inferred from solution structure)

## Web Technologies
- **HTML5/CSS3** - Frontend markup and styling
- **JavaScript** - Client-side scripting
- **Razor Pages** - Server-side templating engine
- **Bootstrap** - CSS framework (likely, common in ASP.NET Core projects)

## Authentication & Security
- **Cookie Authentication** - Built-in ASP.NET Core authentication
- **HTTPS** - SSL/TLS encryption (configured but currently disabled)
- **Anti-forgery Tokens** - CSRF protection

## Data Processing
- **EPPlus** - 7.6.1 - Excel file generation and processing library

## Deployment & Operations
- **Docker Compose** - Multi-container orchestration
- **Linux Containers** - Target deployment platform
- **Kestrel** - ASP.NET Core web server
- **Forwarded Headers** - Reverse proxy support configuration

## Configuration Management
- **appsettings.json** - Application configuration
- **Environment Variables** - Runtime configuration overrides
- **User Secrets** - Development-time secret management

## Version Control
- **Git** - Source control system
- **.gitignore/.gitattributes** - Repository configuration

## Current Technology Gaps (Based on Goals)
- **Local MongoDB Container** - Currently using external MongoDB connection
- **CloudFlare Tunnel** - Not yet configured for home server access
- **Home Server Deployment** - Pipeline currently targets local environments but may need updates
- **MCP Server** - May be needed for advanced deployment scenarios