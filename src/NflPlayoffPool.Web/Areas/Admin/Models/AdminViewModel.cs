// <copyright file="AdminViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using NflPlayoffPool.Web.ViewModels;
using NflPlayoffPool.Web.Models;

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    public class AdminViewModel : IBreadcrumb
    {
        public List<BreadcrumbItemModel> BreadcrumbList => new List<BreadcrumbItemModel>
        {
            new BreadcrumbItemModel
            {
                Text = "Admin",
                Url = "/Admin",
                IsActive = true,
            },
        };
    }
}
