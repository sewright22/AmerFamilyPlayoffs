// <copyright file="RoundModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Bracket
{
    public class RoundModel
    {
        public int Id { get; set; }
        public required int RoundNumber { get; set; }
        public int PointValue { get; set; }
        public string Name { get; set; }
        public string? Conference { get; set; }
        public bool IsLocked { get; set; }
        public List<MatchupModel> Games { get; set; } = new List<MatchupModel>();
    }
}
