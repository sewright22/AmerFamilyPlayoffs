using System.Linq.Expressions;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Extensions;
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

            var afcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("NFC");

            model.Name = "Test";
            model.CanEdit = true;

            model.Rounds = this.Context.Rounds.AsNoTracking().ToList();

            model.AfcWildCardGame1 = new Models.Bracket.Matchup
            {
                Name = nameof(model.AfcWildCardGame1),
                HomeTeam = afcTeams.GetTeamFromSeed(4),
                AwayTeam = afcTeams.GetTeamFromSeed(5),
            };

            model.NfcWildCardGame1 = new Models.Bracket.Matchup
            {
                Name = nameof(model.NfcWildCardGame1),
                HomeTeam = nfcTeams.GetTeamFromSeed(4),
                AwayTeam = nfcTeams.GetTeamFromSeed(5),
            };

            model.AfcWildCardGame2 = new Models.Bracket.Matchup
            {
                Name = nameof(model.AfcWildCardGame2),
                HomeTeam = afcTeams.GetTeamFromSeed(3),
                AwayTeam = afcTeams.GetTeamFromSeed(6),
            };

            model.NfcWildCardGame2 = new Models.Bracket.Matchup
            {
                Name = nameof(model.NfcWildCardGame2),
                HomeTeam = nfcTeams.GetTeamFromSeed(3),
                AwayTeam = nfcTeams.GetTeamFromSeed(6),
            };

            model.AfcWildCardGame3 = new Models.Bracket.Matchup
            {
                Name = nameof(model.AfcWildCardGame3),
                HomeTeam = afcTeams.GetTeamFromSeed(2),
                AwayTeam = afcTeams.GetTeamFromSeed(7),
            };

            model.NfcWildCardGame3 = new Models.Bracket.Matchup
            {
                Name = nameof(model.NfcWildCardGame3),
                HomeTeam = nfcTeams.GetTeamFromSeed(2),
                AwayTeam = nfcTeams.GetTeamFromSeed(7),
            };

            return View(model);
        }

        public IActionResult Submit(BracketViewModel BracketViewModel)
        {
            return View(BracketViewModel);
        }
    }
}
