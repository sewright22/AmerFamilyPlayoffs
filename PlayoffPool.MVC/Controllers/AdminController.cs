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

            var users = await this.Context.Users.ToListAsync().ConfigureAwait(false);
            var roles = this.RoleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
            foreach (var user in users)
            {
                var userRoles = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);

                model.ManageUsersViewModel.Users.Add(new Models.UserModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles,
                    RoleId = roles.Where(x=>userRoles.Contains(x.Text)).Select(x=>x.Value).FirstOrDefault(),
                });
            }

            return this.View(model);
        }
    }
}
