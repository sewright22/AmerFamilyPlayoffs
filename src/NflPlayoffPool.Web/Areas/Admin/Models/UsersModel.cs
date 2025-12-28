// <copyright file="UsersModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class UsersModel : IBreadcrumb
    {
        public List<UserModel> Users { get; } = new List<UserModel>();

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
                Text = "Users",
                Url = "/Admin/User/Index",
                IsActive = true,
            },
        };
    }
}
