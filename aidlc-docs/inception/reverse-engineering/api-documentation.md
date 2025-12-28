# API Documentation

## REST APIs

### Bracket Management APIs

#### GET /Bracket/Create
- **Method**: GET
- **Purpose**: Display bracket creation form
- **Authorization**: Required (Authenticated users only)
- **Request**: None
- **Response**: HTML form for creating new bracket

#### POST /Bracket/Create
- **Method**: POST
- **Purpose**: Create a new bracket with user predictions
- **Authorization**: Required (Authenticated users only)
- **Request**: BracketModel with user picks
- **Response**: Redirect to bracket update page or validation errors

#### GET /Bracket/Update/{id}
- **Method**: GET
- **Purpose**: Display bracket editing form with current predictions
- **Authorization**: Required (Owner or admin only)
- **Request**: Bracket ID in URL path
- **Response**: HTML form with populated bracket data

#### POST /Bracket/Update/{id}
- **Method**: POST
- **Purpose**: Update existing bracket with new predictions
- **Authorization**: Required (Owner or admin only)
- **Request**: Updated BracketModel
- **Response**: Redirect based on completion status

#### GET /Bracket/Reset/{id}
- **Method**: GET
- **Purpose**: Reset bracket to empty state (clear all picks)
- **Authorization**: Required (Owner or admin only)
- **Request**: Bracket ID in URL path
- **Response**: Redirect to bracket update page

### Home/Dashboard APIs

#### GET /Home/Index
- **Method**: GET
- **Purpose**: Display user dashboard with brackets and leaderboard
- **Authorization**: Required (Authenticated users only)
- **Request**: None
- **Response**: Dashboard with user's brackets and current leaderboard

#### GET /Home/LogOut
- **Method**: GET
- **Purpose**: Sign out current user
- **Authorization**: Required
- **Request**: None
- **Response**: Redirect to home page after clearing authentication

### Authentication APIs

#### GET /Account/Login
- **Method**: GET
- **Purpose**: Display login form
- **Authorization**: None
- **Request**: None
- **Response**: Login form HTML

#### POST /Account/Login
- **Method**: POST
- **Purpose**: Authenticate user credentials
- **Authorization**: None
- **Request**: Login credentials
- **Response**: Redirect to dashboard or validation errors

## Internal APIs

### PlayoffPoolContext
- **Methods**: 
  - `GetCurrentSeason()`: Retrieve active playoff season
  - `SaveChanges()`: Persist changes to database
  - `Add<T>(entity)`: Add new entity to context
- **Parameters**: Entity objects, query expressions
- **Return Types**: Domain entities, change tracking results

### BracketController Internal Methods
- **Methods**:
  - `SaveBracket(BracketModel, IEnumerable<PlayoffTeam>, IEnumerable<PlayoffTeam>)`: Persist bracket data
  - `BuildWildcardRound(Season, Bracket, BracketViewModel)`: Generate wild card round data
  - `BuildDivisionalRound(Season, Bracket, BracketViewModel)`: Generate divisional round data
  - `BuildConferenceRound(Season, Bracket, BracketViewModel)`: Generate conference championship data
  - `BuildSuperBowlRound(Season, Bracket, BracketViewModel)`: Generate Super Bowl data
- **Parameters**: Domain entities, view models
- **Return Types**: String IDs, void for builder methods

### HomeController Internal Methods
- **Methods**:
  - `BuildLeaderboard(Season)`: Calculate bracket rankings and scores
  - `BuildPlaceAsString(int, bool, bool)`: Format ranking display text
- **Parameters**: Season entity, ranking data
- **Return Types**: List of BracketSummaryModel, formatted strings

## Data Models

### Bracket
- **Fields**:
  - `Id`: Unique identifier (ObjectId)
  - `UserId`: Owner reference (ObjectId)
  - `SeasonYear`: Associated season (int)
  - `Name`: User-defined bracket name (string)
  - `CreatedAt`: Creation timestamp (DateTime)
  - `LastModified`: Last update timestamp (DateTime)
  - `IsSubmitted`: Completion status (bool)
  - `PredictedWinner`: Super Bowl winner prediction (string)
  - `Picks`: Collection of game predictions (ICollection<BracketPick>)
  - `CurrentScore`: Points earned so far (int)
  - `MaxPossibleScore`: Maximum achievable points (int)
- **Relationships**: Belongs to User, contains multiple BracketPicks
- **Validation**: Name required, picks must be valid team selections

### BracketPick
- **Fields**:
  - `Conference`: AFC or NFC (string)
  - `RoundNumber`: Playoff round 1-4 (int)
  - `PredictedWinningId`: Team identifier (string)
  - `PredictedWinningTeam`: Team name (string)
- **Relationships**: Belongs to Bracket
- **Validation**: Conference must be AFC/NFC, round 1-4, valid team selection

### Season
- **Fields**:
  - `Year`: Season year (int)
  - `Status`: Current season state (SeasonStatus enum)
  - `Teams`: Playoff teams (ICollection<PlayoffTeam>)
  - `Bracket`: Master results (MasterBracket)
  - `StartDate`: Playoff start (DateTime)
  - `EndDate`: Season end (DateTime)
- **Relationships**: Contains multiple PlayoffTeams, has one MasterBracket
- **Validation**: Year must be valid, teams must be properly seeded

### User
- **Fields**:
  - `Id`: Unique identifier (ObjectId)
  - `Username`: Login name (string)
  - `Email`: Email address (string)
  - `Role`: User permissions (Role enum)
  - `CreatedAt`: Registration date (DateTime)
- **Relationships**: Has multiple Brackets
- **Validation**: Username and email must be unique, valid email format