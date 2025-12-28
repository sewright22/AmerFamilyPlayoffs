// <copyright file="SuperGridRawImportRow.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NflPlayoffPool.Data.Models
{
    public class SuperGridRawImportRow
    {
        public int RowNumber { get; set; }
        public List<SuperGridRawImportCell> Cells { get; set; } = new List<SuperGridRawImportCell>();
    }
}
