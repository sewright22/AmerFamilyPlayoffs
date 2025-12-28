using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Extensions;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for SeasonExtensions, focusing on NFL playoff bracket generation logic
/// that ensures lowest seed plays highest seed after each round
/// </summary>
[TestClass]
public class SeasonExtensionsTests
{
    private Season CreateSeasonWithTeams()
    {
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        // Create AFC teams (seeds 1-7)
        for (int seed = 1; seed <= 7; seed++)
        {
            season.Teams.Add(new PlayoffTeamBuilder()
                .WithCode($"AFC{seed}")
                .WithName($"AFC Team {seed}")
                .WithConference(Conference.AFC)
                .WithSeed(seed)
                .Build());
        }

        // Create NFC teams (seeds 1-7)
        for (int seed = 1; seed <= 7; seed++)
        {
            season.Teams.Add(new PlayoffTeamBuilder()
                .WithCode($"NFC{seed}")
                .WithName($"NFC Team {seed}")
                .WithConference(Conference.NFC)
                .WithSeed(seed)
                .Build());
        }

        return season;
    }

    [TestMethod]
    public void GenerateAfcWildcardRound_CreatesCorrectMatchups()
    {
        // Arrange
        var season = CreateSeasonWithTeams();

        // Act
        var round = season.GenerateAfcWildcardRound();

        // Assert
        round.Should().NotBeNull();
        round.Conference.Should().Be("AFC");
        round.Name.Should().Be("Wildcard");
        round.RoundNumber.Should().Be(1);
        round.PointValue.Should().Be(1);
        round.Games.Should().HaveCount(3, "wildcard round has 3 games");

        // Verify matchups: 2v7, 3v6, 4v5
        round.Games[0].HomeTeam.Seed.Should().Be(2, "first game should be 2 seed at home");
        round.Games[0].AwayTeam.Seed.Should().Be(7, "first game should be 7 seed away");

        round.Games[1].HomeTeam.Seed.Should().Be(3, "second game should be 3 seed at home");
        round.Games[1].AwayTeam.Seed.Should().Be(6, "second game should be 6 seed away");

        round.Games[2].HomeTeam.Seed.Should().Be(4, "third game should be 4 seed at home");
        round.Games[2].AwayTeam.Seed.Should().Be(5, "third game should be 5 seed away");
    }

    [TestMethod]
    public void GenerateNfcWildcardRound_CreatesCorrectMatchups()
    {
        // Arrange
        var season = CreateSeasonWithTeams();

        // Act
        var round = season.GenerateNfcWildcardRound();

        // Assert
        round.Should().NotBeNull();
        round.Conference.Should().Be("NFC");
        round.Name.Should().Be("Wildcard");
        round.RoundNumber.Should().Be(1);
        round.PointValue.Should().Be(1);
        round.Games.Should().HaveCount(3, "wildcard round has 3 games");

        // Verify matchups: 2v7, 3v6, 4v5
        round.Games[0].HomeTeam.Seed.Should().Be(2);
        round.Games[0].AwayTeam.Seed.Should().Be(7);

        round.Games[1].HomeTeam.Seed.Should().Be(3);
        round.Games[1].AwayTeam.Seed.Should().Be(6);

        round.Games[2].HomeTeam.Seed.Should().Be(4);
        round.Games[2].AwayTeam.Seed.Should().Be(5);
    }

    [TestMethod]
    public void GenerateAfcDivisionalRound_LowestSeedPlaysOne_Scenario1()
    {
        // Arrange - Wildcard winners: 2, 3, 4 (all favorites win)
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateAfcWildcardRound();
        
        // Simulate wildcard results: 2, 3, 4 all win
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].HomeTeam.Id; // 2 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].HomeTeam.Id; // 3 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].HomeTeam.Id; // 4 wins

        // Act
        var divisionalRound = season.GenerateAfcDivisionalRound(wildcardRound);

        // Assert
        divisionalRound.Should().NotBeNull();
        divisionalRound.Conference.Should().Be("AFC");
        divisionalRound.Name.Should().Be("Divisional");
        divisionalRound.RoundNumber.Should().Be(2);
        divisionalRound.PointValue.Should().Be(2);
        divisionalRound.Games.Should().HaveCount(2, "divisional round has 2 games");

        // Verify NFL playoff rule: Lowest seed (4) plays highest seed (1)
        divisionalRound.Games[0].HomeTeam.Seed.Should().Be(1, "highest remaining seed (#1) should be home");
        divisionalRound.Games[0].AwayTeam.Seed.Should().Be(4, "lowest remaining seed (#4) should be away");

        // Other two seeds (2 and 3) play each other
        divisionalRound.Games[1].HomeTeam.Seed.Should().Be(2, "second lowest seed should be home");
        divisionalRound.Games[1].AwayTeam.Seed.Should().Be(3, "third lowest seed should be away");
    }

    [TestMethod]
    public void GenerateAfcDivisionalRound_LowestSeedPlaysOne_Scenario2()
    {
        // Arrange - Wildcard winners: 7, 6, 5 (all underdogs win)
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateAfcWildcardRound();
        
        // Simulate wildcard results: 7, 6, 5 all win (upsets)
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].AwayTeam.Id; // 7 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].AwayTeam.Id; // 6 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].AwayTeam.Id; // 5 wins

        // Act
        var divisionalRound = season.GenerateAfcDivisionalRound(wildcardRound);

        // Assert
        // Verify NFL playoff rule: Lowest seed (7) plays highest seed (1)
        divisionalRound.Games[0].HomeTeam.Seed.Should().Be(1, "highest remaining seed (#1) should be home");
        divisionalRound.Games[0].AwayTeam.Seed.Should().Be(7, "lowest remaining seed (#7) should be away");

        // Other two seeds (5 and 6) play each other
        // Algorithm: thirdLowestSeed=5, nextLowestSeed=6, so matchup is 5 (home) vs 6 (away)
        divisionalRound.Games[1].HomeTeam.Seed.Should().Be(5, "third lowest seed (5) should be home");
        divisionalRound.Games[1].AwayTeam.Seed.Should().Be(6, "second lowest seed (6) should be away");
    }

    [TestMethod]
    public void GenerateAfcDivisionalRound_LowestSeedPlaysOne_MixedScenario()
    {
        // Arrange - Wildcard winners: 2, 6, 5 (mixed results)
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateAfcWildcardRound();
        
        // Simulate wildcard results: 2 wins, 6 wins, 5 wins
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].HomeTeam.Id; // 2 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].AwayTeam.Id; // 6 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].AwayTeam.Id; // 5 wins

        // Act
        var divisionalRound = season.GenerateAfcDivisionalRound(wildcardRound);

        // Assert
        // Verify NFL playoff rule: Lowest seed (6) plays highest seed (1)
        divisionalRound.Games[0].HomeTeam.Seed.Should().Be(1, "highest remaining seed (#1) should be home");
        divisionalRound.Games[0].AwayTeam.Seed.Should().Be(6, "lowest remaining seed (#6) should be away");

        // Other two seeds (2 and 5) play each other
        // Algorithm: thirdLowestSeed=2, nextLowestSeed=5, so matchup is 2 (home) vs 5 (away)
        divisionalRound.Games[1].HomeTeam.Seed.Should().Be(2, "third lowest seed (2) should be home");
        divisionalRound.Games[1].AwayTeam.Seed.Should().Be(5, "second lowest seed (5) should be away");
    }

    [TestMethod]
    public void GenerateNfcDivisionalRound_LowestSeedPlaysOne()
    {
        // Arrange - Wildcard winners: 2, 3, 4
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateNfcWildcardRound();
        
        // Simulate wildcard results
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].HomeTeam.Id; // 2 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].HomeTeam.Id; // 3 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].HomeTeam.Id; // 4 wins

        // Act
        var divisionalRound = season.GenerateNfcDivisionalRound(wildcardRound);

        // Assert
        divisionalRound.Conference.Should().Be("NFC");
        divisionalRound.Games.Should().HaveCount(2);

        // Verify NFL playoff rule: Lowest seed (4) plays highest seed (1)
        divisionalRound.Games[0].HomeTeam.Seed.Should().Be(1);
        divisionalRound.Games[0].AwayTeam.Seed.Should().Be(4);

        divisionalRound.Games[1].HomeTeam.Seed.Should().Be(2);
        divisionalRound.Games[1].AwayTeam.Seed.Should().Be(3);
    }

    [TestMethod]
    public void GenerateAfcConferenceRound_LowestSeedPlaysHighestSeed()
    {
        // Arrange - Divisional winners: 1, 3
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateAfcWildcardRound();
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].HomeTeam.Id; // 2 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].HomeTeam.Id; // 3 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].HomeTeam.Id; // 4 wins

        var divisionalRound = season.GenerateAfcDivisionalRound(wildcardRound);
        divisionalRound.Games[0].SelectedWinner = divisionalRound.Games[0].HomeTeam.Id; // 1 wins
        divisionalRound.Games[1].SelectedWinner = divisionalRound.Games[1].AwayTeam.Id; // 3 wins

        // Act
        var conferenceRound = season.GenerateAfcConferenceRound(divisionalRound);

        // Assert
        conferenceRound.Should().NotBeNull();
        conferenceRound.Conference.Should().Be("AFC");
        conferenceRound.Name.Should().Be("Conference");
        conferenceRound.RoundNumber.Should().Be(3);
        conferenceRound.PointValue.Should().Be(3);
        conferenceRound.Games.Should().HaveCount(1, "conference championship has 1 game");

        // Verify NFL playoff rule: Highest seed (1) gets home field, plays lowest seed (3)
        conferenceRound.Games[0].HomeTeam.Seed.Should().Be(1, "highest remaining seed (#1) should be home");
        conferenceRound.Games[0].AwayTeam.Seed.Should().Be(3, "lowest remaining seed (#3) should be away");
    }

    [TestMethod]
    public void GenerateNfcConferenceRound_LowestSeedPlaysHighestSeed()
    {
        // Arrange - Divisional winners: 1, 2
        var season = CreateSeasonWithTeams();
        var wildcardRound = season.GenerateNfcWildcardRound();
        wildcardRound.Games[0].SelectedWinner = wildcardRound.Games[0].HomeTeam.Id; // 2 wins
        wildcardRound.Games[1].SelectedWinner = wildcardRound.Games[1].HomeTeam.Id; // 3 wins
        wildcardRound.Games[2].SelectedWinner = wildcardRound.Games[2].HomeTeam.Id; // 4 wins

        var divisionalRound = season.GenerateNfcDivisionalRound(wildcardRound);
        divisionalRound.Games[0].SelectedWinner = divisionalRound.Games[0].HomeTeam.Id; // 1 wins
        divisionalRound.Games[1].SelectedWinner = divisionalRound.Games[1].AwayTeam.Id; // 2 wins

        // Act
        var conferenceRound = season.GenerateNfcConferenceRound(divisionalRound);

        // Assert
        conferenceRound.Conference.Should().Be("NFC");
        conferenceRound.Games.Should().HaveCount(1);

        // Verify NFL playoff rule: Highest seed (1) gets home field, plays lowest seed (3)
        conferenceRound.Games[0].HomeTeam.Seed.Should().Be(1, "highest remaining seed (#1) should be home");
        conferenceRound.Games[0].AwayTeam.Seed.Should().Be(3, "lowest remaining seed (#3) should be away");
    }

    [TestMethod]
    public void GenerateSuperBowlRound_MatchesConferenceWinners()
    {
        // Arrange - Set up complete bracket through conference championships
        var season = CreateSeasonWithTeams();
        
        // AFC path
        var afcWildcard = season.GenerateAfcWildcardRound();
        afcWildcard.Games[0].SelectedWinner = afcWildcard.Games[0].HomeTeam.Id; // 2 wins
        afcWildcard.Games[1].SelectedWinner = afcWildcard.Games[1].HomeTeam.Id; // 3 wins
        afcWildcard.Games[2].SelectedWinner = afcWildcard.Games[2].HomeTeam.Id; // 4 wins

        var afcDivisional = season.GenerateAfcDivisionalRound(afcWildcard);
        afcDivisional.Games[0].SelectedWinner = afcDivisional.Games[0].HomeTeam.Id; // 1 wins
        afcDivisional.Games[1].SelectedWinner = afcDivisional.Games[1].HomeTeam.Id; // 3 wins

        var afcConference = season.GenerateAfcConferenceRound(afcDivisional);
        afcConference.Games[0].SelectedWinner = afcConference.Games[0].HomeTeam.Id; // 1 wins (home team)

        // NFC path
        var nfcWildcard = season.GenerateNfcWildcardRound();
        nfcWildcard.Games[0].SelectedWinner = nfcWildcard.Games[0].HomeTeam.Id; // 2 wins
        nfcWildcard.Games[1].SelectedWinner = nfcWildcard.Games[1].HomeTeam.Id; // 3 wins
        nfcWildcard.Games[2].SelectedWinner = nfcWildcard.Games[2].HomeTeam.Id; // 4 wins

        var nfcDivisional = season.GenerateNfcDivisionalRound(nfcWildcard);
        nfcDivisional.Games[0].SelectedWinner = nfcDivisional.Games[0].HomeTeam.Id; // 1 wins
        nfcDivisional.Games[1].SelectedWinner = nfcDivisional.Games[1].AwayTeam.Id; // 3 wins (away team)

        var nfcConference = season.GenerateNfcConferenceRound(nfcDivisional);
        nfcConference.Games[0].SelectedWinner = nfcConference.Games[0].HomeTeam.Id; // 1 wins (home team)

        // Act
        var superBowl = season.GenerateSuperBowlRound(afcConference, nfcConference);

        // Assert
        superBowl.Should().NotBeNull();
        superBowl.Conference.Should().Be("Super Bowl");
        superBowl.Name.Should().Be("Super Bowl");
        superBowl.RoundNumber.Should().Be(4);
        superBowl.PointValue.Should().Be(5);
        superBowl.Games.Should().HaveCount(1, "Super Bowl has 1 game");

        // Both conference winners are seed 1, so Super Bowl should be AFC #1 vs NFC #1
        superBowl.Games[0].HomeTeam.Seed.Should().Be(1, "AFC winner (seed 1) should be represented");
        superBowl.Games[0].AwayTeam.Seed.Should().Be(1, "NFC winner (seed 1) should be represented");
        
        // Verify conferences are different
        var homeTeam = season.Teams.First(t => t.Id == superBowl.Games[0].HomeTeam.Id);
        var awayTeam = season.Teams.First(t => t.Id == superBowl.Games[0].AwayTeam.Id);
        homeTeam.Conference.Should().NotBe(awayTeam.Conference, "Super Bowl should match AFC vs NFC");
    }
}