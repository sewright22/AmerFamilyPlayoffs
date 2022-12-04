using AmerFamilyPlayoffs.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Helpers;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(
            IMapper mapper,
            ILogger<AccountController> logger,
            IDataManager dataManager)
        {
            this.Mapper = mapper;
            this.Logger = logger;
            this.DataManager = dataManager;
            this.Context = dataManager.DataContext;
        }

        public IMapper Mapper { get; }
        public ILogger<AccountController> Logger { get; }
        public IDataManager DataManager { get; }
        public AmerFamilyPlayoffContext Context { get; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await this.DataManager.Seed().ConfigureAwait(false);
            return this.View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await this.Context.Users.SingleOrDefaultAsync(x => x.Email == model.Email).ConfigureAwait(false);

            if (user is not null && await this.DataManager.UserManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                try
                {
                    await this.DataManager.SignInManager.SignInAsync(user, true).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    this.Logger.LogError(e, "Failed to login.");
                    return this.View(model);
                }
            }


            return RedirectToAction(nameof(HomeController.Index), Constants.Controllers.HOME);

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = this.Mapper.Map<User>(model);

            var result = await this.DataManager.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(model);
            }

            await this.DataManager.UserManager.AddToRoleAsync(user, Constants.Roles.Player).ConfigureAwait(false);

            return RedirectToAction(nameof(HomeController.Index), Constants.Controllers.HOME);

        }
    }
}
