// <copyright file="Movie.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Movie
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [BsonElement("title")]

        public string Title { get; set; } = string.Empty;

        [BsonElement("rated")]
        public string? Rated { get; set; } = string.Empty;

        [BsonElement("plot")]
        public string? Plot { get; set; }

        [BsonElement("new_column")]
        public string? NewColumn { get; set; }
    }
}
