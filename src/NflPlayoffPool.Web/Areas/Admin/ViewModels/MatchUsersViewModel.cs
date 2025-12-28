// <copyright file="MatchUsersViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class MatchUsersViewModel : IBreadcrumb
    {
        public List<SuperGridImportUserModel> ImportedUsers { get; set; } = new List<SuperGridImportUserModel>();
        public List<User> ExistingUsers { get; set; } = new List<User>();

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
