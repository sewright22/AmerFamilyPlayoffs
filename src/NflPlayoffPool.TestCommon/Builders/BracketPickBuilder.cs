using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating BracketPick test data with fluent API
/// </summary>
public class BracketPickBuilder
{
    private BracketPick _pick;

    public BracketPickBuilder()
    {
        _pick = new BracketPick
        {
            Conference = "AFC",
            RoundNumber = 1,
            PointValue = 1,
            GameNumber = 1,
            PredictedWinningId = "team1",
            PredictedWinningTeam = "Team 1",
            PointsEarned = null
        };
    }

    public BracketPickBuilder ForConference(string conference)
    {
        _pick.Conference = conference;
        return this;
    }

    public BracketPickBuilder ForRound(int roundNumber, int pointValue)
    {
        _pick.RoundNumber = roundNumber;
        _pick.PointValue = pointValue;
        return this;
    }

    public BracketPickBuilder ForGame(int gameNumber)
    {
        _pick.GameNumber = gameNumber;
        return this;
    }

    public BracketPickBuilder WithWinner(string teamId, string teamName)
    {
        _pick.PredictedWinningId = teamId;
        _pick.PredictedWinningTeam = teamName;
        return this;
    }

    public BracketPickBuilder WithConference(string conference)
    {
        _pick.Conference = conference;
        return this;
    }

    public BracketPickBuilder WithRoundNumber(int roundNumber)
    {
        _pick.RoundNumber = roundNumber;
        return this;
    }

    public BracketPickBuilder WithGameNumber(int gameNumber)
    {
        _pick.GameNumber = gameNumber;
        return this;
    }

    public BracketPickBuilder WithPredictedWinningId(string? teamId)
    {
        _pick.PredictedWinningId = teamId;
        return this;
    }

    public BracketPickBuilder WithPointsEarned(int points)
    {
        _pick.PointsEarned = points;
        return this;
    }

    public BracketPick Build() => _pick;
}