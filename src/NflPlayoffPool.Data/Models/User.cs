// <copyright file="User.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("roles")]
        public List<Role> Roles { get; set; } = new List<Role>();

        [BsonElement("aliases")]
        public List<string>? Aliases { get; set; } = new List<string>();

        [BsonIgnore] // Exclude from MongoDB serialization
        public string RolesCsv => string.Join(",", Roles.Select(r => r.ToString()));

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
