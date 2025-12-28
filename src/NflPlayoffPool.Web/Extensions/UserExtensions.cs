// <copyright file="UserExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using System.Security.Claims;
    using Microsoft.EntityFrameworkCore;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;

    public static class UserExtensions
    {
        public static async Task UpdateUser(this PlayoffPoolContext dbContext, UserModel model)
        {
            User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id.ToString() == model.Id).ConfigureAwait(false);

            if (user == null)
            {
                return;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Roles.Clear();

            foreach (var roleId in model.RoleIds)
            {
                if (Enum.TryParse<Role>(roleId, out var role))
                {
                    user.Roles.Add(role);
                }
            }

            dbContext.Users.Update(user);
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
