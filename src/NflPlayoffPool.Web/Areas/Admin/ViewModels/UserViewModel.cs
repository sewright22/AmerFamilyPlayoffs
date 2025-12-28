// <copyright file="UserViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using System;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models;

    public class UserViewModel : IModal
    {
        public required UserModel UserModel { get; set; }

        public string? Title
        {
            get
            {
                return "Update User";
            }
            set
            {

            }
        }
    }
}
