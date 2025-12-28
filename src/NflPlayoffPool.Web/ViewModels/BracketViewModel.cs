// <copyright file="BracketViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.ViewModels
{
    using System.Collections.Generic;
    using NflPlayoffPool.Web.Models.Bracket;

    public class BracketViewModel : IBreadcrumb
    {
        public string UserId { get; set; }
        public required BracketModel Bracket { get; set; }

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

        internal void AddBreadcrumb(string displayText)
        {
            var lastItem = this.BreadcrumbList.LastOrDefault();

            if (lastItem != null)
            {
                lastItem.IsActive = false;
            }

            this.BreadcrumbList.Add(new BreadcrumbItemModel
            {
                Text = !string.IsNullOrEmpty(displayText) ? displayText : "New Season",
                Url = "/Admin/Season/Index",
                IsActive = true,
            });
        }
    }
}
