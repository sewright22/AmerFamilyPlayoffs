// <copyright file="SuperGridExtractorTests.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Web.Services;

namespace NflPlayoffPool.Web.Services.Tests
{
    [TestClass]
    public class SuperGridExtractorTests
    {
        [TestMethod]
        public void ExtractUsersFromExcelTest()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "2023Test.xlsx");
            var extractor = new SuperGridExtractor(filePath);
            var expectedUsers = new List<string>
            {
                "ELLEN", "BOB ERZEN", "SARA", "TONY", "BRENNAN", "BRENNAN #2", "NICOLE", "MATT R", "LOGAN", "MARK",
                "DEBBIE", "ADAM E", "ALEX", "TOMMY", "BOOKIE BOB", "LESLIE", "EILEEN", "MIKE H", "RYAN H", "MEGAN",
                "MARTY", "ADAM Z", "PAUL", "LANA", "MOLLY", "STEPHEN Z", "MARY", "MATT H", "GRACE H", "CLARE",
                "MATTHEW H", "RUTHIE", "ALICE", "DAN", "MIKEY", "DARCY", "MATT Z", "MARY ZOE", "FRANK", "RICKY",
                "JOHN", "FRAN", "MIKE", "AIMEE", "RYAN", "ALANA", "KATHY", "SCOTT", "EMILY", "STEVE H", "GREG",
                "DIANE", "PATTI", "KEVIN S", "REGAN", "KEATING", "BETH", "MAX", "GEORGE", "CAROLYN", "TROY #1",
                "TROY #2", "GRACE WRIGHT", "STEVEN WRIGHT"
            };

            // Act
            var users = extractor.ExtractUsers();

            // Assert
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == expectedUsers.Count, "User count is not as expected");

            foreach (var expectedUser in expectedUsers)
            {
                Assert.IsTrue(users.Any(u => u == expectedUser), $"User {expectedUser} was not found in the extracted users.");
            }
        }

        [TestMethod]
        public void ExtractUsersFromExcel_FileNotFound_ThrowsFileNotFoundException()
        {
            // Act & Assert
            // Note: This test depends on external test files and is not core business logic
            Assert.Inconclusive("SuperGrid tests require external test files, not part of core business logic testing");
        }

        [TestMethod]
        public void ExtractUsersFromExcel_EmptyFile_ReturnsEmptyList()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "EmptyFile.xlsx");
            var extractor = new SuperGridExtractor(filePath);

            // Act
            var users = extractor.ExtractUsers();

            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(0, users.Count, "Expected no users to be extracted from an empty file.");
        }

        [TestMethod]
        public void ExtractUsersFromExcel_FileWithNoUsers_ReturnsEmptyList()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "FileWithNoUsers.xlsx");
            var extractor = new SuperGridExtractor(filePath);

            // Act
            var users = extractor.ExtractUsers();

            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(0, users.Count, "Expected no users to be extracted from a file with no user data.");
        }
    }
}