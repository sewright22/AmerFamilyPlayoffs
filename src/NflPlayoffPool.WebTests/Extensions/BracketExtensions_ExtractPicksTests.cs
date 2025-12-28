using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Extensions;
using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for BracketExtensions.ExtractPicks() method
/// This method converts UI bracket model to database picks
/// </summary>
[TestClass]
public class BracketExtensions_ExtractPicksTests
{
    [TestMethod]
    public void ExtractPicks_EmptyBracket_ReturnsEmptyList()
    {
        // Arrange
        var bracket = new BracketModelBuilder().Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().NotBeNull();
        picks.Should().BeEmpty("bracket has no rounds or games");
    }

    [TestMethod]
    public void ExtractPicks_AfcWildcardRoundOnly_ExtractsThreePicks()
    {
        // Arrange
        var homeTeam1 = new PlayoffTeamModelBuilder().WithId("team1").WithSeed(2).WithName("AFC Team 2").Build();
        var awayTeam1 = new PlayoffTeamModelBuilder().WithId("team2").WithSeed(7).WithName("AFC Team 7").Build();
        
        var homeTeam2 = new PlayoffTeamModelBuilder().WithId("team3").WithSeed(3).WithName("AFC Team 3").Build();
        var awayTeam2 = new PlayoffTeamModelBuilder().WithId("team4").WithSeed(6).WithName("AFC Team 6").Build();
        
        var homeTeam3 = new PlayoffTeamModelBuilder().WithId("team5").WithSeed(4).WithName("AFC Team 4").Build();
        var awayTeam3 = new PlayoffTeamModelBuilder().WithId("team6").WithSeed(5).WithName("AFC Team 5").Build();

        var wildcardRound = new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(1).WithHomeTeam(homeTeam1).WithAwayTeam(awayTeam1).WithHomeTeamWinner().Build(),
                new MatchupModelBuilder().WithGameNumber(2).WithHomeTeam(homeTeam2).WithAwayTeam(awayTeam2).WithAwayTeamWinner().Build(),
                new MatchupModelBuilder().WithGameNumber(3).WithHomeTeam(homeTeam3).WithAwayTeam(awayTeam3).WithHomeTeamWinner().Build()
            )
            .Build();

        var bracket = new BracketModelBuilder()
            .WithAfcRound(wildcardRound)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(3, "wildcard round has 3 games");
        
        // Verify first pick
        picks[0].Conference.Should().Be("AFC");
        picks[0].RoundNumber.Should().Be(1);
        picks[0].PointValue.Should().Be(1);
        picks[0].GameNumber.Should().Be(1);
        picks[0].PredictedWinningId.Should().Be("team1");
        picks[0].PredictedWinningTeam.Should().Be("AFC Team 2");

        // Verify second pick (away team wins)
        picks[1].PredictedWinningId.Should().Be("team4");
        picks[1].PredictedWinningTeam.Should().Be("AFC Team 6");

        // Verify third pick
        picks[2].PredictedWinningId.Should().Be("team5");
        picks[2].PredictedWinningTeam.Should().Be("AFC Team 4");
    }

    [TestMethod]
    public void ExtractPicks_NfcWildcardRoundOnly_ExtractsThreePicks()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("nfc1").WithSeed(2).WithName("NFC Team 2").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("nfc2").WithSeed(7).WithName("NFC Team 7").Build();

        var wildcardRound = new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGame(new MatchupModelBuilder().WithGameNumber(4).WithHomeTeam(homeTeam).WithAwayTeam(awayTeam).WithHomeTeamWinner().Build())
            .Build();

        var bracket = new BracketModelBuilder()
            .WithNfcRound(wildcardRound)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(1);
        picks[0].Conference.Should().Be("NFC");
        picks[0].RoundNumber.Should().Be(1);
        picks[0].GameNumber.Should().Be(4);
    }

    [TestMethod]
    public void ExtractPicks_CompleteBracket_ExtractsAllThirteenPicks()
    {
        // Arrange - Create a complete bracket with all rounds
        var bracket = CreateCompleteBracket();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(13, "complete bracket has 13 games total");
        
        // Verify AFC picks: 3 wildcard + 2 divisional + 1 conference = 6
        picks.Count(p => p.Conference == "AFC").Should().Be(6);
        
        // Verify NFC picks: 3 wildcard + 2 divisional + 1 conference = 6
        picks.Count(p => p.Conference == "NFC").Should().Be(6);
        
        // Verify Super Bowl pick: 1
        picks.Count(p => p.Conference == "Super Bowl").Should().Be(1);
    }

    [TestMethod]
    public void ExtractPicks_SuperBowlOnly_ExtractsOnePick()
    {
        // Arrange
        var afcTeam = new PlayoffTeamModelBuilder().WithId("afc1").WithSeed(1).WithName("AFC Champion").Build();
        var nfcTeam = new PlayoffTeamModelBuilder().WithId("nfc1").WithSeed(1).WithName("NFC Champion").Build();

        var superBowl = new MatchupModelBuilder()
            .WithGameNumber(13)
            .WithName("Super Bowl")
            .WithHomeTeam(afcTeam)
            .WithAwayTeam(nfcTeam)
            .WithHomeTeamWinner()
            .Build();

        var bracket = new BracketModelBuilder()
            .WithSuperBowl(superBowl)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(1);
        picks[0].Conference.Should().Be("Super Bowl");
        picks[0].RoundNumber.Should().Be(4);
        picks[0].PointValue.Should().Be(5);
        picks[0].GameNumber.Should().Be(13);
        picks[0].PredictedWinningId.Should().Be("afc1");
        picks[0].PredictedWinningTeam.Should().Be("AFC Champion");
    }

    [TestMethod]
    public void ExtractPicks_SuperBowlWithNoWinner_DoesNotExtractSuperBowlPick()
    {
        // Arrange
        var afcTeam = new PlayoffTeamModelBuilder().WithId("afc1").WithSeed(1).WithName("AFC Champion").Build();
        var nfcTeam = new PlayoffTeamModelBuilder().WithId("nfc1").WithSeed(1).WithName("NFC Champion").Build();

        var superBowl = new MatchupModelBuilder()
            .WithGameNumber(13)
            .WithName("Super Bowl")
            .WithHomeTeam(afcTeam)
            .WithAwayTeam(nfcTeam)
            .WithSelectedWinner(null) // No winner selected
            .Build();

        var bracket = new BracketModelBuilder()
            .WithSuperBowl(superBowl)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().BeEmpty("Super Bowl has no winner selected");
    }

    [TestMethod]
    public void ExtractPicks_PartialBracket_ExtractsOnlyCompletedPicks()
    {
        // Arrange - Wildcard round with some picks, divisional with no picks
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").WithSeed(2).Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").WithSeed(7).Build();

        var wildcardRound = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                new MatchupModelBuilder().WithGameNumber(1).WithHomeTeam(homeTeam).WithAwayTeam(awayTeam).WithHomeTeamWinner().Build(),
                new MatchupModelBuilder().WithGameNumber(2).WithHomeTeam(homeTeam).WithAwayTeam(awayTeam).WithSelectedWinner(null).Build() // No pick
            )
            .Build();

        var bracket = new BracketModelBuilder()
            .WithAfcRound(wildcardRound)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(2, "both games are extracted even if one has no winner");
        picks[0].PredictedWinningId.Should().NotBeNull();
        picks[1].PredictedWinningId.Should().BeNull("second game has no winner selected");
    }

    [TestMethod]
    public void ExtractPicks_MultipleRounds_ExtractsInCorrectOrder()
    {
        // Arrange
        var wildcardRound = CreateAfcWildcardRound();
        var divisionalRound = CreateAfcDivisionalRound();

        var bracket = new BracketModelBuilder()
            .WithAfcRound(wildcardRound)
            .WithAfcRound(divisionalRound)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks.Should().HaveCount(5, "3 wildcard + 2 divisional");
        
        // Verify order: wildcard games first, then divisional
        picks[0].RoundNumber.Should().Be(1);
        picks[1].RoundNumber.Should().Be(1);
        picks[2].RoundNumber.Should().Be(1);
        picks[3].RoundNumber.Should().Be(2);
        picks[4].RoundNumber.Should().Be(2);
    }

    [TestMethod]
    public void ExtractPicks_PointValuesPreserved_FromRounds()
    {
        // Arrange
        var wildcardRound = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGame(CreateSimpleMatchup(1))
            .Build();

        var divisionalRound = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(2)
            .WithPointValue(2)
            .WithGame(CreateSimpleMatchup(7))
            .Build();

        var conferenceRound = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(3)
            .WithPointValue(3)
            .WithGame(CreateSimpleMatchup(11))
            .Build();

        var bracket = new BracketModelBuilder()
            .WithAfcRound(wildcardRound)
            .WithAfcRound(divisionalRound)
            .WithAfcRound(conferenceRound)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks[0].PointValue.Should().Be(1, "wildcard round");
        picks[1].PointValue.Should().Be(2, "divisional round");
        picks[2].PointValue.Should().Be(3, "conference round");
    }

    [TestMethod]
    public void ExtractPicks_GameNumbersPreserved_FromMatchups()
    {
        // Arrange
        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGames(
                CreateSimpleMatchup(1),
                CreateSimpleMatchup(2),
                CreateSimpleMatchup(3)
            )
            .Build();

        var bracket = new BracketModelBuilder()
            .WithAfcRound(round)
            .Build();

        // Act
        var picks = bracket.ExtractPicks();

        // Assert
        picks[0].GameNumber.Should().Be(1);
        picks[1].GameNumber.Should().Be(2);
        picks[2].GameNumber.Should().Be(3);
    }

    // Helper methods
    private BracketModel CreateCompleteBracket()
    {
        return new BracketModelBuilder()
            .WithAfcRound(CreateAfcWildcardRound())
            .WithAfcRound(CreateAfcDivisionalRound())
            .WithAfcRound(CreateAfcConferenceRound())
            .WithNfcRound(CreateNfcWildcardRound())
            .WithNfcRound(CreateNfcDivisionalRound())
            .WithNfcRound(CreateNfcConferenceRound())
            .WithSuperBowl(CreateSuperBowl())
            .Build();
    }

    private RoundModel CreateAfcWildcardRound()
    {
        return new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                CreateSimpleMatchup(1),
                CreateSimpleMatchup(2),
                CreateSimpleMatchup(3)
            )
            .Build();
    }

    private RoundModel CreateAfcDivisionalRound()
    {
        return new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Divisional")
            .WithRoundNumber(2)
            .WithPointValue(2)
            .WithGames(
                CreateSimpleMatchup(7),
                CreateSimpleMatchup(8)
            )
            .Build();
    }

    private RoundModel CreateAfcConferenceRound()
    {
        return new RoundModelBuilder()
            .WithConference("AFC")
            .WithName("Conference")
            .WithRoundNumber(3)
            .WithPointValue(3)
            .WithGame(CreateSimpleMatchup(11))
            .Build();
    }

    private RoundModel CreateNfcWildcardRound()
    {
        return new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Wildcard")
            .WithRoundNumber(1)
            .WithPointValue(1)
            .WithGames(
                CreateSimpleMatchup(4),
                CreateSimpleMatchup(5),
                CreateSimpleMatchup(6)
            )
            .Build();
    }

    private RoundModel CreateNfcDivisionalRound()
    {
        return new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Divisional")
            .WithRoundNumber(2)
            .WithPointValue(2)
            .WithGames(
                CreateSimpleMatchup(9),
                CreateSimpleMatchup(10)
            )
            .Build();
    }

    private RoundModel CreateNfcConferenceRound()
    {
        return new RoundModelBuilder()
            .WithConference("NFC")
            .WithName("Conference")
            .WithRoundNumber(3)
            .WithPointValue(3)
            .WithGame(CreateSimpleMatchup(12))
            .Build();
    }

    private MatchupModel CreateSuperBowl()
    {
        var afcTeam = new PlayoffTeamModelBuilder().WithId("afc-champ").WithSeed(1).WithName("AFC Champion").Build();
        var nfcTeam = new PlayoffTeamModelBuilder().WithId("nfc-champ").WithSeed(1).WithName("NFC Champion").Build();

        return new MatchupModelBuilder()
            .WithGameNumber(13)
            .WithName("Super Bowl")
            .WithHomeTeam(afcTeam)
            .WithAwayTeam(nfcTeam)
            .WithHomeTeamWinner()
            .Build();
    }

    private MatchupModel CreateSimpleMatchup(int gameNumber)
    {
        var homeTeam = new PlayoffTeamModelBuilder()
            .WithId($"home-{gameNumber}")
            .WithSeed(1)
            .WithName($"Home Team {gameNumber}")
            .Build();
        
        var awayTeam = new PlayoffTeamModelBuilder()
            .WithId($"away-{gameNumber}")
            .WithSeed(2)
            .WithName($"Away Team {gameNumber}")
            .Build();

        return new MatchupModelBuilder()
            .WithGameNumber(gameNumber)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .WithHomeTeamWinner()
            .Build();
    }
}
