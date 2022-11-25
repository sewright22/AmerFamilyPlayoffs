using AmerFamilyPlayoffs.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            AmerFamilyPlayoffContext context)
        {
            this.Mapper = mapper;
            this.UserManager = userManager;
            SignInManager = signInManager;
            Logger = logger;
            Context = context;
            roleManager.CreateAsync(new IdentityRole("Test"));
        }

        public IMapper Mapper { get; }
        public UserManager<User> UserManager { get; }
        public SignInManager<User> SignInManager { get; }
        public ILogger<AccountController> Logger { get; }
        public AmerFamilyPlayoffContext Context { get; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
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

            // var user = await this.UserManager.FindByIdAsync(model.Email).ConfigureAwait(false);

            if (user is not null && await this.UserManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                try
                {
                    await this.SignInManager.SignInAsync(user, true).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    this.Logger.LogError(e, "Failed to login.");
                    return this.View(model);
                }
            }


            return RedirectToAction(nameof(HomeController.Index), "Home");

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

            var result = await this.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(model);
            }

            await this.UserManager.AddToRoleAsync(user, "Test").ConfigureAwait(false);

            return RedirectToAction(nameof(HomeController.Index), "Home");

        }
    }
}
