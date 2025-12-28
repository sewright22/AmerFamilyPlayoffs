// <copyright file="SuperGridImporter.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using OfficeOpenXml;

    /// <summary>
    /// Service for importing SuperGrid data from an Excel file.
    /// </summary>
    public class SuperGridImporter
    {
        private readonly PlayoffPoolContext context;
        private readonly Stream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperGridImporter"/> class with a file path.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="filePath">The path to the Excel file.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
        public SuperGridImporter(PlayoffPoolContext context, string filePath)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            this.fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperGridImporter"/> class with a file stream.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="fileStream">The stream of the Excel file.</param>
        /// <exception cref="ArgumentNullException">Thrown when context or fileStream is null.</exception>
        public SuperGridImporter(PlayoffPoolContext context, Stream fileStream)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
        }

        /// <summary>
        /// Imports SuperGrid data from the Excel file.
        /// </summary>
        /// <param name="name">The name of the import.</param>
        /// <returns>The imported SuperGrid data.</returns>
        public SuperGridRawImport Import(string name)
        {
            var superGridRawImport = new SuperGridRawImport
            {
                Name = name,
                ImportDate = DateTime.Now,
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(this.fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                for (int row = 1; row <= rowCount; row++)
                {
                    var importRow = new SuperGridRawImportRow
                    {
                        RowNumber = row,
                    };

                    for (int col = 1; col <= colCount; col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        SuperGridRawImportCell importCell = new SuperGridRawImportCell()
                        {
                            Name = $"Column{col}",
                            Value = cell.Value?.ToString(),
                            Formula = cell.Formula,
                        };
                        importRow.Cells.Add(importCell);
                    }

                    superGridRawImport.Rows.Add(importRow);
                }
            }

            // Save to database (if needed)
            this.context.SuperGridRawImports.Add(superGridRawImport);
            this.context.SaveChanges();

            return superGridRawImport;
        }
    }
}
