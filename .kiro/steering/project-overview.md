---
inclusion: always
---

# Project Overview and Context

This steering document provides essential context about the NFL Playoff Pool project by referencing the current project state.

## Project State Reference

#[[file:project-state.md]]

## Development Context

When working on this project, keep these key points in mind:

### Architecture Principles
- **Framework-First**: Leverage ASP.NET Core and EF Core capabilities directly
- **Direct DbContext Usage**: Avoid repository pattern over EF Core (see personal coding standards)
- **Pragmatic Solutions**: Choose simplicity over theoretical "best practices"

### Current Development Phase
The project has recently completed infrastructure modernization and is now ready for:
1. Database seeding implementation
2. Feature development and enhancements
3. Deployment pipeline setup

### Key Technical Decisions
- **MongoDB with EF Core**: Using Entity Framework Core MongoDB provider for familiar LINQ queries
- **Cookie Authentication**: Simple, effective authentication for this use case
- **Docker Compose**: Single-file approach for all environments
- **Extension Methods**: Clean separation of concerns for MongoDB configuration

### Working with the Codebase
- **Data Access**: Use `PlayoffPoolContext` directly in controllers and services
- **User Management**: Existing user system with Admin/Player roles
- **Authentication**: Cookie-based with login/logout functionality
- **Admin Features**: Separate admin area for management functions

### Environment Setup
- Local development uses Docker Compose with MongoDB
- Environment variables configured via `.env` file
- Health checks implemented for monitoring
- Deployment scripts ready for home server deployment

### Testing Approach
- Focus on business logic testing
- Use in-memory databases for integration tests
- Avoid mocking framework components (EF Core, etc.)
- Pragmatic test coverage based on risk and complexity

## Quick Start for New Contributors

1. **Setup**: Follow `DOCKER_SETUP.md` for local environment
2. **Standards**: Review `.kiro/steering/personal-coding-standards.md`
3. **Security**: Review `.kiro/steering/security-standards.md` - NEVER hardcode passwords
4. **History**: Check `aidlc-docs/audit.md` for recent changes
5. **Architecture**: Understand the direct DbContext approach
6. **Features**: Explore existing controllers and models for patterns

## Common Patterns in This Project

### Data Access Pattern
```csharp
public class BracketController : Controller
{
    private readonly PlayoffPoolContext _context;
    
    public BracketController(PlayoffPoolContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> GetBrackets()
    {
        var brackets = await _context.Brackets
            .Where(b => b.UserId == GetCurrentUserId())
            .Include(b => b.Picks)
            .ToListAsync();
        return View(brackets);
    }
}
```

### Extension Method Pattern
```csharp
// Clean separation of concerns
public static class MongoDbExtensions
{
    public static IServiceCollection AddContainerizedMongoDB(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configuration logic here
    }
}
```

### Authentication Pattern
```csharp
// Simple, effective authentication
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Admin-only actions
}
```

This project follows a pragmatic, framework-first approach that prioritizes working software over theoretical perfection.