# Component Inventory

## Application Packages
- **NflPlayoffPool.Web** - Main ASP.NET Core web application providing user interface and business logic for NFL playoff bracket management

## Infrastructure Packages
- **Docker Configuration** - Containerization setup with Dockerfile and docker-compose.yml for deployment
- **Azure DevOps Pipelines** - CI/CD configuration with build.yml for automated deployment to test and production environments

## Shared Packages
- **NflPlayoffPool.Data** - Data access layer with Entity Framework Core MongoDB integration and domain models

## Test Packages
- **NflPlayoffPool.WebTests** - Unit and integration test suite for web application components and services

## Configuration Files
- **stylecop.json** - Code style and analysis rules shared across projects
- **.gitignore** - Git version control exclusions
- **.gitattributes** - Git file handling attributes

## Total Count
- **Total Packages**: 4
- **Application**: 1 (NflPlayoffPool.Web)
- **Infrastructure**: 2 (Docker, Azure DevOps)
- **Shared**: 1 (NflPlayoffPool.Data)
- **Test**: 1 (NflPlayoffPool.WebTests)

## Detailed Component Breakdown

### Web Application Components
- **Controllers**: 4 controllers (Account, Admin, Bracket, Home)
- **Models**: Domain models, view models, and DTOs
- **Views**: Razor templates for user interface
- **Services**: Business logic services
- **Extensions**: Helper and extension methods
- **Static Assets**: CSS, JavaScript, images in wwwroot

### Data Layer Components
- **Entity Models**: 15+ domain entities (Bracket, User, Season, Team, etc.)
- **DbContext**: PlayoffPoolContext for MongoDB integration
- **Migrations**: Database schema management (if applicable)

### Infrastructure Components
- **Docker**: Multi-stage Dockerfile with .NET 8.0 runtime
- **CI/CD**: Azure DevOps pipeline with local agent pool deployment
- **Configuration**: Environment-specific appsettings files

### External Dependencies
- **MongoDB.EntityFrameworkCore**: 8.2.0
- **MongoDB.Driver**: 3.0.0
- **EPPlus**: 7.6.1 (Excel processing)
- **ASP.NET Core**: .NET 8.0 framework components