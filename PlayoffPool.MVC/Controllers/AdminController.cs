namespace PlayoffPool.MVC.Controllers
{
	using AmerFamilyPlayoffs.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using PlayoffPool.MVC.Models;
    using PlayoffPool.MVC.Models.Admin;
    using System;

    public class AdminController : Controller
	{
		public AdminController(ILogger<AdminController> logger, AmerFamilyPlayoffContext context, RoleManager<IdentityRole> roleManager)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Logger = logger;
            Context = context ?? throw new ArgumentNullException(nameof(context));
            RoleManager = roleManager;
        }

        public ILogger<AdminController> Logger { get; }
        public AmerFamilyPlayoffContext Context { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
		{
            var model = new AdminViewModel();
            model.ManageUsersViewModel = new ManageUsersViewModel();

            var users = await this.Context.Users.ToListAsync().ConfigureAwait(false);
            var roles = this.RoleManager.Roles.Select(x => new SelectListItem(x.Id, x.Name)).ToList();

            foreach (var user in users)
            {
                model.ManageUsersViewModel.Users.Add(new Models.UserModel
                {
                    Email = user.Email,
                    FirstName= user.FirstName,
                    LastName= user.LastName,
                    Roles = roles,
                });
            }

            return this.View(model);
        }
	}
}
