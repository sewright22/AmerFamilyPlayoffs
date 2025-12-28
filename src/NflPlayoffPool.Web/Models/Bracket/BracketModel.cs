// <copyright file="BracketModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Bracket
{
    using System.ComponentModel.DataAnnotations;

    public class BracketModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Bracket Name", Prompt = "Please name your bracket")]
        [Required(ErrorMessage = "Don't forget to name your bracket.")]
        public string? Name { get; set; }

        public int SeasonYear { get; set; }

        public bool CanEdit { get; set; }

        public List<RoundModel> AfcRounds { get; set; } = new List<RoundModel>();
        public List<RoundModel> NfcRounds { get; set; } = new List<RoundModel>();
        public MatchupModel? SuperBowl { get; set; }
    }
}
