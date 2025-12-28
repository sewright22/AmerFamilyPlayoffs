using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Web.Extensions;

namespace NflPlayoffPool.WebTests.Extensions;

/// <summary>
/// Tests for DatabaseSeedingExtensions password hashing functionality
/// These methods contain security-critical logic for password management
/// </summary>
[TestClass]
public class DatabaseSeedingExtensionsTests
{
    [TestMethod]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().Contain(":", "hash should contain salt separator");
        
        var parts = hashedPassword.Split(':');
        parts.Should().HaveCount(2, "hash should have salt and hash parts");
        parts[0].Should().NotBeNullOrEmpty("salt should not be empty");
        parts[1].Should().NotBeNullOrEmpty("hash should not be empty");
    }

    [TestMethod]
    public void HashPassword_WithSamePassword_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = DatabaseSeedingExtensions.HashPassword(password);
        var hash2 = DatabaseSeedingExtensions.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2, "same password should produce different hashes due to random salt");
    }

    [TestMethod]
    public void HashPassword_WithEmptyPassword_ReturnsHash()
    {
        // Arrange
        var password = "";

        // Act
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().Contain(":");
    }

    [TestMethod]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, hashedPassword);

        // Assert
        isValid.Should().BeTrue("correct password should verify successfully");
    }

    [TestMethod]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "TestPassword123!";
        var incorrectPassword = "WrongPassword456!";
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(correctPassword);

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword(incorrectPassword, hashedPassword);

        // Assert
        isValid.Should().BeFalse("incorrect password should not verify");
    }

    [TestMethod]
    public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "TestPassword123!";
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(correctPassword);

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword("", hashedPassword);

        // Assert
        isValid.Should().BeFalse("empty password should not verify against non-empty hash");
    }

    [TestMethod]
    public void VerifyPassword_WithMalformedHash_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var malformedHash = "invalid_hash_format";

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, malformedHash);

        // Assert
        isValid.Should().BeFalse("malformed hash should not verify");
    }

    [TestMethod]
    public void VerifyPassword_WithHashMissingSalt_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashWithoutSalt = "just_a_hash";

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, hashWithoutSalt);

        // Assert
        isValid.Should().BeFalse("hash without salt should not verify");
    }

    [TestMethod]
    public void VerifyPassword_WithEmptyHashParts_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var emptyHash = ":";

        // Act
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, emptyHash);

        // Assert
        isValid.Should().BeFalse("empty hash parts should not verify");
    }

    [TestMethod]
    public void HashPassword_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var password = "P@ssw0rd!#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, hashedPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        isValid.Should().BeTrue("password with special characters should hash and verify correctly");
    }

    [TestMethod]
    public void HashPassword_WithUnicodeCharacters_HandlesCorrectly()
    {
        // Arrange
        var password = "Pässwörd123!ñ";

        // Act
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, hashedPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        isValid.Should().BeTrue("password with unicode characters should hash and verify correctly");
    }

    [TestMethod]
    public void HashPassword_WithLongPassword_HandlesCorrectly()
    {
        // Arrange
        var password = new string('a', 1000) + "123!";

        // Act
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);
        var isValid = DatabaseSeedingExtensions.VerifyPassword(password, hashedPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        isValid.Should().BeTrue("long password should hash and verify correctly");
    }
}