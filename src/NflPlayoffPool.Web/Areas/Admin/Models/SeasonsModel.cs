// <copyright file="SeasonsModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using NflPlayoffPool.Web.ViewModels;
    using NflPlayoffPool.Web.Models;

    public class SeasonsModel : IBreadcrumb
    {
        public List<SeasonSummaryModel> Seasons { get; } = new List<SeasonSummaryModel>();

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
                Text = "Seasons",
                IsActive = true,
            },
        };
    }
}
