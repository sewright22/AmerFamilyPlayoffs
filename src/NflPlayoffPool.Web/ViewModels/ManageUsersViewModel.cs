// <copyright file="ManageUsersViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.ViewModels
{
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models;

    public class ManageUsersViewModel : IBreadcrumb
    {
        public List<User> Users { get; } = new List<User>();

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
                Text = "Manage Users",
                Url = "/Admin/ManageUsers",
                IsActive = true,
            },
        };
    }
}
