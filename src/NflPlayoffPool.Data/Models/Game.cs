// <copyright file="Game.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Game
    {
        [BsonElement("id")]
        public Guid Id { get; set; }

        [BsonElement("round")]
        public PlayoffRound Round { get; set; }

        [BsonElement("gameTime")]
        public DateTime GameTime { get; set; }
    }
}
