// <copyright file="SuperGridRawImportCell.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NflPlayoffPool.Data.Models
{
    public class SuperGridRawImportCell
    {
        public required string Name { get; set; }
        public string? Value { get; set; }
        public string? Formula { get; set; }
    }
}
