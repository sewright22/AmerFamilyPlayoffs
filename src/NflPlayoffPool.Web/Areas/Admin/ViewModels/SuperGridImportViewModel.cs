// <copyright file="SuperGridImportViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using System.Collections.Generic;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class SuperGridImportViewModel : IBreadcrumb
    {
        public List<BreadcrumbItemModel> BreadcrumbList { get; private set; }

        public SuperGridImportViewModel()
        {
            BreadcrumbList = new List<BreadcrumbItemModel>
            {
                new BreadcrumbItemModel
                {
                    Text = "Admin",
                    Url = "/Admin",
                    IsActive = false,
                },
                new BreadcrumbItemModel
                {
                    Text = "Manage Seasons",
                    Url = "/Admin/Season",
                    IsActive = true,
                },
            };
        }

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
