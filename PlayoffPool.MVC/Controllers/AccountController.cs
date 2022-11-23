﻿using AmerFamilyPlayoffs.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.Mapper = mapper;
            this.UserManager = userManager;
            roleManager.CreateAsync(new IdentityRole("Test"));
        }

        public IMapper Mapper { get; }
        public UserManager<User> UserManager { get; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegistrationUserViewModel model)
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

            await this.UserManager.AddToRoleAsync(user, "Test");

            return RedirectToAction(nameof(HomeController.Index), "Home");

        }
    }
}
