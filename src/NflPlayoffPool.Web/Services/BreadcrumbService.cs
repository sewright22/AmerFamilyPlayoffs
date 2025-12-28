// <copyright file="BreadcrumbService.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Services
{
    using NflPlayoffPool.Web.ViewModels;

    public class BreadcrumbService : IBreadcrumbService
    {
        public void AddAdminBreadcrumbs(IBreadcrumb model, string seasonId, int seasonYear)
        {
            string seasonYearString = seasonYear.ToString();
            model.BreadcrumbList.Clear();

            if (!model.BreadcrumbList.Any(b => b.Text == "Admin"))
            {
                model.BreadcrumbList.Add(new BreadcrumbItemModel
                {
                    Text = "Admin",
                    Url = "/Admin",
                    IsActive = false,
                });
            }

            if (!model.BreadcrumbList.Any(b => b.Text == "Seasons"))
            {
                model.BreadcrumbList.Add(new BreadcrumbItemModel
                {
                    Text = "Manage Seasons",
                    Url = "/Admin/Season",
                    IsActive = false,
                });
            }

            if (!model.BreadcrumbList.Any(b => b.Text == seasonYearString))
            {
                model.BreadcrumbList.Add(new BreadcrumbItemModel
                {
                    Text = seasonYearString,
                    Url = $"/Admin/Season/Details/{seasonId}",
                    IsActive = false,
                });
            }

            if (!model.BreadcrumbList.Any(b => b.Text == "Brackets"))
            {
                model.BreadcrumbList.Add(new BreadcrumbItemModel
                {
                    Text = "Manage Brackets",
                    IsActive = true,
                });
            }
        }
    }
}
