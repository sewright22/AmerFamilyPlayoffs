// <copyright file="AdminUsersViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.ViewModels
{
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models;

    public class AdminUsersViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
    }
}
