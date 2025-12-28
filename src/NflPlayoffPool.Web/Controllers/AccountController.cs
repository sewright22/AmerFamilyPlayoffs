// <copyright file="AccountController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NflPlayoffPool.Data;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.Web.Extensions;
using NflPlayoffPool.Web.Models;

namespace NflPlayoffPool.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public PlayoffPoolContext DbContext { get; }

        public AccountController(ILogger<AccountController> logger, PlayoffPoolContext dbContext)
        {
            _logger = logger;
            this.DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            var viewModel = new LoginViewModel { ReturnUrl = returnUrl };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            User? user = this.DbContext.ValidateUser(model);

            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, ((Role)role).ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = model.ReturnUrl
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return this.LocalRedirect(model.ReturnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
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

            var user = this.DbContext.CreateUser(model);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "User with this email already exists.");

                return View(model);
            }

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).GetControllerNameForUri());

        }
    }
}