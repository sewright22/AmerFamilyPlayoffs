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
using PlayoffPool.MVC.Models.Bracket;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AmerFamilyPlayoffContext dataContext;
        private readonly SignInManager<User> signInManager;

        public IMapper Mapper { get; }
        public UserManager<User> UserManager { get; }

        public HomeController(ILogger<HomeController> logger, AmerFamilyPlayoffContext dataContext, SignInManager<User> signInManager, IMapper mapper, UserManager<User> userManager)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            Mapper = mapper;
            UserManager = userManager;
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

            foreach (var completedBracket in this.dataContext.BracketPredictions.Include("MatchupPredictions.PlayoffRound.Round").Include("MatchupPredictions.PredictedWinner.SeasonTeam.Team").AsNoTracking().Where(x => x.UserId == this.UserManager.GetUserId(this.User)).Where(x => x.MatchupPredictions.Count(x => x.PredictedWinner != null) == 13))
            {
                var teamName = $"{completedBracket.SuperBowl.PredictedWinner.SeasonTeam.Team.Location} {completedBracket.SuperBowl.PredictedWinner.SeasonTeam.Team.Name}";
                model.CompletedBrackets.Add(new BracketSummaryModel
                {
                    Id = completedBracket.Id,
                    Name= completedBracket.Name,
                    PredictedWinner = new TeamViewModel()
                    {
                        Name= completedBracket.SuperBowl.PredictedWinner.SeasonTeam.Team.Name,
                    }
                });
            }

            model.IncompleteBrackets = this.dataContext.BracketPredictions.AsNoTracking().Where(x => x.UserId == this.UserManager.GetUserId(this.User)).Where(x => x.MatchupPredictions == null || x.MatchupPredictions.Count(x => x.PredictedWinner != null) < 13).ProjectTo<BracketSummaryModel>(this.Mapper.ConfigurationProvider).ToList();
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