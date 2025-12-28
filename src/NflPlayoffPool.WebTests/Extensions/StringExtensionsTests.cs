using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Web.Extensions;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for StringExtensions utility methods
/// These methods contain logic for string manipulation and validation
/// </summary>
[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void GetControllerNameForUri_WithControllerSuffix_RemovesSuffix()
    {
        // Arrange
        var controllerName = "HomeController";

        // Act
        var result = controllerName.GetControllerNameForUri();

        // Assert
        result.Should().Be("Home");
    }

    [TestMethod]
    public void GetControllerNameForUri_WithoutControllerSuffix_ReturnsOriginal()
    {
        // Arrange
        var controllerName = "Home";

        // Act
        var result = controllerName.GetControllerNameForUri();

        // Assert
        result.Should().Be("Home");
    }

    [TestMethod]
    public void GetControllerNameForUri_WithEmptyString_ReturnsEmpty()
    {
        // Arrange
        var controllerName = "";

        // Act
        var result = controllerName.GetControllerNameForUri();

        // Assert
        result.Should().Be("");
    }

    [TestMethod]
    public void GetControllerNameForUri_WithMultipleControllerOccurrences_RemovesAll()
    {
        // Arrange
        var controllerName = "ControllerController";

        // Act
        var result = controllerName.GetControllerNameForUri();

        // Assert
        result.Should().Be("");
    }

    [TestMethod]
    public void HasValue_WithValidString_ReturnsTrue()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void HasValue_WithNullString_ReturnsFalse()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void HasValue_WithEmptyString_ReturnsFalse()
    {
        // Arrange
        var value = "";

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void HasValue_WithWhitespaceString_ReturnsFalse()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void HasValue_WithIgnoreStrings_IgnoresSpecifiedValues()
    {
        // Arrange
        var value = "ignore_me";

        // Act
        var result = value.HasValue("ignore_me", "also_ignore");

        // Assert
        result.Should().BeFalse("value should be ignored when in ignore list");
    }

    [TestMethod]
    public void HasValue_WithIgnoreStrings_AcceptsNonIgnoredValues()
    {
        // Arrange
        var value = "valid_value";

        // Act
        var result = value.HasValue("ignore_me", "also_ignore");

        // Assert
        result.Should().BeTrue("value should be accepted when not in ignore list");
    }

    [TestMethod]
    public void HasValue_WithIgnoreStrings_CaseSensitive()
    {
        // Arrange
        var value = "IGNORE_ME";

        // Act
        var result = value.HasValue("ignore_me");

        // Assert
        result.Should().BeTrue("ignore comparison should be case sensitive");
    }

    [TestMethod]
    public void HasValue_WithTrimmedWhitespace_HandlesCorrectly()
    {
        // Arrange
        var value = "  valid value  ";

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeTrue("whitespace should be trimmed before evaluation");
    }

    [TestMethod]
    public void HasValue_WithIgnoreStringsAndWhitespace_TrimsBeforeComparison()
    {
        // Arrange
        var value = "  ignore_me  ";

        // Act
        var result = value.HasValue("ignore_me");

        // Assert
        result.Should().BeFalse("whitespace should be trimmed before ignore comparison");
    }

    [TestMethod]
    public void HasValue_WithEmptyIgnoreStrings_BehavesNormally()
    {
        // Arrange
        var value = "test";

        // Act
        var result = value.HasValue();

        // Assert
        result.Should().BeTrue("should work normally with no ignore strings");
    }
}