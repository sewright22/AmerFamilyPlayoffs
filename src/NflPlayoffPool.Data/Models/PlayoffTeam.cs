// <copyright file="PlayoffTeam.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Represents a team in the NFL playoff pool.
    /// </summary>
    public class PlayoffTeam
    {
        /// <summary>
        /// Gets or sets the unique identifier for the team.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = new ObjectId().ToString();

        /// <summary>
        /// Gets or sets the code of the team (e.g., "SF", "GB", "KC").
        /// </summary>
        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of the team.
        /// </summary>
        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the conference of the team.
        /// </summary>
        [BsonElement("conference")]
        public Conference Conference { get; set; }

        /// <summary>
        /// Gets or sets the division of the team.
        /// </summary>
        [BsonElement("division")]
        public string Division { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the seed of the team.
        /// </summary>
        [BsonElement("seed")]
        public int Seed { get; set; }
    }
}

