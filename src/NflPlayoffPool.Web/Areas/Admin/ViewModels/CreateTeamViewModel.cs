// <copyright file="CreateTeamViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Models;

    public class CreateTeamViewModel : IModal
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

        public required TeamModel Team { get; set; }
        public List<SelectListItem> Teams { get; internal set; }
    }
}
