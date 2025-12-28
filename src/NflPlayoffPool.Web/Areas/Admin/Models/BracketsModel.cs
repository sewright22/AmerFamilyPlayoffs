// <copyright file="BracketsModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using NflPlayoffPool.Web.Models.Home;
    using NflPlayoffPool.Web.ViewModels;

    public class BracketsModel : IBreadcrumb
    {
        public BracketsModel()
        {
            this.Brackets = new List<BracketSummaryModel>();

            this.BreadcrumbList = new List<BreadcrumbItemModel>
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

        public List<BracketSummaryModel> Brackets { get; }

        public List<BreadcrumbItemModel> BreadcrumbList { get; private set; }
    }
}
