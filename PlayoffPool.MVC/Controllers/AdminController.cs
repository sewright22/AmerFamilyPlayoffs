﻿namespace PlayoffPool.MVC.Controllers
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using PlayoffPool.MVC.Helpers;
    using PlayoffPool.MVC.Models;
    using System;

    public class AdminController : Controller
    {
        public AdminController(ILogger<AdminController> logger, IDataManager dataManager)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Logger = logger;
            this.DataManager = dataManager;
        }

        public ILogger<AdminController> Logger { get; }
        public IDataManager DataManager { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            await this.Seed().ConfigureAwait(false);
            var model = new AdminViewModel();
            model.ManageUsersViewModel = new ManageUsersViewModel();
            model.ManageRolesViewModel = new ManageRolesViewModel();

            var users = await this.DataManager.DataContext.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);
            var roles = this.DataManager.RoleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();

            foreach (var role in roles)
            {
                model.ManageRolesViewModel.Roles.Add(new RoleModel
                {
                    Id = role.Value,
                    Name = role.Text,
                });
            }

            foreach (var user in users)
            {
                var userRoles = await this.DataManager.UserManager.GetRolesAsync(user).ConfigureAwait(false);

                model.ManageUsersViewModel.Users.Add(new Models.UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles,
                    RoleId = roles.Where(x => userRoles.Contains(x.Text)).Select(x => x.Value).FirstOrDefault(),
                });
            }

            return this.View(model);
        }

        private Task Seed()
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUsers(UserModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            try
            {
                var dbUser = await this.DataManager.UserManager.FindByIdAsync(model.Id).ConfigureAwait(false);

                if (dbUser == null)
                {
                    return this.View(model);
                }

                dbUser.FirstName = model.FirstName;
                dbUser.LastName = model.LastName;
                dbUser.Email = model.Email;

                await this.DataManager.UserManager.UpdateAsync(dbUser).ConfigureAwait(false);

                var userRoles = await this.DataManager.UserManager.GetRolesAsync(dbUser).ConfigureAwait(false);

                if (userRoles.Contains(model.RoleId))
                {
                    return this.View(model);
                }

                if (userRoles.Any())
                {
                    var result = await this.DataManager.UserManager.RemoveFromRoleAsync(dbUser, userRoles.First()).ConfigureAwait(false);
                }

                var newRole = await this.DataManager.RoleManager.FindByIdAsync(model.RoleId).ConfigureAwait(false);

                await this.DataManager.UserManager.AddToRoleAsync(dbUser, newRole.Name).ConfigureAwait(false);
                var claims = await this.DataManager.UserManager.GetClaimsAsync(dbUser).ConfigureAwait(false);
                var test = claims;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateRoles(RoleModel model)
        {
            return this.View(model);
        }
    }
}