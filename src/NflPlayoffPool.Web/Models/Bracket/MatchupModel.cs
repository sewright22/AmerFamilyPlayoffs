// <copyright file="MatchupModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Bracket
{
    using System.ComponentModel.DataAnnotations;

    public class MatchupModel
    {
        public int GameNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public required PlayoffTeamModel HomeTeam { get; set; }
        public required PlayoffTeamModel AwayTeam { get; set; }

        [Required(ErrorMessage = "Pick a winner for this game")]
        public string? SelectedWinner { get; set; }
        public bool? IsCorrect { get; set; }
        public bool IsLocked { get; set; }
    }
}
