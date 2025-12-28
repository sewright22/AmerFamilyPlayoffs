// <copyright file="Season.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Season
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("isCurrent")]
        public bool IsCurrent { get; set; }

        [BsonElement("status")]
        public SeasonStatus Status { get; set; }

        [BsonElement("currentRound")]
        public PlayoffRound CurrentRound { get; set; }

        [BsonElement("submissionDeadline")]
        public DateTime SubmissionDeadline { get; set; }

        [BsonElement("wildcardPoints")]
        public int WildcardPoints { get; set; }

        [BsonElement("divisionalPoints")]
        public int DivisionalPoints { get; set; }

        [BsonElement("conferencePoints")]
        public int ConferencePoints { get; set; }

        [BsonElement("superBowlPoints")]
        public int SuperBowlPoints { get; set; }

        [BsonElement("teams")]
        public ICollection<PlayoffTeam> Teams { get; set; } = new List<PlayoffTeam>();

        [BsonElement("bracket")]
        public MasterBracket? Bracket { get; set; }
    }
}
