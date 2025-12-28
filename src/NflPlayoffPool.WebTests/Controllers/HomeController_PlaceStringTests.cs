using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data;
using NflPlayoffPool.Web.Controllers;
using System.Reflection;
using System.Security.Claims;

namespace NflPlayoffPool.WebTests.Controllers;

/// <summary>
/// Tests for HomeController BuildPlaceAsString method
/// This method contains specific business logic for ordinal number formatting and tie/elimination markers
/// </summary>
[TestClass]
public class HomeController_PlaceStringTests
{
    private PlayoffPoolContext _context = null!;
    private HomeController _controller = null!;
    private ILogger<HomeController> _logger = null!;
    private MethodInfo _buildPlaceAsStringMethod = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlayoffPoolContext(options);
        _logger = new LoggerFactory().CreateLogger<HomeController>();
        _controller = new HomeController(_logger, _context);

        // Setup HTTP context with authenticated user
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
                User = principal
            }
        };

        // Get the private BuildPlaceAsString method using reflection
        _buildPlaceAsStringMethod = typeof(HomeController)
            .GetMethod("BuildPlaceAsString", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void BuildPlaceAsString_FirstPlace_Returns1st()
    {
        // Act
        var result = InvokeBuildPlaceAsString(1, false, false);

        // Assert
        result.Should().Be("1st");
    }

    [TestMethod]
    public void BuildPlaceAsString_SecondPlace_Returns2nd()
    {
        // Act
        var result = InvokeBuildPlaceAsString(2, false, false);

        // Assert
        result.Should().Be("2nd");
    }

    [TestMethod]
    public void BuildPlaceAsString_ThirdPlace_Returns3rd()
    {
        // Act
        var result = InvokeBuildPlaceAsString(3, false, false);

        // Assert
        result.Should().Be("3rd");
    }

    [TestMethod]
    public void BuildPlaceAsString_FourthPlace_Returns4th()
    {
        // Act
        var result = InvokeBuildPlaceAsString(4, false, false);

        // Assert
        result.Should().Be("4th");
    }

    [TestMethod]
    public void BuildPlaceAsString_EleventhPlace_Returns11th()
    {
        // Act - Special case: 11th should be "11th", not "11st"
        var result = InvokeBuildPlaceAsString(11, false, false);

        // Assert
        result.Should().Be("11th", "11th is a special case that should not use 'st' suffix");
    }

    [TestMethod]
    public void BuildPlaceAsString_TwelfthPlace_Returns12th()
    {
        // Act - Special case: 12th should be "12th", not "12nd"
        var result = InvokeBuildPlaceAsString(12, false, false);

        // Assert
        result.Should().Be("12th", "12th is a special case that should not use 'nd' suffix");
    }

    [TestMethod]
    public void BuildPlaceAsString_ThirteenthPlace_Returns13th()
    {
        // Act - Special case: 13th should be "13th", not "13rd"
        var result = InvokeBuildPlaceAsString(13, false, false);

        // Assert
        result.Should().Be("13th", "13th is a special case that should not use 'rd' suffix");
    }

    [TestMethod]
    public void BuildPlaceAsString_TwentyFirstPlace_Returns21st()
    {
        // Act - After the teens, normal rules apply again
        var result = InvokeBuildPlaceAsString(21, false, false);

        // Assert
        result.Should().Be("21st", "21st should use normal ordinal rules after the teens");
    }

    [TestMethod]
    public void BuildPlaceAsString_TwentySecondPlace_Returns22nd()
    {
        // Act
        var result = InvokeBuildPlaceAsString(22, false, false);

        // Assert
        result.Should().Be("22nd");
    }

    [TestMethod]
    public void BuildPlaceAsString_TwentyThirdPlace_Returns23rd()
    {
        // Act
        var result = InvokeBuildPlaceAsString(23, false, false);

        // Assert
        result.Should().Be("23rd");
    }

    [TestMethod]
    public void BuildPlaceAsString_TiedPlace_AddsTPrefix()
    {
        // Act
        var result = InvokeBuildPlaceAsString(1, true, false);

        // Assert
        result.Should().Be("T-1st", "tied places should have 'T-' prefix");
    }

    [TestMethod]
    public void BuildPlaceAsString_EliminatedPlace_AddsEPrefix()
    {
        // Act
        var result = InvokeBuildPlaceAsString(5, false, true);

        // Assert
        result.Should().Be("e-5th", "eliminated places should have 'e-' prefix");
    }

    [TestMethod]
    public void BuildPlaceAsString_EliminatedTakesPrecedenceOverTied()
    {
        // Act - Both tied and eliminated
        var result = InvokeBuildPlaceAsString(3, true, true);

        // Assert
        result.Should().Be("e-3rd", "elimination marker should take precedence over tie marker");
    }

    [TestMethod]
    public void BuildPlaceAsString_LargeNumbers_UseCorrectSuffix()
    {
        // Act & Assert - Test various large numbers
        InvokeBuildPlaceAsString(101, false, false).Should().Be("101st");
        InvokeBuildPlaceAsString(102, false, false).Should().Be("102nd");
        InvokeBuildPlaceAsString(103, false, false).Should().Be("103rd");
        InvokeBuildPlaceAsString(104, false, false).Should().Be("104th");
        InvokeBuildPlaceAsString(111, false, false).Should().Be("111th"); // Special case
        InvokeBuildPlaceAsString(112, false, false).Should().Be("112th"); // Special case
        InvokeBuildPlaceAsString(113, false, false).Should().Be("113th"); // Special case
        InvokeBuildPlaceAsString(121, false, false).Should().Be("121st"); // Back to normal
    }

    private string InvokeBuildPlaceAsString(int place, bool isTied, bool isEliminated)
    {
        return (string)_buildPlaceAsStringMethod.Invoke(_controller, new object[] { place, isTied, isEliminated })!;
    }
}