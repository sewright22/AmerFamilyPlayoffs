namespace PlayoffPool.MVC.Controllers
{
	using AmerFamilyPlayoffs.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PlayoffPool.MVC.Models;
    using PlayoffPool.MVC.Models.Admin;
    using System;

    public class AdminController : Controller
	{
		public AdminController(ILogger<AdminController> logger, AmerFamilyPlayoffContext context)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Logger = logger;
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ILogger<AdminController> Logger { get; }
        public AmerFamilyPlayoffContext Context { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
		{
            var model = new AdminViewModel();
            model.ManageUsersViewModel = new ManageUsersViewModel();

            var users = await this.Context.Users.ToListAsync().ConfigureAwait(false);

            foreach (var user in users)
            {
                model.ManageUsersViewModel.Users.Add(new Models.Admin.UserModel
                {
                    Email = user.Email,
                    FirstName= user.FirstName,
                    LastName= user.LastName,
                });
            }

            return this.View(model);
        }
	}
}
