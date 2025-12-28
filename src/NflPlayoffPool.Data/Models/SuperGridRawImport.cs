// <copyright file="SuperGridRawImport.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NflPlayoffPool.Data.Models
{
    public class SuperGridRawImport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("importDate")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [BsonElement("rows")]
        public List<SuperGridRawImportRow> Rows { get; set; } = new List<SuperGridRawImportRow>();
    }
}
