using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Controllers;
using NflPlayoffPool.Web.Models.Bracket;
using NflPlayoffPool.Web.ViewModels;
using System.Security.Claims;

namespace NflPlayoffPool.WebTests.Controllers;

/// <summary>
/// Tests for BracketController bracket building methods
/// These methods contain complex business logic for building playoff brackets
/// </summary>
[TestClass]
public class BracketControllerTests
{
    private PlayoffPoolContext _context = null!;
    private BracketController _controller = null!;
    private ILogger<BracketController> _logger = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlayoffPoolContext(options);
        _logger = new LoggerFactory().CreateLogger<BracketController>();
        _controller = new BracketController(_logger, _context);

        // Setup user context
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "Test User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Create_Get_ReturnsViewWithNewBracket()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithIsCurrent(true)
            .WithStatus(SeasonStatus.NotStarted)
            .Build();
        
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Create() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<BracketViewModel>();
        
        var viewModel = result.Model as BracketViewModel;
        viewModel!.Bracket.Should().NotBeNull();
        viewModel.Bracket.UserId.Should().Be("test-user-id");
        viewModel.Bracket.CanEdit.Should().BeTrue("season has not started");
    }

    [TestMethod]
    public void Create_Get_WhenSeasonStarted_SetsCanEditToFalse()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithIsCurrent(true)
            .WithStatus(SeasonStatus.InProgress)
            .WithSubmissionDeadline(DateTime.UtcNow.AddDays(-1)) // Past deadline
            .Build();
        
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Create() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var viewModel = result!.Model as BracketViewModel;
        viewModel!.Bracket.CanEdit.Should().BeFalse("season has started");
    }

    [TestMethod]
    public void Update_Get_WithValidBracket_BuildsCompleteRounds()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        var bracket = CreateBracketWithAllPicks(season);
        
        _context.Seasons.Add(season);
        _context.Brackets.Add(bracket);
        _context.SaveChanges();

        // Act
        var result = _controller.Update(bracket.Id) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var viewModel = result!.Model as BracketViewModel;
        viewModel.Should().NotBeNull();
        
        // Verify all rounds are built
        viewModel!.Bracket.AfcRounds.Should().HaveCount(3, "AFC has wildcard, divisional, and conference rounds");
        viewModel.Bracket.NfcRounds.Should().HaveCount(3, "NFC has wildcard, divisional, and conference rounds");
        viewModel.Bracket.SuperBowl.Should().NotBeNull("Super Bowl should be built");
        
        // Verify round structure
        var afcWildcard = viewModel.Bracket.AfcRounds.FirstOrDefault(r => r.RoundNumber == 1);
        afcWildcard.Should().NotBeNull();
        afcWildcard!.Games.Should().HaveCount(3, "AFC wildcard has 3 games");
        
        var afcDivisional = viewModel.Bracket.AfcRounds.FirstOrDefault(r => r.RoundNumber == 2);
        afcDivisional.Should().NotBeNull();
        afcDivisional!.Games.Should().HaveCount(2, "AFC divisional has 2 games");
        
        var afcConference = viewModel.Bracket.AfcRounds.FirstOrDefault(r => r.RoundNumber == 3);
        afcConference.Should().NotBeNull();
        afcConference!.Games.Should().HaveCount(1, "AFC conference has 1 game");
    }

    [TestMethod]
    public void Update_Get_WithPartialBracket_BuildsOnlyAvailableRounds()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        var bracket = CreateBracketWithWildcardPicksOnly(season);
        
        _context.Seasons.Add(season);
        _context.Brackets.Add(bracket);
        _context.SaveChanges();

        // Act
        var result = _controller.Update(bracket.Id) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var viewModel = result!.Model as BracketViewModel;
        
        // Should have wildcard and divisional rounds (divisional built from wildcard picks)
        viewModel!.Bracket.AfcRounds.Should().HaveCount(2, "AFC has wildcard and divisional rounds");
        viewModel.Bracket.NfcRounds.Should().HaveCount(2, "NFC has wildcard and divisional rounds");
        viewModel.Bracket.SuperBowl.Should().BeNull("Super Bowl cannot be built without conference picks");
    }

    [TestMethod]
    public void Update_Get_WithNonExistentBracket_RedirectsToCreate()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Update("non-existent-id");

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirect = result as RedirectToActionResult;
        redirect!.ActionName.Should().Be("Create");
    }

    [TestMethod]
    public void Update_Get_SetsLockStatusBasedOnEditability()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        season.Status = SeasonStatus.InProgress; // Season started, should lock bracket
        
        var bracket = CreateBracketWithAllPicks(season);
        bracket.UserId = "test-user-id"; // Same as controller user
        
        _context.Seasons.Add(season);
        _context.Brackets.Add(bracket);
        _context.SaveChanges();

        // Act
        var result = _controller.Update(bracket.Id) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var viewModel = result!.Model as BracketViewModel;
        
        viewModel!.Bracket.CanEdit.Should().BeFalse("season has started");
        
        // All rounds should be locked
        foreach (var round in viewModel.Bracket.AfcRounds)
        {
            round.IsLocked.Should().BeTrue("round should be locked when bracket cannot be edited");
            foreach (var game in round.Games)
            {
                game.IsLocked.Should().BeTrue("game should be locked when bracket cannot be edited");
            }
        }
        
        viewModel.Bracket.SuperBowl!.IsLocked.Should().BeTrue("Super Bowl should be locked when bracket cannot be edited");
    }

    [TestMethod]
    public void Reset_WithValidBracket_ClearsPicks()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        var bracket = CreateBracketWithAllPicks(season);
        bracket.IsSubmitted = true;
        bracket.PredictedWinner = "Some Team";
        
        _context.Seasons.Add(season);
        _context.Brackets.Add(bracket);
        _context.SaveChanges();

        var originalPickCount = bracket.Picks.Count;
        originalPickCount.Should().BeGreaterThan(0, "bracket should have picks before reset");

        // Act
        var result = _controller.Reset(bracket.Id);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        
        // Verify bracket was reset
        var updatedBracket = _context.Brackets.Include(b => b.Picks).First(b => b.Id == bracket.Id);
        updatedBracket.Picks.Should().BeEmpty("picks should be cleared");
        updatedBracket.IsSubmitted.Should().BeFalse("submission status should be reset");
        updatedBracket.PredictedWinner.Should().BeNull("predicted winner should be cleared");
    }

    [TestMethod]
    public void Reset_WithNonExistentBracket_DoesNotThrow()
    {
        // Act & Assert - Should not throw exception
        var result = _controller.Reset("non-existent-id");
        
        result.Should().BeOfType<RedirectToActionResult>();
        var redirect = result as RedirectToActionResult;
        redirect!.ActionName.Should().Be("Update");
    }

    // Helper methods
    private Season CreateSeasonWithTeams()
    {
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithIsCurrent(true)
            .WithStatus(SeasonStatus.NotStarted)
            .WithSubmissionDeadline(DateTime.UtcNow.AddDays(1))
            .Build();

        // Add AFC teams (seeds 1-7) - using consistent ID pattern
        for (int seed = 1; seed <= 7; seed++)
        {
            season.Teams.Add(new PlayoffTeamBuilder()
                .WithId($"afc-{seed}")
                .WithCode($"AFC{seed}")
                .WithName($"AFC Team {seed}")
                .WithSeed(seed)
                .WithConference(Conference.AFC)
                .Build());
        }

        // Add NFC teams (seeds 1-7) - using consistent ID pattern
        for (int seed = 1; seed <= 7; seed++)
        {
            season.Teams.Add(new PlayoffTeamBuilder()
                .WithId($"nfc-{seed}")
                .WithCode($"NFC{seed}")
                .WithName($"NFC Team {seed}")
                .WithSeed(seed)
                .WithConference(Conference.NFC)
                .Build());
        }

        return season;
    }

    private Bracket CreateBracketWithAllPicks(Season season)
    {
        var bracket = new BracketBuilder()
            .WithId("test-bracket-id")
            .WithName("Test Bracket")
            .WithUserId("test-user-id")
            .WithSeasonYear(season.Year)
            .Build();

        // Add wildcard picks (AFC: games 1-3, NFC: games 4-6)
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(1).WithWinner("afc-2", "AFC Team 2").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(2).WithWinner("afc-3", "AFC Team 3").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(3).WithWinner("afc-4", "AFC Team 4").Build());
        
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(4).WithWinner("nfc-2", "NFC Team 2").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(5).WithWinner("nfc-3", "NFC Team 3").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(6).WithWinner("nfc-4", "NFC Team 4").Build());

        // Add divisional picks (AFC: games 7-8, NFC: games 9-10)
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(2, 2).ForGame(7).WithWinner("afc-1", "AFC Team 1").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(2, 2).ForGame(8).WithWinner("afc-2", "AFC Team 2").Build());
        
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(2, 2).ForGame(9).WithWinner("nfc-1", "NFC Team 1").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(2, 2).ForGame(10).WithWinner("nfc-2", "NFC Team 2").Build());

        // Add conference picks (AFC: game 11, NFC: game 12)
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(3, 3).ForGame(11).WithWinner("afc-1", "AFC Team 1").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(3, 3).ForGame(12).WithWinner("nfc-1", "NFC Team 1").Build());

        // Add Super Bowl pick (game 13)
        bracket.Picks.Add(new BracketPickBuilder().ForConference("Super Bowl").ForRound(4, 5).ForGame(13).WithWinner("afc-1", "AFC Team 1").Build());

        return bracket;
    }

    private Bracket CreateBracketWithWildcardPicksOnly(Season season)
    {
        var bracket = new BracketBuilder()
            .WithId("test-bracket-wildcard-only")
            .WithName("Wildcard Only Bracket")
            .WithUserId("test-user-id")
            .WithSeasonYear(season.Year)
            .Build();

        // Add only wildcard picks
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(1).WithWinner("afc-2", "AFC Team 2").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(2).WithWinner("afc-3", "AFC Team 3").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("AFC").ForRound(1, 1).ForGame(3).WithWinner("afc-4", "AFC Team 4").Build());
        
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(4).WithWinner("nfc-2", "NFC Team 2").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(5).WithWinner("nfc-3", "NFC Team 3").Build());
        bracket.Picks.Add(new BracketPickBuilder().ForConference("NFC").ForRound(1, 1).ForGame(6).WithWinner("nfc-4", "NFC Team 4").Build());

        return bracket;
    }
}