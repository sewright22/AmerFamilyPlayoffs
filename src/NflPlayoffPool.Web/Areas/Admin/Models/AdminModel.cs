// <copyright file="AdminModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class AdminModel : IBreadcrumb
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
