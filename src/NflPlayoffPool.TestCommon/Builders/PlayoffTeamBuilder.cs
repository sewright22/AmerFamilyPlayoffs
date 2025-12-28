using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating PlayoffTeam test data with fluent API
/// </summary>
public class PlayoffTeamBuilder
{
    private PlayoffTeam _team;

    public PlayoffTeamBuilder()
    {
        _team = new PlayoffTeam
        {
            Id = Guid.NewGuid().ToString(),
            Code = "TM",
            Name = "Test Team",
            City = "Test City",
            Conference = Conference.AFC,
            Division = "Test Division",
            Seed = 1
        };
    }

    public PlayoffTeamBuilder WithId(string id)
    {
        _team.Id = id;
        return this;
    }

    public PlayoffTeamBuilder WithCode(string code)
    {
        _team.Code = code;
        return this;
    }

    public PlayoffTeamBuilder WithName(string name)
    {
        _team.Name = name;
        return this;
    }

    public PlayoffTeamBuilder WithCity(string city)
    {
        _team.City = city;
        return this;
    }

    public PlayoffTeamBuilder WithConference(Conference conference)
    {
        _team.Conference = conference;
        return this;
    }

    public PlayoffTeamBuilder WithDivision(string division)
    {
        _team.Division = division;
        return this;
    }

    public PlayoffTeamBuilder WithSeed(int seed)
    {
        _team.Seed = seed;
        return this;
    }

    public PlayoffTeam Build() => _team;
}