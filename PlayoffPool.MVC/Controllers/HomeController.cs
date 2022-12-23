using System.Diagnostics;
using AmerFamilyPlayoffs.Data;
using AmerFamilyPlayoffs.Data.SeedExtensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Extensions;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AmerFamilyPlayoffContext dataContext;
        private readonly SignInManager<User> signInManager;

        public IMapper Mapper { get; }

        public HomeController(ILogger<HomeController> logger, AmerFamilyPlayoffContext dataContext, SignInManager<User> signInManager, IMapper mapper)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            Mapper = mapper;
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
                return this.RedirectToAction(Constants.Actions.LOGIN, Constants.Controllers.ACCOUNT);
            }

            var model = new HomeViewModel();
            model.Brackets = this.dataContext.BracketPredictions.ProjectTo<BracketSummaryModel>(this.Mapper.ConfigurationProvider).ToList();
            return View(model);
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

        public async Task<IActionResult> LogOut()
        {
            await this.signInManager.SignOutAsync().ConfigureAwait(false);
            return this.RedirectToAction(Constants.Actions.LOGIN, Constants.Controllers.ACCOUNT);
        }
    }
}