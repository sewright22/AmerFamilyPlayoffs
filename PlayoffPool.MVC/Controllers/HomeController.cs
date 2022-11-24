using System.Diagnostics;
using AmerFamilyPlayoffs.Data;
using AmerFamilyPlayoffs.Data.SeedExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Extensions;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AmerFamilyPlayoffContext dataContext;
        private readonly SignInManager<User> signInManager;

        public HomeController(ILogger<HomeController> logger, AmerFamilyPlayoffContext dataContext, SignInManager<User> signInManager)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            SetupDatabase();
        }

        private void SetupDatabase()
        {
            try
            {
                this.dataContext.Database.Migrate();
                this.dataContext.SeedData();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to setup database.");
            }
        }

        public IActionResult Index()
        {
            if (this.signInManager.IsSignedIn(this.User) == false)
            {
                return this.Redirect($"../{Constants.Controllers.ACCOUNT}/{Constants.Actions.LOGIN}");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}