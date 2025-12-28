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
/// Tests for HomeController business logic
/// These methods contain complex leaderboard calculation and bracket management logic
/// </summary>
[TestClass]
public class HomeControllerTests
{
    private PlayoffPoolContext _context = null!;
    private HomeController _controller = null!;
    private ILogger<HomeController> _logger = null!;

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
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Index_WithNoCurrentSeason_DisablesBracketSubmission()
    {
        // Arrange - No season in database

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as HomeViewModel;
        model.Should().NotBeNull();
        model!.CanSubmitBrackets.Should().BeFalse("no current season should disable bracket submission");
    }

    [TestMethod]
    public void Index_WithCurrentSeason_EnablesBracketSubmission()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithStatus(SeasonStatus.NotStarted)
            .Build();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as HomeViewModel;
        model.Should().NotBeNull();
        model!.CanSubmitBrackets.Should().BeTrue("current season should enable bracket submission");
    }

    [TestMethod]
    public void Index_WithStartedSeason_ShowsLeaderboard()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithStatus(SeasonStatus.InProgress)
            .Build();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as HomeViewModel;
        model.Should().NotBeNull();
        model!.IsPlayoffStarted.Should().BeTrue("in-progress season should show playoffs as started");
        model.Leaderboard.ShowLeaderboard.Should().BeTrue("started season should show leaderboard");
    }

    [TestMethod]
    public void Index_WithNotStartedSeason_HidesLeaderboard()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithStatus(SeasonStatus.NotStarted)
            .Build();
        _context.Seasons.Add(season);
        _context.SaveChanges();

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as HomeViewModel;
        model.Should().NotBeNull();
        model!.IsPlayoffStarted.Should().BeFalse("not-started season should show playoffs as not started");
        model.Leaderboard.ShowLeaderboard.Should().BeFalse("not-started season should hide leaderboard");
    }

    [TestMethod]
    public void Index_SeparatesIncompleteAndCompletedBrackets()
    {
        // Arrange
        var season = new SeasonBuilder()
            .WithYear(2024)
            .WithStatus(SeasonStatus.NotStarted)
            .Build();
        _context.Seasons.Add(season);

        var incompleteBracket = new BracketBuilder()
            .WithSeasonYear(2024)
            .WithUserId("test-user-id")
            .WithName("Incomplete Bracket")
            .WithIsSubmitted(false)
            .Build();

        var completedBracket = new BracketBuilder()
            .WithSeasonYear(2024)
            .WithUserId("test-user-id")
            .WithName("Completed Bracket")
            .WithIsSubmitted(true)
            .Build();

        _context.Brackets.AddRange(incompleteBracket, completedBracket);
        _context.SaveChanges();

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var model = result!.Model as HomeViewModel;
        model.Should().NotBeNull();
        model!.IncompleteBrackets.Should().HaveCount(1);
        model.IncompleteBrackets.First().Name.Should().Be("Incomplete Bracket");
        model.CompletedBrackets.Should().HaveCount(1);
        model.CompletedBrackets.First().Name.Should().Be("Completed Bracket");
    }

    [TestMethod]
    public void Error_ReturnsErrorView()
    {
        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
    }
}