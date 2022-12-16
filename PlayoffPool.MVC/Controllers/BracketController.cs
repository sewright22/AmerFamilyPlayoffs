using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Models.Bracket;

namespace PlayoffPool.MVC.Controllers
{
    public class BracketController : Controller
    {
        public BracketController(AmerFamilyPlayoffContext amerFamilyPlayoffContext)
        {
            this.Context = amerFamilyPlayoffContext;
        }

        public AmerFamilyPlayoffContext Context { get; }

        [HttpGet]
        public IActionResult Index(int? bracketId)
        {
            var model = new BracketViewModel();

            if (bracketId is not null)
            {
                // get bracket info from database.
            }

            model.Name = "Test";
            model.CanEdit = true;

            model.Rounds = this.Context.Rounds.AsNoTracking().ToList();

            model.AfcRound1Game1 = new Models.Bracket.Matchup
            {
                Name = "AfcRound1Game1",
                HomeTeam = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FirstOrDefault(x => x.Seed == 4).SeasonTeam.Team,
                AwayTeam = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FirstOrDefault(x => x.Seed == 5).SeasonTeam.Team,
            };

            return View(model);
        }
    }
}
