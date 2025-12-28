// <copyright file="ModelExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;

    public static class ModelExtensions
    {
        public static List<UserModel> AsUserModelList(this List<User> userList)
        {
            List<UserModel> userModels = new List<UserModel>();

            foreach (var item in userList)
            {
                userModels.Add(item.AsUserModel());
            }

            return userModels;
        }

        public static UserModel AsUserModel(this User user)
        {
            var userModel = new UserModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id  = user.Id.ToString(),
            };

            userModel.RoleIds.AddRange(user.Roles.Select(x => ((int)x).ToString()));

            return userModel;
        }
    }
}
