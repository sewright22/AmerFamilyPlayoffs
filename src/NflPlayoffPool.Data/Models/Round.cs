// <copyright file="Round.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Represents a round in the NFL playoff pool.
    /// </summary>
    public class Round
    {
        /// <summary>
        /// Gets or sets the unique identifier for the round.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the round.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the point value for the round.
        /// </summary>
        [BsonElement("pointValue")]
        public int PointValue { get; set; }
    }
}
