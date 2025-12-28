// <copyright file="AdminRoundModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using System.ComponentModel;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models;

    public class AdminRoundModel : IModal
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public int PlayoffId { get; set; }

        [DisplayName("Round Number")]
        public int Number { get; set; }

        [DisplayName("Points")]
        public int PointValue { get; set; }
        public required string Name { get; set; }
        public List<string>? Winners { get; set; }
        public List<SelectListItem> Teams { get; } = new ();
        public List<SelectListItem> Rounds { get; } = new ();
        public string? Title
        {
            get
            {
                return this.Name.HasValue() ? $"Round {this.Number} - {this.Name}" : "Add Round";
            }
            set
            {

            }
        }
    }
}
