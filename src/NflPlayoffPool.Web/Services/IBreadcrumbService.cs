// <copyright file="IBreadcrumbService.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Services
{
    using NflPlayoffPool.Web.ViewModels;

    public interface IBreadcrumbService
    {
        void AddAdminBreadcrumbs(IBreadcrumb model, string seasonId, int seasonYear);
    }
}