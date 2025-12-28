# NflPlayoffPool.TestCommon

Shared test infrastructure for the NFL Playoff Pool project, providing common test utilities and builders.

## Purpose

This project contains shared test infrastructure used across multiple test projects:
- `NflPlayoffPool.Web.Tests`
- `NflPlayoffPool.Data.Tests`
- Future integration test projects

## Test Builders

### Builder Pattern
All builders follow a fluent API pattern for creating test data with sensible defaults and easy customization.

### Available Builders

#### SeasonBuilder
Creates `Season` test data with configurable properties:
```csharp
var season = new SeasonBuilder()
    .WithYear(2024)
    .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
    .WithCurrentRound(PlayoffRound.Wildcard)
    .WithMasterBracket(masterBracket)
    .Build();
```

#### BracketBuilder
Creates `Bracket` test data with picks:
```csharp
var bracket = new BracketBuilder()
    .WithUserId("user123")
    .WithName("Test Bracket")
    .WithSeasonYear(2024)
    .WithPicks(pick1, pick2, pick3)
    .Build();
```

#### BracketPickBuilder
Creates individual `BracketPick` test data:
```csharp
var pick = new BracketPickBuilder()
    .ForConference("AFC")
    .ForRound(1, 1) // Round 1, 1 point
    .ForGame(1)
    .WithWinner("team1", "Team 1")
    .Build();
```

#### MasterBracketBuilder
Creates `MasterBracket` test data with playoff results:
```csharp
var masterBracket = new MasterBracketBuilder()
    .WithUserId("admin")
    .WithName("Master Bracket")
    .WithPicks(resultPick1, resultPick2)
    .Build();
```

## Design Principles

- **Sensible Defaults**: All builders provide reasonable default values
- **Fluent API**: Method chaining for easy customization
- **Immutable**: Each builder method returns a new instance
- **Domain Language**: Method names reflect business concepts

## Usage Examples

### Simple Test Data
```csharp
// Minimal setup with defaults
var season = new SeasonBuilder().Build();
var bracket = new BracketBuilder().Build();
```

### Complex Test Scenarios
```csharp
// Realistic playoff scenario
var season = new SeasonBuilder()
    .WithPointValues(1, 2, 3, 5)
    .Build();

var userBracket = new BracketBuilder()
    .WithPicks(
        new BracketPickBuilder().ForRound(1, 1).WithWinner("team1", "Team 1").Build(),
        new BracketPickBuilder().ForRound(2, 2).WithWinner("team1", "Team 1").Build()
    )
    .Build();

var masterBracket = new MasterBracketBuilder()
    .WithPicks(
        new BracketPickBuilder().ForRound(1, 1).WithWinner("team1", "Team 1").Build() // Correct
    )
    .Build();
```

## Dependencies

- **Microsoft.VisualStudio.TestTools.UnitTesting**: For test attributes and base functionality
- **FluentAssertions**: For readable assertions
- **NflPlayoffPool.Data**: For domain models

## Testing Philosophy

This shared infrastructure supports the project's testing approach:
- Focus on business logic testing
- Avoid unnecessary mocking
- Use real domain objects
- Prioritize readability and maintainability