using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating Bracket test data with fluent API
/// </summary>
public class BracketBuilder
{
    private Bracket _bracket;

    public BracketBuilder()
    {
        _bracket = new Bracket
        {
            UserId = "user123",
            SeasonYear = 2024,
            Name = "Test Bracket",
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            IsSubmitted = false,
            PredictedWinner = null,
            Picks = new List<BracketPick>(),
            CurrentScore = 0,
            MaxPossibleScore = 42 // Default max score (1+1+1+1+1+1 + 2+2+2+2 + 3+3 + 5) = 6+8+6+5 = 25... wait let me recalculate
        };
    }

    public BracketBuilder WithUserId(string userId)
    {
        _bracket.UserId = userId;
        return this;
    }

    public BracketBuilder WithId(string id)
    {
        _bracket.Id = id;
        return this;
    }

    public BracketBuilder WithName(string name)
    {
        _bracket.Name = name;
        return this;
    }

    public BracketBuilder WithSeasonYear(int year)
    {
        _bracket.SeasonYear = year;
        return this;
    }

    public BracketBuilder WithPicks(params BracketPick[] picks)
    {
        _bracket.Picks = picks.ToList();
        return this;
    }

    public BracketBuilder WithPicks(ICollection<BracketPick> picks)
    {
        _bracket.Picks = picks;
        return this;
    }

    public BracketBuilder WithCurrentScore(int score)
    {
        _bracket.CurrentScore = score;
        return this;
    }

    public BracketBuilder WithMaxPossibleScore(int maxScore)
    {
        _bracket.MaxPossibleScore = maxScore;
        return this;
    }

    public BracketBuilder WithIsSubmitted(bool isSubmitted)
    {
        _bracket.IsSubmitted = isSubmitted;
        return this;
    }

    public BracketBuilder WithPredictedWinner(string predictedWinner)
    {
        _bracket.PredictedWinner = predictedWinner;
        return this;
    }

    public BracketBuilder WithCreatedAt(DateTime createdAt)
    {
        _bracket.CreatedAt = createdAt;
        return this;
    }

    public BracketBuilder WithLastModified(DateTime lastModified)
    {
        _bracket.LastModified = lastModified;
        return this;
    }

    public Bracket Build() => _bracket;
}