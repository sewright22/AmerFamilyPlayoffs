using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating BracketModel test data with fluent API
/// </summary>
public class BracketModelBuilder
{
    private BracketModel _bracket;

    public BracketModelBuilder()
    {
        _bracket = new BracketModel
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Bracket",
            UserId = "test-user-id",
            CanEdit = true,
            AfcRounds = new List<RoundModel>(),
            NfcRounds = new List<RoundModel>(),
            SuperBowl = null
        };
    }

    public BracketModelBuilder WithId(string id)
    {
        _bracket.Id = id;
        return this;
    }

    public BracketModelBuilder WithName(string name)
    {
        _bracket.Name = name;
        return this;
    }

    public BracketModelBuilder WithUserId(string userId)
    {
        _bracket.UserId = userId;
        return this;
    }

    public BracketModelBuilder WithCanEdit(bool canEdit)
    {
        _bracket.CanEdit = canEdit;
        return this;
    }

    public BracketModelBuilder WithAfcRounds(params RoundModel[] rounds)
    {
        _bracket.AfcRounds = rounds.ToList();
        return this;
    }

    public BracketModelBuilder WithAfcRound(RoundModel round)
    {
        _bracket.AfcRounds.Add(round);
        return this;
    }

    public BracketModelBuilder WithNfcRounds(params RoundModel[] rounds)
    {
        _bracket.NfcRounds = rounds.ToList();
        return this;
    }

    public BracketModelBuilder WithNfcRound(RoundModel round)
    {
        _bracket.NfcRounds.Add(round);
        return this;
    }

    public BracketModelBuilder WithSuperBowl(MatchupModel superBowl)
    {
        _bracket.SuperBowl = superBowl;
        return this;
    }

    public BracketModelBuilder WithCompleteSuperBowl(string winnerId)
    {
        _bracket.SuperBowl = new MatchupModelBuilder()
            .WithGameNumber(13)
            .WithName("Super Bowl")
            .WithSelectedWinner(winnerId)
            .WithHomeTeam(new PlayoffTeamModelBuilder().WithId("afc-1").WithName("AFC Team 1").Build())
            .WithAwayTeam(new PlayoffTeamModelBuilder().WithId("nfc-1").WithName("NFC Team 1").Build())
            .Build();
        return this;
    }

    public BracketModelBuilder WithAllPicks()
    {
        // Create AFC Wildcard Round (3 games)
        var afcWildcard = new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(1).WithSelectedWinner("afc-2").Build(),
                new MatchupModelBuilder().WithGameNumber(2).WithSelectedWinner("afc-3").Build(),
                new MatchupModelBuilder().WithGameNumber(3).WithSelectedWinner("afc-4").Build()
            )
            .Build();

        // Create AFC Divisional Round (2 games)
        var afcDivisional = new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Divisional")
            .WithRoundNumber(2)
            .WithPointValue(2)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(7).WithSelectedWinner("afc-1").Build(),
                new MatchupModelBuilder().WithGameNumber(8).WithSelectedWinner("afc-2").Build()
            )
            .Build();

        // Create AFC Conference Round (1 game)
        var afcConference = new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Conference")
            .WithRoundNumber(3)
            .WithPointValue(3)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(11).WithSelectedWinner("afc-1").Build()
            )
            .Build();

        // Create NFC Wildcard Round (3 games)
        var nfcWildcard = new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(4).WithSelectedWinner("nfc-2").Build(),
                new MatchupModelBuilder().WithGameNumber(5).WithSelectedWinner("nfc-3").Build(),
                new MatchupModelBuilder().WithGameNumber(6).WithSelectedWinner("nfc-4").Build()
            )
            .Build();

        // Create NFC Divisional Round (2 games)
        var nfcDivisional = new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Divisional")
            .WithRoundNumber(2)
            .WithPointValue(2)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(9).WithSelectedWinner("nfc-1").Build(),
                new MatchupModelBuilder().WithGameNumber(10).WithSelectedWinner("nfc-2").Build()
            )
            .Build();

        // Create NFC Conference Round (1 game)
        var nfcConference = new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Conference")
            .WithRoundNumber(3)
            .WithPointValue(3)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(12).WithSelectedWinner("nfc-1").Build()
            )
            .Build();

        // Create Super Bowl (1 game)
        var superBowl = new MatchupModelBuilder()
            .WithGameNumber(13)
            .WithName("Super Bowl")
            .WithSelectedWinner("afc-1")
            .Build();

        _bracket.AfcRounds = new List<RoundModel> { afcWildcard, afcDivisional, afcConference };
        _bracket.NfcRounds = new List<RoundModel> { nfcWildcard, nfcDivisional, nfcConference };
        _bracket.SuperBowl = superBowl;

        return this;
    }

    public BracketModel Build() => _bracket;
}