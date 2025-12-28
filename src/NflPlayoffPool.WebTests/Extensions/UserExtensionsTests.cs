using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Extensions;
using NflPlayoffPool.Web.Areas.Admin.Models;
using System.Security.Claims;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for UserExtensions user management functionality
/// These methods contain business logic for user updates and claims processing
/// </summary>
[TestClass]
public class UserExtensionsTests
{
    private PlayoffPoolContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlayoffPoolContext(options);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task UpdateUser_WithValidUser_UpdatesUserProperties()
    {
        // Arrange
        var existingUser = new UserBuilder()
            .WithId(Guid.NewGuid())
            .WithEmail("original@example.com")
            .WithFirstName("Original")
            .WithLastName("User")
            .WithRole(Role.Player)
            .Build();

        _context.Users.Add(existingUser);
        _context.SaveChanges();

        var userModel = new UserModel
        {
            Id = existingUser.Id,
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "Name",
            RoleIds = new List<string> { ((int)Role.Admin).ToString() }
        };

        // Act
        await _context.UpdateUser(userModel);

        // Assert
        var updatedUser = await _context.Users.FirstAsync(u => u.Id == existingUser.Id);
        updatedUser.Email.Should().Be("updated@example.com");
        updatedUser.FirstName.Should().Be("Updated");
        updatedUser.LastName.Should().Be("Name");
        updatedUser.Roles.Should().Contain(Role.Admin);
    }

    [TestMethod]
    public async Task UpdateUser_WithNonExistentUser_DoesNotThrow()
    {
        // Arrange
        var userModel = new UserModel
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Email = "nonexistent@example.com",
            FirstName = "Non",
            LastName = "Existent",
            RoleIds = new List<string> { ((int)Role.Player).ToString() }
        };

        // Act & Assert
        var act = async () => await _context.UpdateUser(userModel);
        await act.Should().NotThrowAsync("updating non-existent user should not throw exception");
    }

    [TestMethod]
    public void GetUserId_WithValidClaims_ReturnsUserId()
    {
        // Arrange
        var userId = "test-user-123";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "Test User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        result.Should().Be(userId);
    }

    [TestMethod]
    public void GetUserId_WithoutNameIdentifierClaim_ReturnsEmptyString()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        result.Should().Be(string.Empty, "should return empty string when NameIdentifier claim is missing");
    }

    [TestMethod]
    public void GetUserId_WithEmptyNameIdentifierClaim_ReturnsEmptyString()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, ""),
            new Claim(ClaimTypes.Name, "Test User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = principal.GetUserId();

        // Assert
        result.Should().Be(string.Empty);
    }
}