# NflPlayoffPool.Web.Tests

Unit tests for the NflPlayoffPool.Web project, focusing on business logic and complex methods.

## Test Structure

This project follows the one-to-one mapping standard where each code project has a corresponding test project:
- `NflPlayoffPool.Web` → `NflPlayoffPool.Web.Tests`

## Test Coverage

### BracketExtensionsTests
Tests for the `BracketExtensions` class, specifically targeting the complex `CalculateScores` method which handles:

1. **Score Calculation**: Calculates current score based on correct picks across all playoff rounds
2. **Max Possible Score**: Tracks maximum achievable score as teams are eliminated
3. **Elimination Logic**: Reduces max possible score when a user's picked team is eliminated in earlier rounds
4. **Round-by-Round Processing**: Handles partial results when only some rounds have completed
5. **Point Values**: Different rounds have different point values (Wildcard: 1, Divisional: 2, Conference: 3, Super Bowl: 5)

#### Test Scenarios

- **Empty Master Bracket**: Verifies behavior when no playoff results exist yet
- **Correct Wildcard Picks**: Tests scoring for correct picks in the wildcard round
- **Eliminated Team Logic**: Tests the complex elimination tracking across multiple rounds
- **Complete Bracket**: Tests full scoring when all playoff rounds are complete
- **Partial Results**: Tests behavior when only some rounds have results
- **Mixed Results**: Tests realistic scenarios with both correct and incorrect picks

## Dependencies

- **MSTest**: Test framework (consistent with existing project tests)
- **FluentAssertions**: Fluent assertion library for readable test assertions
- **NflPlayoffPool.TestCommon**: Shared test infrastructure with builders

## Running Tests

Tests can be run using Visual Studio Test Explorer or via command line:
```bash
dotnet test
```

## Test Builders

This project uses the builder pattern from `NflPlayoffPool.TestCommon` to create test data:
- `SeasonBuilder`: Creates Season test data with configurable point values
- `BracketBuilder`: Creates Bracket test data with picks
- `BracketPickBuilder`: Creates individual BracketPick test data
- `MasterBracketBuilder`: Creates MasterBracket test data with results

## Testing Philosophy

Following the project's pragmatic approach:
- Focus on testing complex business logic (fat methods)
- Avoid testing framework features
- Use real objects instead of mocks where possible
- Prioritize behavior verification over implementation details
