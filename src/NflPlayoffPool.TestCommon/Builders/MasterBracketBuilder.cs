using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating MasterBracket test data with fluent API
/// </summary>
public class MasterBracketBuilder
{
    private MasterBracket _masterBracket;

    public MasterBracketBuilder()
    {
        _masterBracket = new MasterBracket
        {
            UserId = "admin",
            Name = "Master Bracket",
            SeasonYear = 2024,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            IsSubmitted = true,
            Picks = new List<BracketPick>()
        };
    }

    public MasterBracketBuilder WithUserId(string userId)
    {
        _masterBracket.UserId = userId;
        return this;
    }

    public MasterBracketBuilder WithName(string name)
    {
        _masterBracket.Name = name;
        return this;
    }

    public MasterBracketBuilder WithSeasonYear(int year)
    {
        _masterBracket.SeasonYear = year;
        return this;
    }

    public MasterBracketBuilder WithPicks(params BracketPick[] picks)
    {
        _masterBracket.Picks = picks.ToList();
        return this;
    }

    public MasterBracketBuilder WithPicks(ICollection<BracketPick> picks)
    {
        _masterBracket.Picks = picks;
        return this;
    }

    public MasterBracket Build() => _masterBracket;
}