// <copyright file="BreadcrumbItemModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.ViewModels
{
    public class BreadcrumbItemModel
    {
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}