using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating Season test data with fluent API
/// </summary>
public class SeasonBuilder
{
    private Season _season;

    public SeasonBuilder()
    {
        _season = new Season
        {
            Year = 2024,
            IsCurrent = true,
            Status = SeasonStatus.InProgress,
            CurrentRound = PlayoffRound.Wildcard,
            SubmissionDeadline = DateTime.UtcNow.AddDays(7),
            WildcardPoints = 1,
            DivisionalPoints = 2,
            ConferencePoints = 3,
            SuperBowlPoints = 5,
            Teams = new List<PlayoffTeam>(),
            Bracket = null
        };
    }

    public SeasonBuilder WithYear(int year)
    {
        _season.Year = year;
        return this;
    }

    public SeasonBuilder WithPointValues(int wildcard, int divisional, int conference, int superBowl)
    {
        _season.WildcardPoints = wildcard;
        _season.DivisionalPoints = divisional;
        _season.ConferencePoints = conference;
        _season.SuperBowlPoints = superBowl;
        return this;
    }

    public SeasonBuilder WithCurrentRound(PlayoffRound round)
    {
        _season.CurrentRound = round;
        return this;
    }

    public SeasonBuilder WithMasterBracket(MasterBracket masterBracket)
    {
        _season.Bracket = masterBracket;
        return this;
    }

    public SeasonBuilder WithIsCurrent(bool isCurrent)
    {
        _season.IsCurrent = isCurrent;
        return this;
    }

    public SeasonBuilder WithStatus(SeasonStatus status)
    {
        _season.Status = status;
        return this;
    }

    public SeasonBuilder WithSubmissionDeadline(DateTime deadline)
    {
        _season.SubmissionDeadline = deadline;
        return this;
    }

    public Season Build() => _season;
}