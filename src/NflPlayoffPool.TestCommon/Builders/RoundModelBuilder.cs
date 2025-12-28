using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating RoundModel test data with fluent API
/// </summary>
public class RoundModelBuilder
{
    private RoundModel _round;

    public RoundModelBuilder()
    {
        _round = new RoundModel
        {
            Conference = "AFC",
            Name = "Wildcard",
            PointValue = 1,
            RoundNumber = 1,
            IsLocked = false,
            Games = new List<MatchupModel>()
        };
    }

    public RoundModelBuilder WithConference(string conference)
    {
        _round.Conference = conference;
        return this;
    }

    public RoundModelBuilder WithName(string name)
    {
        _round.Name = name;
        return this;
    }

    public RoundModelBuilder WithPointValue(int pointValue)
    {
        _round.PointValue = pointValue;
        return this;
    }

    public RoundModelBuilder WithRoundNumber(int roundNumber)
    {
        _round.RoundNumber = roundNumber;
        return this;
    }

    public RoundModelBuilder WithGame(MatchupModel game)
    {
        _round.Games.Add(game);
        return this;
    }

    public RoundModelBuilder WithGames(params MatchupModel[] games)
    {
        _round.Games.AddRange(games);
        return this;
    }

    public RoundModelBuilder AsLocked()
    {
        _round.IsLocked = true;
        return this;
    }

    public RoundModel Build() => _round;
}