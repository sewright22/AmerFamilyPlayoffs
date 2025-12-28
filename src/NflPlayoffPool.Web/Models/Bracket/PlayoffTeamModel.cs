// <copyright file="PlayoffTeamModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Bracket
{
    public class PlayoffTeamModel
    {
        public string Id { get; set; }
        public int Seed { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the mathchup. The same team id may appear in multiple matchups.
        /// </summary>
        public string ViewId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }
}
