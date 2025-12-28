// <copyright file="Bracket.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Represents a bracket in the NFL playoff pool.
    /// </summary>
    public class Bracket
    {
        /// <summary>
        /// Gets or sets the unique identifier for the bracket.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the season year for the bracket.
        /// </summary>
        [BsonElement("seasonYear")]
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the name of the bracket.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the bracket.
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last modified date of the bracket.
        /// </summary>
        [BsonElement("lastModified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bracket is submitted.
        /// </summary>
        [BsonElement("isSubmitted")]
        public bool IsSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the predicted winner of the bracket.
        /// </summary>
        [BsonElement("predictedWinner")]
        public string? PredictedWinner { get; set; }

        /// <summary>
        /// Gets or sets the collection of bracket picks.
        /// </summary>
        [BsonElement("picks")]
        public ICollection<BracketPick> Picks { get; set; } = new List<BracketPick>();
        public int CurrentScore { get; set; }
        public int MaxPossibleScore { get; set; }
    }
}
