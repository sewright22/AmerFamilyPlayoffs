// <copyright file="IBreadcrumb.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.ViewModels
{
    public interface IBreadcrumb
    {
        List<BreadcrumbItemModel> BreadcrumbList { get; }
    }
}
