using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Extensions;
using NflPlayoffPool.Web.Models.Bracket;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for BracketExtensions.SetSelectedWinners() method
/// This method sets winners on round games based on bracket picks and validates against master picks
/// </summary>
[TestClass]
public class BracketExtensions_SetSelectedWinnersTests
{
    [TestMethod]
    public void SetSelectedWinners_EmptyPicksCollection_DoesNotSetAnyWinners()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").WithName("Home Team").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").WithName("Away Team").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var emptyPicks = new List<BracketPick>();

        // Act
        round.SetSelectedWinners(emptyPicks);

        // Assert
        matchup.SelectedWinner.Should().BeNull("no picks provided");
        homeTeam.Selected.Should().BeFalse("no winner selected");
        awayTeam.Selected.Should().BeFalse("no winner selected");
    }

    [TestMethod]
    public void SetSelectedWinners_MatchingPick_SetsHomeTeamAsWinner()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").WithName("Home Team").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").WithName("Away Team").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        homeTeam.Selected.Should().BeTrue("home team was selected as winner");
        awayTeam.Selected.Should().BeFalse("away team was not selected");
    }

    [TestMethod]
    public void SetSelectedWinners_MatchingPick_SetsAwayTeamAsWinner()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").WithName("Home Team").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").WithName("Away Team").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team2")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().Be("team2");
        homeTeam.Selected.Should().BeFalse("home team was not selected");
        awayTeam.Selected.Should().BeTrue("away team was selected as winner");
    }

    [TestMethod]
    public void SetSelectedWinners_NoMatchingPick_DoesNotSetWinner()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").WithName("Home Team").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").WithName("Away Team").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("NFC") // Different conference
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().BeNull("no matching pick found");
        homeTeam.Selected.Should().BeFalse("no winner selected");
        awayTeam.Selected.Should().BeFalse("no winner selected");
    }

    [TestMethod]
    public void SetSelectedWinners_MultipleGames_SetsCorrectWinnersForEachGame()
    {
        // Arrange
        var game1Home = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var game1Away = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var game2Home = new PlayoffTeamModelBuilder().WithId("team3").Build();
        var game2Away = new PlayoffTeamModelBuilder().WithId("team4").Build();

        var matchup1 = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(game1Home)
            .WithAwayTeam(game1Away)
            .Build();

        var matchup2 = new MatchupModelBuilder()
            .WithGameNumber(2)
            .WithHomeTeam(game2Home)
            .WithAwayTeam(game2Away)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGames(matchup1, matchup2)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1") // Home team wins game 1
                .Build(),
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(2)
                .WithPredictedWinningId("team4") // Away team wins game 2
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup1.SelectedWinner.Should().Be("team1");
        game1Home.Selected.Should().BeTrue();
        game1Away.Selected.Should().BeFalse();

        matchup2.SelectedWinner.Should().Be("team4");
        game2Home.Selected.Should().BeFalse();
        game2Away.Selected.Should().BeTrue();
    }

    [TestMethod]
    public void SetSelectedWinners_WithMasterPicks_SetsCorrectPickAsCorrect()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        var masterPicks = new List<string?> { "team1", "team3", "team5" }; // team1 is correct

        // Act
        round.SetSelectedWinners(picks, masterPicks);

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        matchup.IsCorrect.Should().BeTrue("pick matches master picks");
    }

    [TestMethod]
    public void SetSelectedWinners_WithMasterPicks_SetsIncorrectPickAsIncorrect()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        var masterPicks = new List<string?> { "team2", "team3", "team5" }; // team1 is not in master picks

        // Act
        round.SetSelectedWinners(picks, masterPicks);

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        matchup.IsCorrect.Should().BeFalse("pick does not match master picks");
    }

    [TestMethod]
    public void SetSelectedWinners_WithEmptyMasterPicks_DoesNotSetCorrectness()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        var emptyMasterPicks = new List<string?>();

        // Act
        round.SetSelectedWinners(picks, emptyMasterPicks);

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        matchup.IsCorrect.Should().BeNull("no master picks to compare against");
    }

    [TestMethod]
    public void SetSelectedWinners_WithNullPredictedWinningId_DoesNotSetCorrectness()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId(null) // No winner selected
                .Build()
        };

        var masterPicks = new List<string?> { "team1", "team2" };

        // Act
        round.SetSelectedWinners(picks, masterPicks);

        // Assert
        matchup.SelectedWinner.Should().BeNull();
        matchup.IsCorrect.Should().BeNull("no predicted winner to validate");
        homeTeam.Selected.Should().BeFalse();
        awayTeam.Selected.Should().BeFalse();
    }

    [TestMethod]
    public void SetSelectedWinners_WithNullsInMasterPicks_FiltersNullsCorrectly()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        var masterPicksWithNulls = new List<string?> { null, "team1", null, "team3" };

        // Act
        round.SetSelectedWinners(picks, masterPicksWithNulls);

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        matchup.IsCorrect.Should().BeTrue("team1 is in non-null master picks");
    }

    [TestMethod]
    public void SetSelectedWinners_DifferentConference_DoesNotMatch()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("NFC") // Different conference
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().BeNull("conference mismatch");
    }

    [TestMethod]
    public void SetSelectedWinners_DifferentRoundNumber_DoesNotMatch()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(2) // Different round
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().BeNull("round number mismatch");
    }

    [TestMethod]
    public void SetSelectedWinners_DifferentGameNumber_DoesNotMatch()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(2) // Different game number
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks);

        // Assert
        matchup.SelectedWinner.Should().BeNull("game number mismatch");
    }

    [TestMethod]
    public void SetSelectedWinners_OverloadWithoutMasterPicks_CallsMainOverload()
    {
        // Arrange
        var homeTeam = new PlayoffTeamModelBuilder().WithId("team1").Build();
        var awayTeam = new PlayoffTeamModelBuilder().WithId("team2").Build();
        var matchup = new MatchupModelBuilder()
            .WithGameNumber(1)
            .WithHomeTeam(homeTeam)
            .WithAwayTeam(awayTeam)
            .Build();

        var round = new RoundModelBuilder()
            .WithConference("AFC")
            .WithRoundNumber(1)
            .WithGame(matchup)
            .Build();

        var picks = new List<BracketPick>
        {
            new BracketPickBuilder()
                .WithConference("AFC")
                .WithRoundNumber(1)
                .WithGameNumber(1)
                .WithPredictedWinningId("team1")
                .Build()
        };

        // Act
        round.SetSelectedWinners(picks); // Overload without master picks

        // Assert
        matchup.SelectedWinner.Should().Be("team1");
        homeTeam.Selected.Should().BeTrue();
        awayTeam.Selected.Should().BeFalse();
        matchup.IsCorrect.Should().BeNull("no master picks provided");
    }
}