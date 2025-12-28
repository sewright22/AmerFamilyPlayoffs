// <copyright file="TeamModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;
    using NflPlayoffPool.Data.Models;

    public class TeamModel
    {
        public string? Id { get; set; }

        [Required]
        public string SeasonId { get; set; }

        [Display(Name = "Team Name")]
        [Required]
        public string Code { get; set; }

        [Required]
        public int Seed { get; set; }
    }
}
