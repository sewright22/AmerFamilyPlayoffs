// <copyright file="UserModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NflPlayoffPool.Web.Extensions;

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    public class UserModel
    {
        [Required]
        public string Id { get; set; } = "New_User";
        public List<string> RoleIds { get; set; } = new List<string>();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public string? FullName
        {
            get
            {
                if (this.FirstName.HasValue() == false && this.LastName.HasValue() == false)
                {
                    return "Unknown";
                }

                return $"{this.FirstName} {this.LastName}".Trim();
            }
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public string? Role { get; set; }

        [Display(Name = "Reset Password")]
        public bool ShouldResetPassword { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
