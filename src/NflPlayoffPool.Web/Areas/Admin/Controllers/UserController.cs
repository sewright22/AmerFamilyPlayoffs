// <copyright file="UserController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Areas.Admin.ViewModels;
    using NflPlayoffPool.Web.Controllers;
    using NflPlayoffPool.Web.Extensions;

    [Area("Admin")]
    public class UserController : Controller
    {

        public PlayoffPoolContext DbContext { get; }
        public UserController(ILogger<UserController> logger, PlayoffPoolContext dbContext)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;
            this.DbContext = dbContext;
        }

        public ILogger<UserController> Logger { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = new UsersModel();

            var users = await this.DbContext.Users
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync().ConfigureAwait(false);

            model.Users.AddRange(users.AsUserModelList());

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(string? id)
        {
            UserModel? model = this.DbContext.Users.FirstOrDefault(x => x.Id.ToString() == id)?.AsUserModel();

            if ( model == null)
            {
                return this.NotFound();
            }

            model.Roles.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(Role.Player.ToString(), ((int)Role.Player).ToString()));
            model.Roles.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(Role.Admin.ToString(), ((int)Role.Admin).ToString()));

            UserViewModel viewModel = GenerateViewModel(model);

            return this.PartialView(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            if (ModelState.IsValid == false)
            {
                return this.View(GenerateViewModel(model));
            }

            await this.DbContext.UpdateUser(model);
            await this.DbContext.SaveChangesAsync().ConfigureAwait(false);
            return this.RedirectToAction(nameof(this.Index));
        }

        private static UserViewModel GenerateViewModel(UserModel model)
        {
            UserViewModel viewModel = new UserViewModel()
            {
                UserModel = model,
            };

            return viewModel;
        }
    }
}
