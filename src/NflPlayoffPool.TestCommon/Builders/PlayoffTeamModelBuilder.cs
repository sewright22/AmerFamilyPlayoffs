using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating PlayoffTeamModel test data with fluent API
/// </summary>
public class PlayoffTeamModelBuilder
{
    private PlayoffTeamModel _team;

    public PlayoffTeamModelBuilder()
    {
        _team = new PlayoffTeamModel
        {
            Id = Guid.NewGuid().ToString(),
            Seed = 1,
            ViewId = Guid.NewGuid().ToString(),
            Name = "Test Team",
            Selected = false
        };
    }

    public PlayoffTeamModelBuilder WithId(string id)
    {
        _team.Id = id;
        return this;
    }

    public PlayoffTeamModelBuilder WithSeed(int seed)
    {
        _team.Seed = seed;
        return this;
    }

    public PlayoffTeamModelBuilder WithName(string name)
    {
        _team.Name = name;
        return this;
    }

    public PlayoffTeamModelBuilder WithViewId(string viewId)
    {
        _team.ViewId = viewId;
        return this;
    }

    public PlayoffTeamModelBuilder AsSelected()
    {
        _team.Selected = true;
        return this;
    }

    public PlayoffTeamModel Build() => _team;
}