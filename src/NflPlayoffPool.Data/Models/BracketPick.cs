// <copyright file="BracketPick.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Represents a bracket pick in the NFL playoff pool.
    /// </summary>
    public class BracketPick
    {
        /// <summary>
        /// Gets or sets the unique identifier for the bracket pick.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("conference")]
        public required string Conference { get; set; }

        /// <summary>
        /// Gets or sets the round number for the bracket pick.
        /// </summary>
        [BsonElement("roundNumber")]
        public int RoundNumber { get; set; }

        /// <summary>
        /// Gets or sets the point value for the bracket pick.
        /// </summary>
        [BsonElement("pointValue")]
        public int PointValue { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the game.
        /// </summary>
        [BsonElement("gameNumber")]
        public int GameNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the predicted winner.
        /// </summary>
        [BsonElement("predictedWinningId")]
        public string? PredictedWinningId { get; set; }

        [BsonElement("predictedWinningTeam")]
        public string? PredictedWinningTeam { get; set; }

        /// <summary>
        /// Gets or sets the points earned for the bracket pick.
        /// </summary>
        [BsonElement("pointsEarned")]
        public int? PointsEarned { get; set; }
    }
}
