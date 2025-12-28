// <copyright file="CreateSeasonViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Models;

    public class CreateSeasonViewModel : IModal
    {
        public string? Title
        {
            get
            {
                return "Create Season";
            }
            set
            { }
        }

        public required SeasonModel Season { get; set; }
    }
}
