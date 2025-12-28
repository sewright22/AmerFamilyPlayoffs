using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating MatchupModel test data with fluent API
/// </summary>
public class MatchupModelBuilder
{
    private MatchupModel _matchup;

    public MatchupModelBuilder()
    {
        _matchup = new MatchupModel
        {
            GameNumber = 1,
            Name = "Game 1",
            HomeTeam = new PlayoffTeamModelBuilder().WithSeed(1).WithName("Home Team").Build(),
            AwayTeam = new PlayoffTeamModelBuilder().WithSeed(2).WithName("Away Team").Build(),
            SelectedWinner = null,
            IsCorrect = null,
            IsLocked = false
        };
    }

    public MatchupModelBuilder WithGameNumber(int gameNumber)
    {
        _matchup.GameNumber = gameNumber;
        return this;
    }

    public MatchupModelBuilder WithName(string name)
    {
        _matchup.Name = name;
        return this;
    }

    public MatchupModelBuilder WithHomeTeam(PlayoffTeamModel homeTeam)
    {
        _matchup.HomeTeam = homeTeam;
        return this;
    }

    public MatchupModelBuilder WithAwayTeam(PlayoffTeamModel awayTeam)
    {
        _matchup.AwayTeam = awayTeam;
        return this;
    }

    public MatchupModelBuilder WithSelectedWinner(string? winnerId)
    {
        _matchup.SelectedWinner = winnerId;
        return this;
    }

    public MatchupModelBuilder WithHomeTeamWinner()
    {
        _matchup.SelectedWinner = _matchup.HomeTeam.Id;
        return this;
    }

    public MatchupModelBuilder WithAwayTeamWinner()
    {
        _matchup.SelectedWinner = _matchup.AwayTeam.Id;
        return this;
    }

    public MatchupModelBuilder AsCorrect()
    {
        _matchup.IsCorrect = true;
        return this;
    }

    public MatchupModelBuilder AsIncorrect()
    {
        _matchup.IsCorrect = false;
        return this;
    }

    public MatchupModelBuilder AsLocked()
    {
        _matchup.IsLocked = true;
        return this;
    }

    public MatchupModel Build() => _matchup;
}