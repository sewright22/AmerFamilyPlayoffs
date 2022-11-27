namespace PlayoffPool.MVC.Controllers
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using PlayoffPool.MVC.Models;
    using System;

    public class AdminController : Controller
    {
        public AdminController(ILogger<AdminController> logger, AmerFamilyPlayoffContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Logger = logger;
            Context = context ?? throw new ArgumentNullException(nameof(context));
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public ILogger<AdminController> Logger { get; }
        public AmerFamilyPlayoffContext Context { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public UserManager<User> UserManager { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel();
            model.ManageUsersViewModel = new ManageUsersViewModel();
            model.ManageRolesViewModel = new ManageRolesViewModel();

            var users = await this.Context.Users.ToListAsync().ConfigureAwait(false);
            var roles = this.RoleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();

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
                var userRoles = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);

                model.ManageUsersViewModel.Users.Add(new Models.UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles,
                    RoleId = roles.Where(x=>userRoles.Contains(x.Text)).Select(x=>x.Value).FirstOrDefault(),
                });
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUsers(UserModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            var user = model;

            //foreach (var user in model.Users)
            //{
            try
            {
                var dbUser = await this.Context.Users.SingleOrDefaultAsync(x => x.Id == user.Id).ConfigureAwait(false);

                if (dbUser == null)
                {
                    return this.View(model);
                }

                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.Email = user.Email;

                await this.Context.SaveChangesAsync().ConfigureAwait(false);

                var userRoles = await this.UserManager.GetRolesAsync(dbUser).ConfigureAwait(false);

                if (userRoles.Contains(user.RoleId))
                {
                    return this.View(model);
                }

                var result = await this.UserManager.RemoveFromRoleAsync(dbUser, userRoles.First()).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await this.UserManager.AddToRoleAsync(dbUser, user.RoleId).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
            }
            //}

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
