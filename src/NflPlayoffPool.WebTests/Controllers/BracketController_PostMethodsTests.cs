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
using System.Security.Claims;

namespace NflPlayoffPool.WebTests.Controllers;

/// <summary>
/// Tests for BracketController POST methods (Create and Update)
/// These methods contain complex business logic for bracket creation and updates
/// </summary>
[TestClass]
public class BracketController_PostMethodsTests
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
    public void Create_Post_WithValidModel_RedirectsToUpdate()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithName("Test Bracket")
            .Build();

        // Act
        var result = _controller.Create(bracketModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Update");
        redirectResult.RouteValues.Should().ContainKey("id");
        redirectResult.RouteValues!["id"].Should().NotBeNull();
    }

    [TestMethod]
    public void Create_Post_WithNoCurrentSeason_ReturnsViewWithError()
    {
        // Arrange - No season in database
        var bracketModel = new BracketModelBuilder()
            .WithName("Test Bracket")
            .Build();

        // Act
        var result = _controller.Create(bracketModel);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(bracketModel);
        
        _controller.ModelState.Should().ContainKey(string.Empty);
        _controller.ModelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "No current season found.");
    }

    [TestMethod]
    public void Create_Post_WithInvalidModelState_ReturnsView()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithName("Test Bracket")
            .Build();

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = _controller.Create(bracketModel);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(bracketModel);
    }

    [TestMethod]
    public void Create_Post_SavesBracketToDatabase()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithName("Test Bracket")
            .Build();

        // Act
        _controller.Create(bracketModel);

        // Assert
        var savedBracket = _context.Brackets.FirstOrDefault();
        savedBracket.Should().NotBeNull();
        savedBracket!.Name.Should().Be("Test Bracket");
        savedBracket.UserId.Should().Be("test-user-id");
        savedBracket.SeasonYear.Should().Be(season.Year);
        savedBracket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        savedBracket.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public void Update_Post_WithExistingBracket_UpdatesBracket()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        var existingBracket = new BracketBuilder()
            .WithId("existing-bracket-id")
            .WithName("Original Name")
            .WithUserId("test-user-id")
            .WithSeasonYear(season.Year)
            .Build();

        _context.Seasons.Add(season);
        _context.Brackets.Add(existingBracket);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("existing-bracket-id")
            .WithName("Updated Name")
            .Build();

        // Act
        var result = _controller.Update(bracketModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        
        var updatedBracket = _context.Brackets.First(b => b.Id == "existing-bracket-id");
        updatedBracket.Name.Should().Be("Updated Name");
        updatedBracket.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public void Update_Post_WithNewBracket_CreatesBracket()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("new-bracket-id")
            .WithName("New Bracket")
            .Build();

        // Act
        var result = _controller.Update(bracketModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        
        var newBracket = _context.Brackets.FirstOrDefault(b => b.Id == "new-bracket-id");
        newBracket.Should().NotBeNull();
        newBracket!.Name.Should().Be("New Bracket");
        newBracket.UserId.Should().Be("test-user-id");
    }

    [TestMethod]
    public void Update_Post_WithCompleteBracket_RedirectsToHome()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("complete-bracket-id")
            .WithName("Complete Bracket")
            .WithCompleteSuperBowl("afc-1") // Has Super Bowl winner
            .Build();

        // Act
        var result = _controller.Update(bracketModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }

    [TestMethod]
    public void Update_Post_WithIncompleteBracket_RedirectsToUpdate()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("incomplete-bracket-id")
            .WithName("Incomplete Bracket")
            .Build(); // No Super Bowl winner

        // Act
        var result = _controller.Update(bracketModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Update");
        redirectResult.RouteValues!["id"].Should().Be("incomplete-bracket-id");
    }

    [TestMethod]
    public void Update_Post_SetsIsSubmittedCorrectly()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("submitted-bracket-id")
            .WithName("Submitted Bracket")
            .WithAllPicks() // Has all 13 picks
            .Build();

        // Act
        _controller.Update(bracketModel);

        // Assert
        var savedBracket = _context.Brackets.First(b => b.Id == "submitted-bracket-id");
        savedBracket.IsSubmitted.Should().BeTrue("bracket with all 13 picks should be marked as submitted");
    }

    [TestMethod]
    public void Update_Post_SetsPredictedWinnerFromSuperBowl()
    {
        // Arrange
        var season = CreateSeasonWithTeams();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        var bracketModel = new BracketModelBuilder()
            .WithId("winner-bracket-id")
            .WithName("Winner Bracket")
            .WithCompleteSuperBowl("afc-1")
            .Build();

        // Act
        _controller.Update(bracketModel);

        // Assert
        var savedBracket = _context.Brackets.First(b => b.Id == "winner-bracket-id");
        savedBracket.PredictedWinner.Should().Be("AFC Team 1", "predicted winner should be set from Super Bowl pick");
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

        // Add AFC teams (seeds 1-7)
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

        // Add NFC teams (seeds 1-7)
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
}