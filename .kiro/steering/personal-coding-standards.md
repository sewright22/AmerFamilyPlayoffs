---
inclusion: always
---

# Personal Coding Standards and Preferences

## Overview
This document captures personal coding standards, architectural preferences, and development practices to ensure consistency across all projects and AI-assisted development sessions.

## Core Principles

### 1. Pragmatic Architecture
- Favor simplicity over complexity
- Choose patterns that add genuine value, not just theoretical "best practices"
- Avoid over-engineering and unnecessary abstractions
- Let the framework do what it's designed to do

### 2. Framework-First Approach
- Leverage framework capabilities fully before adding abstractions
- Don't fight the framework - work with its intended patterns
- Prefer framework conventions over custom implementations

## Data Access Standards

### Entity Framework Core Usage

#### ✅ PREFERRED: Direct DbContext Usage
- **Principle**: Use DbContext directly in controllers and services
- **Rationale**: EF Core IS the abstraction layer - adding repository pattern restricts its powerful features
- **Benefits**:
  - Full access to EF Core's rich querying capabilities (Include, Select, GroupBy, etc.)
  - Native support for async operations
  - Change tracking and unit of work patterns built-in
  - LINQ expression trees for optimal query generation
  - No artificial limitations on query complexity

#### ❌ AVOID: Repository Pattern Over DbContext
- **Anti-Pattern**: Wrapping DbContext in generic repository interfaces
- **Problems**:
  - Restricts access to EF Core's advanced features
  - Creates unnecessary abstraction layers
  - Limits query optimization opportunities
  - Adds complexity without meaningful benefits
  - Makes testing more difficult, not easier

#### 📋 Implementation Guidelines
```csharp
// ✅ GOOD: Direct DbContext usage
public class BracketController : Controller
{
    private readonly PlayoffPoolContext _context;
    
    public BracketController(PlayoffPoolContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> GetUserBrackets(string userId)
    {
        var brackets = await _context.Brackets
            .Where(b => b.UserId == userId)
            .Include(b => b.Picks)
            .Select(b => new BracketSummary 
            { 
                Id = b.Id, 
                Name = b.Name,
                Score = b.CurrentScore 
            })
            .ToListAsync();
            
        return Ok(brackets);
    }
}

// ❌ AVOID: Unnecessary repository wrapper
public interface IBracketRepository
{
    Task<List<Bracket>> GetUserBracketsAsync(string userId);
}
```

### Database Context Management
- Use dependency injection for DbContext lifecycle management
- Prefer scoped lifetime for web applications
- Use `AsNoTracking()` for read-only queries
- Implement proper async/await patterns for all database operations

## Service Layer Standards

### When to Create Services
- **Business Logic**: Complex business rules that span multiple entities
- **External Integrations**: Third-party API calls, file processing
- **Cross-Cutting Concerns**: Logging, caching, notifications
- **Reusable Operations**: Logic used across multiple controllers

### When NOT to Create Services
- **Simple CRUD Operations**: Direct DbContext usage is sufficient
- **Single-Entity Operations**: Basic create, read, update, delete
- **Framework Features**: Don't wrap what the framework already provides well

## Testing Standards

### Project Structure
- **One-to-One Mapping**: Each code project should have a corresponding unit test project
  - `ProjectName` → `ProjectName.Tests`
  - Example: `NflPlayoffPool.Web` → `NflPlayoffPool.Web.Tests`
- **Integration Tests**: Separate project for integration tests
  - Example: `NflPlayoffPool.IntegrationTests`
- **Shared Test Infrastructure**: Common setup in separate shared project
  - Example: `NflPlayoffPool.TestCommon`
  - Contains test utilities, builders, and shared setup code

### Unit Testing Approach
- Test business logic, not framework features
- Use in-memory databases for integration tests when appropriate
- Mock external dependencies, not internal framework components
- Focus on behavior verification over implementation details
- **Target Fat Methods**: Prioritize testing methods with complex business logic
- **Test Framework**: Use MSTest for consistency across all test projects

### DbContext Testing
```csharp
// ✅ GOOD: Test with real DbContext using in-memory provider
[TestMethod]
public async Task GetUserBrackets_ReturnsCorrectBrackets()
{
    // Arrange
    using var context = CreateInMemoryContext();
    var controller = new BracketController(context);
    
    // Act & Assert
    var result = await controller.GetUserBrackets("user123");
    
    // Verify behavior
}

// ❌ AVOID: Mocking DbContext
[TestMethod]
public void GetUserBrackets_MockedDbContext()
{
    // Don't mock what you don't own
    var mockContext = new Mock<PlayoffPoolContext>();
    // This becomes complex and brittle
}
```

### Test Framework Standards
- **MSTest**: Preferred test framework for all projects
  - Use `[TestClass]` for test classes
  - Use `[TestMethod]` for test methods
  - Use `[TestInitialize]` and `[TestCleanup]` for setup/teardown
- **FluentAssertions**: Use for readable, expressive assertions
- **Avoid NUnit or xUnit**: Maintain consistency with MSTest across all projects

## Code Organization Standards

### Project Structure
- Keep controllers focused on HTTP concerns
- Extract complex business logic to services when it adds value
- Use extension methods for model conversions and utilities
- Organize by feature when projects grow large

### Naming Conventions
- Use descriptive, intention-revealing names
- Prefer clarity over brevity
- Follow C# naming conventions consistently
- Use domain language in business logic

## Future Standards (To Be Added)
- Authentication and Authorization patterns
- API design standards
- Error handling approaches
- Logging and monitoring practices
- Performance optimization guidelines
- Security best practices

## Testing Package Standards
When creating new test projects, use these package references:
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="MSTest.TestFramework" Version="3.6.4" />
<PackageReference Include="MSTest.TestAdapter" Version="3.6.4" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="coverlet.collector" Version="6.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

## Notes
This document will evolve as new patterns and preferences are established. Each standard should include rationale and examples to ensure consistent application across projects.

---

**Last Updated**: 2025-12-20  
**Version**: 1.0