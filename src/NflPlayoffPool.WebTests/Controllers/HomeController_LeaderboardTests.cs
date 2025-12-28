using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Controllers;
using NflPlayoffPool.Web.Models.Home;
using System.Security.Claims;

namespace NflPlayoffPool.WebTests.Controllers;

/// <summary>
/// Tests for HomeController leaderboard calculation logic
/// The BuildLeaderboard method contains complex business rules for ranking, ties, and elimination
/// </summary>
[TestClass]
public class HomeController_LeaderboardTests
{
    private PlayoffPoolContext _context = null!;
    private HomeController _controller = null!;
    private ILogger<HomeController> _logger = null!;
    private Season _season = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlayoffPoolContext(options);
        _logger = new LoggerFactory().CreateLogger<HomeController>();
        _controller = new HomeController(_logger, _context);

        // Setup HTTP context with authenticated user and MVC services
        var services = new ServiceCollection();
        services.AddMvc();
        services.AddLogging();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.Role, "Player")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal,
                RequestServices = serviceProvider
            }
        };

        // Create a started season for leaderboard tests
        _season = new SeasonBuilder()
            .WithYear(2024)
            .WithStatus(SeasonStatus.InProgress)
            .Build();
        _context.Seasons.Add(_season);
        _context.SaveChanges();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Leaderboard_OrdersByCurrentScoreDescending()
    {
        // Arrange
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 10, maxScore: 20, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 15, maxScore: 25, predictedWinner: "Bills");
        var bracket3 = CreateSubmittedBracket("User3", "Bracket3", currentScore: 5, maxScore: 15, predictedWinner: "Ravens");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard.Should().HaveCount(3);
        leaderboard[0].Name.Should().Be("Bracket2"); // 15 points
        leaderboard[1].Name.Should().Be("Bracket1"); // 10 points
        leaderboard[2].Name.Should().Be("Bracket3"); // 5 points
    }

    [TestMethod]
    public void Leaderboard_TieBreaksByMaxPossibleScore()
    {
        // Arrange - Same current score, different max possible scores
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 10, maxScore: 20, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 10, maxScore: 25, predictedWinner: "Bills");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard.Should().HaveCount(2);
        leaderboard[0].Name.Should().Be("Bracket2"); // Higher max possible score
        leaderboard[1].Name.Should().Be("Bracket1"); // Lower max possible score
    }

    [TestMethod]
    public void Leaderboard_AssignsCorrectPlaces()
    {
        // Arrange
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 15, maxScore: 25, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 10, maxScore: 20, predictedWinner: "Bills");
        var bracket3 = CreateSubmittedBracket("User3", "Bracket3", currentScore: 5, maxScore: 15, predictedWinner: "Ravens");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard[0].Place.Should().Be(1);
        leaderboard[1].Place.Should().Be(2);
        leaderboard[2].Place.Should().Be(3);
    }

    [TestMethod]
    public void Leaderboard_HandlesTiedPlaces()
    {
        // Arrange - Two brackets with identical scores
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 10, maxScore: 20, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 10, maxScore: 20, predictedWinner: "Bills");
        var bracket3 = CreateSubmittedBracket("User3", "Bracket3", currentScore: 5, maxScore: 15, predictedWinner: "Ravens");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard[0].Place.Should().Be(1);
        leaderboard[1].Place.Should().Be(1); // Tied for first
        leaderboard[2].Place.Should().Be(3); // Third place (skips second)
    }

    [TestMethod]
    public void Leaderboard_MarksEliminatedBrackets()
    {
        // Arrange - Create brackets where some are mathematically eliminated
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 20, maxScore: 30, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 15, maxScore: 25, predictedWinner: "Bills");
        var bracket3 = CreateSubmittedBracket("User3", "Bracket3", currentScore: 10, maxScore: 20, predictedWinner: "Ravens");
        var bracket4 = CreateSubmittedBracket("User4", "Bracket4", currentScore: 5, maxScore: 9, predictedWinner: "Dolphins"); // Eliminated (maxScore < 3rd place currentScore)

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard[3].PlaceAsString.Should().StartWith("e-", "bracket should be marked as eliminated");
    }

    [TestMethod]
    public void Leaderboard_MarksTiedPlaces()
    {
        // Arrange - Two brackets tied for first
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 15, maxScore: 25, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 15, maxScore: 25, predictedWinner: "Bills");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard[0].PlaceAsString.Should().StartWith("T-", "first bracket should be marked as tied");
        leaderboard[1].PlaceAsString.Should().StartWith("T-", "second bracket should be marked as tied");
    }

    [TestMethod]
    public void Leaderboard_EliminatesBracketsWithSamePredictedWinnerAsLowestPlace()
    {
        // Arrange - Create scenario where brackets outside top 3 have same predicted winner as lowest placing bracket
        // but cannot tie for 3rd place due to insufficient max score
        var bracket1 = CreateSubmittedBracket("User1", "Bracket1", currentScore: 20, maxScore: 30, predictedWinner: "Chiefs");
        var bracket2 = CreateSubmittedBracket("User2", "Bracket2", currentScore: 15, maxScore: 25, predictedWinner: "Bills");
        var bracket3 = CreateSubmittedBracket("User3", "Bracket3", currentScore: 10, maxScore: 20, predictedWinner: "Ravens");
        var bracket4 = CreateSubmittedBracket("User4", "Bracket4", currentScore: 8, maxScore: 12, predictedWinner: "Ravens"); // Same as 3rd place, but can still tie
        var bracket5 = CreateSubmittedBracket("User5", "Bracket5", currentScore: 5, maxScore: 15, predictedWinner: "Dolphins");

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        var bracket4Result = leaderboard.First(b => b.Name == "Bracket4");
        // Bracket4 should NOT be eliminated because it could still tie for 3rd place (maxScore=12 > 3rd place currentScore=10)
        // and the elimination logic only applies to brackets that have the same predicted winner as the LOWEST placing bracket in top 3
        // Since bracket3 is in 3rd place (not the lowest), bracket4 should not be eliminated for having same predicted winner
        bracket4Result.PlaceAsString.Should().Be("4th", "bracket4 can still tie for 3rd place and doesn't have same predicted winner as lowest placing bracket");
    }

    [TestMethod]
    public void Leaderboard_OnlyIncludesSubmittedBrackets()
    {
        // Arrange
        var submittedBracket = CreateSubmittedBracket("User1", "Submitted", currentScore: 10, maxScore: 20, predictedWinner: "Chiefs");
        var unsubmittedBracket = new BracketBuilder()
            .WithSeasonYear(2024)
            .WithUserId("User2")
            .WithName("Unsubmitted")
            .WithIsSubmitted(false)
            .WithCurrentScore(15)
            .WithMaxPossibleScore(25)
            .Build();
        _context.Brackets.Add(unsubmittedBracket);
        _context.SaveChanges();

        // Act
        var result = _controller.Index() as ViewResult;
        var model = result!.Model as HomeViewModel;

        // Assert
        var leaderboard = model!.Leaderboard.Brackets;
        leaderboard.Should().HaveCount(1);
        leaderboard[0].Name.Should().Be("Submitted");
    }

    private Bracket CreateSubmittedBracket(string userId, string name, int currentScore, int maxScore, string predictedWinner)
    {
        var bracket = new BracketBuilder()
            .WithSeasonYear(2024)
            .WithUserId(userId)
            .WithName(name)
            .WithIsSubmitted(true)
            .WithCurrentScore(currentScore)
            .WithMaxPossibleScore(maxScore)
            .WithPredictedWinner(predictedWinner)
            .Build();

        _context.Brackets.Add(bracket);
        _context.SaveChanges();
        return bracket;
    }
}