// <copyright file="SeasonModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models;

    public class SeasonModel : IModal
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Year")]
        public required string Year { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Current Season")]
        public bool IsCurrent { get; set; }

        public int TimeZoneOffset { get; set; }

        [Display(Name = "Cutoff Date and Time")]
        public DateTime? CutoffDateTime { get; set; }

        [Display(Name = "Bracket Count")]
        public int BracketCount { get; set; }

        [Required]
        [Display(Name = "Status")]
        public SeasonStatus Status { get; set; }

        [Required]
        [Display(Name = "Current Round")]
        public PlayoffRound CurrentRound { get; set; }

        [Required]
        [Display(Name = "Wildcard Points")]
        public int WildcardPoints { get; set; } = 2;

        [Required]
        [Display(Name = "Divisional Points")]
        public int DivisionalPoints { get; set; } = 3;

        [Required]
        [Display(Name = "Conference Points")]
        public int ConferencePoints { get; set; } = 5;

        [Required]
        [Display(Name = "Super Bowl Points")]
        public int SuperBowlPoints { get; set; } = 8;

        [Display(Name = "Import SuperGrid")]
        public IFormFile? SuperGridFile { get; set; }

        public List<AdminRoundModel> Rounds { get; } = new();
        public List<TeamModel> Teams { get; } = new();
        public string? Title
        {
            get
            {
                return $"{this.Year} Season";
            }
            set { }
        }
    }
}
