// <copyright file="SuperGridImporterTests.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NflPlayoffPool.Data;
using NflPlayoffPool.Web.Services;

namespace NflPlayoffPool.Web.Services.Tests
{
    [TestClass()]
    public class SuperGridImporterTests
    {
        [TestMethod()]
        public void ImportTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ConvertExcelToJson_ValidFile_ReturnsCorrectJson()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "2023Test.xlsx");

            var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new PlayoffPoolContext(options);
            var importer = new SuperGridImporter(context, filePath);

            // Act
            var result = importer.Import("Test");

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.AreEqual("2023Test.xlsx", result.Name, "The file name should match.");
            Assert.IsTrue(result.Rows.Count > 0, "The result should contain rows.");

            // Additional assertions can be added here to verify the content of the result
        }
    }
}