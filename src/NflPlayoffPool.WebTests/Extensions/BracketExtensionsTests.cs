using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Extensions;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for BracketExtensions, focusing on the complex CalculateScores method
/// </summary>
[TestClass]
public class BracketExtensionsTests
{
    [TestMethod]
    public void CalculateScores_WithEmptyMasterBracket_SetsMaxPossibleScoreOnly()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(3, 3).ForConference("AFC").ForGame(9).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(4, 5).ForConference("Super Bowl").ForGame(13).WithWinner("team1", "Team 1").Build()
            )
            .Build();

        var emptyMasterBracket = new MasterBracketBuilder()
            .WithPicks() // No picks
            .Build();

        season.Bracket = emptyMasterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        userBracket.CurrentScore.Should().Be(0, "no master bracket results exist yet");
        userBracket.MaxPossibleScore.Should().Be(42, "default max score should be set");
    }

    [TestMethod]
    public void CalculateScores_WithCorrectWildcardPicks_AddsWildcardPoints()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team2", "Team 2").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(3).WithWinner("team3", "Team 3").Build()
            )
            .Build();

        var masterBracket = new MasterBracketBuilder()
            .WithPicks(
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(), // Correct
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team99", "Team 99").Build(), // Wrong
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(3).WithWinner("team3", "Team 3").Build()  // Correct
            )
            .Build();

        season.Bracket = masterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        userBracket.CurrentScore.Should().Be(2, "should get 1 point each for 2 correct wildcard picks");
        userBracket.MaxPossibleScore.Should().Be(41, "should lose 1 point for incorrect pick");
    }

    [TestMethod]
    public void CalculateScores_WithEliminatedTeamInLaterRounds_ReducesMaxPossibleScore()
    {
        // Arrange - This tests the complex elimination logic
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                // User picks team1 to win wildcard, but team1 loses
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                // User also picks team1 to win divisional - this should reduce max possible score
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(),
                // User also picks team1 to win conference - this should also reduce max possible score
                new BracketPickBuilder().ForRound(3, 3).ForConference("AFC").ForGame(9).WithWinner("team1", "Team 1").Build()
            )
            .Build();

        var masterBracket = new MasterBracketBuilder()
            .WithPicks(
                // Master bracket shows team1 lost in wildcard
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team2", "Team 2").Build()
                // No divisional or conference results yet
            )
            .Build();

        season.Bracket = masterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        userBracket.CurrentScore.Should().Be(0, "team1 lost, so no points earned");
        userBracket.MaxPossibleScore.Should().Be(36, "should lose 1+2+3=6 points for eliminated team across all rounds");
    }

    [TestMethod]
    public void CalculateScores_WithAllRoundsComplete_CalculatesFullScore()
    {
        // Arrange - Test complete bracket scoring
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                // AFC Wildcard picks (2 games, 1 point each)
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team2", "Team 2").Build(),
                // NFC Wildcard picks (2 games, 1 point each)
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(3).WithWinner("team3", "Team 3").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(4).WithWinner("team4", "Team 4").Build(),
                // AFC Divisional picks (2 games, 2 points each)
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(6).WithWinner("team5", "Team 5").Build(),
                // NFC Divisional picks (2 games, 2 points each)
                new BracketPickBuilder().ForRound(2, 2).ForConference("NFC").ForGame(7).WithWinner("team3", "Team 3").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("NFC").ForGame(8).WithWinner("team6", "Team 6").Build(),
                // Conference Championship picks (2 games, 3 points each)
                new BracketPickBuilder().ForRound(3, 3).ForConference("AFC").ForGame(9).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(3, 3).ForConference("NFC").ForGame(10).WithWinner("team3", "Team 3").Build(),
                // Super Bowl pick (1 game, 5 points)
                new BracketPickBuilder().ForRound(4, 5).ForConference("Super Bowl").ForGame(13).WithWinner("team1", "Team 1").Build()
            )
            .Build();

        var masterBracket = new MasterBracketBuilder()
            .WithPicks(
                // All picks match user picks exactly
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team2", "Team 2").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(3).WithWinner("team3", "Team 3").Build(),
                new BracketPickBuilder().ForRound(1, 1).ForConference("NFC").ForGame(4).WithWinner("team4", "Team 4").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(6).WithWinner("team5", "Team 5").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("NFC").ForGame(7).WithWinner("team3", "Team 3").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("NFC").ForGame(8).WithWinner("team6", "Team 6").Build(),
                new BracketPickBuilder().ForRound(3, 3).ForConference("AFC").ForGame(9).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(3, 3).ForConference("NFC").ForGame(10).WithWinner("team3", "Team 3").Build(),
                new BracketPickBuilder().ForRound(4, 5).ForConference("Super Bowl").ForGame(13).WithWinner("team1", "Team 1").Build()
            )
            .Build();

        season.Bracket = masterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        var expectedScore = (4 * 1) + (4 * 2) + (2 * 3) + (1 * 5); // 4+8+6+5 = 23
        userBracket.CurrentScore.Should().Be(expectedScore, "all picks are correct");
        userBracket.MaxPossibleScore.Should().Be(42, "no points should be lost for perfect bracket");
    }

    [TestMethod]
    public void CalculateScores_WithPartialResults_HandlesIncompleteRounds()
    {
        // Arrange - Test when only some rounds have results
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(3, 3).ForConference("AFC").ForGame(9).WithWinner("team1", "Team 1").Build(),
                new BracketPickBuilder().ForRound(4, 5).ForConference("Super Bowl").ForGame(13).WithWinner("team1", "Team 1").Build()
            )
            .Build();

        var masterBracket = new MasterBracketBuilder()
            .WithPicks(
                // Only wildcard results available
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build()
                // No divisional, conference, or super bowl results yet
            )
            .Build();

        season.Bracket = masterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        userBracket.CurrentScore.Should().Be(1, "should get 1 point for correct wildcard pick");
        userBracket.MaxPossibleScore.Should().Be(42, "should maintain full potential since team1 is still alive");
    }

    [TestMethod]
    public void CalculateScores_WithMixedResults_CalculatesCorrectly()
    {
        // Arrange - Test realistic scenario with some correct and some incorrect picks
        var season = new SeasonBuilder()
            .WithPointValues(wildcard: 1, divisional: 2, conference: 3, superBowl: 5)
            .Build();

        var userBracket = new BracketBuilder()
            .WithPicks(
                // Wildcard picks - 1 correct, 1 wrong
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(), // Correct
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team2", "Team 2").Build(), // Wrong
                // Divisional picks - user picked eliminated team2 and surviving team1
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(), // Correct
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(6).WithWinner("team2", "Team 2").Build()  // Wrong (team2 eliminated)
            )
            .Build();

        var masterBracket = new MasterBracketBuilder()
            .WithPicks(
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(1).WithWinner("team1", "Team 1").Build(), // team1 wins
                new BracketPickBuilder().ForRound(1, 1).ForConference("AFC").ForGame(2).WithWinner("team3", "Team 3").Build(), // team3 wins (not team2)
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(5).WithWinner("team1", "Team 1").Build(), // team1 wins divisional
                new BracketPickBuilder().ForRound(2, 2).ForConference("AFC").ForGame(6).WithWinner("team4", "Team 4").Build()  // team4 wins (not team2)
            )
            .Build();

        season.Bracket = masterBracket;

        // Act
        userBracket.CalculateScores(season);

        // Assert
        userBracket.CurrentScore.Should().Be(3, "1 point for wildcard + 2 points for divisional = 3");
        userBracket.MaxPossibleScore.Should().Be(39, "should lose 1 point for wrong wildcard + 2 points for wrong divisional = 3 points lost");
    }
}