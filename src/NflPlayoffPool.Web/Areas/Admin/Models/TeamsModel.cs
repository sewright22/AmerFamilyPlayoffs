// <copyright file="TeamsModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class TeamsModel : IBreadcrumb
    {
        public List<TeamModel> Teams { get; } = new List<TeamModel>();

        public List<BreadcrumbItemModel> BreadcrumbList => new List<BreadcrumbItemModel>
        {
            new BreadcrumbItemModel
            {
                Text = "Admin",
                Url = "/Admin",
                IsActive = false,
            },
            new BreadcrumbItemModel
            {
                Text = "Teams",
                IsActive = true,
            },
        };
    }
}
