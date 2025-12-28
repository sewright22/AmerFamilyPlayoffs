// <copyright file="SuperGridExtractor.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using NflPlayoffPool.Web.Areas.Admin.Models;
using OfficeOpenXml;

namespace NflPlayoffPool.Web.Services
{
    public class SuperGridExtractor
    {
        private readonly Stream _fileStream;

        public SuperGridExtractor(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public SuperGridExtractor(Stream fileStream)
        {
            _fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
        }

        public List<string> ExtractUsers()
        {
            var users = new List<string>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(_fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
                int columnCount = worksheet.Dimension.Columns;

                for (int column = 3; column <= columnCount; column++)
                {
                    var fullName = worksheet.Cells[1, column].Text;

                    var user = fullName.Trim();

                    if (!string.IsNullOrEmpty(user))
                    {
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public List<SuperGridImportUserModel> ExtractUserModels()
        {
            var userModels = new List<SuperGridImportUserModel>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(_fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns;

                for (int column = 3; column <= columnCount; column++)
                {
                    var fullName = worksheet.Cells[1, column].Text;
                    var user = new SuperGridImportUserModel
                    {
                        Name = fullName.Trim()
                    };

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var pick = worksheet.Cells[row, column].Text;
                        if (!string.IsNullOrEmpty(pick))
                        {
                            user.Picks.Add(pick.Trim());
                        }
                    }

                    if (!string.IsNullOrEmpty(user.Name))
                    {
                        userModels.Add(user);
                    }
                }
            }

            return userModels;
        }
    }
}
