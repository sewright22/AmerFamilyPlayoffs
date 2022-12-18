using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Extensions;
using PlayoffPool.MVC.Models.Bracket;
using Microsoft.AspNetCore.Authorization;

namespace PlayoffPool.MVC.Controllers
{
    public class BracketController : Controller
    {
        public BracketController(AmerFamilyPlayoffContext amerFamilyPlayoffContext, IMapper mapper)
        {
            this.Context = amerFamilyPlayoffContext;
            this.Mapper = mapper;
        }

        public AmerFamilyPlayoffContext Context { get; }
        public IMapper Mapper { get; }

        [HttpGet]
        public IActionResult Create()
        {
            var BracketViewModel = new BracketViewModel();

            var afcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("NFC");

            BracketViewModel.Name = "Test";
            BracketViewModel.CanEdit = true;

            BracketViewModel.AfcWildCardGame1 = this.BuildMatchup(nameof(BracketViewModel.AfcWildCardGame1), afcTeams, 4, 5);
            BracketViewModel.AfcWildCardGame2 = this.BuildMatchup(nameof(BracketViewModel.AfcWildCardGame2), afcTeams, 3, 6);
            BracketViewModel.AfcWildCardGame3 = this.BuildMatchup(nameof(BracketViewModel.AfcWildCardGame3), afcTeams, 2, 7);

            BracketViewModel.NfcWildCardGame1 = this.BuildMatchup(nameof(BracketViewModel.NfcWildCardGame1), nfcTeams, 4, 5);
            BracketViewModel.NfcWildCardGame2 = this.BuildMatchup(nameof(BracketViewModel.NfcWildCardGame2), nfcTeams, 3, 6);
            BracketViewModel.NfcWildCardGame3 = this.BuildMatchup(nameof(BracketViewModel.NfcWildCardGame3), nfcTeams, 2, 7);

            //model.NfcWildCardGame1 = new Models.Bracket.Matchup
            //{
            //    Name = nameof(model.NfcWildCardGame1),
            //    HomeTeam = nfcTeams.GetTeamFromSeed(4),
            //    AwayTeam = nfcTeams.GetTeamFromSeed(5),
            //};

            //model.AfcWildCardGame2 = new Models.Bracket.Matchup
            //{
            //    Name = nameof(model.AfcWildCardGame2),
            //    HomeTeam = afcTeams.GetTeamFromSeed(3),
            //    AwayTeam = afcTeams.GetTeamFromSeed(6),
            //};

            //model.NfcWildCardGame2 = new Models.Bracket.Matchup
            //{
            //    Name = nameof(model.NfcWildCardGame2),
            //    HomeTeam = nfcTeams.GetTeamFromSeed(3),
            //    AwayTeam = nfcTeams.GetTeamFromSeed(6),
            //};

            //model.AfcWildCardGame3 = new Models.Bracket.Matchup
            //{
            //    Name = nameof(model.AfcWildCardGame3),
            //    HomeTeam = afcTeams.GetTeamFromSeed(2),
            //    AwayTeam = afcTeams.GetTeamFromSeed(7),
            //};

            //model.NfcWildCardGame3 = new Models.Bracket.Matchup
            //{
            //    Name = nameof(model.NfcWildCardGame3),
            //    HomeTeam = nfcTeams.GetTeamFromSeed(2),
            //    AwayTeam = nfcTeams.GetTeamFromSeed(7),
            //};

            return View(BracketViewModel);
        }

        private MatchupViewModel BuildMatchup(string name, IQueryable<PlayoffTeam> teams, int seed1, int seed2)
        {
            return new MatchupViewModel
            {
                Name = name,
                HomeTeam = this.Mapper.Map<TeamViewModel>(teams.GetTeamFromSeed(Math.Min(seed1, seed2))),
                AwayTeam = this.Mapper.Map<TeamViewModel>(teams.GetTeamFromSeed(Math.Max(seed1, seed2))),
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(BracketViewModel BracketViewModel)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(BracketViewModel);
            }

            return View(BracketViewModel);
        }
    }
}
