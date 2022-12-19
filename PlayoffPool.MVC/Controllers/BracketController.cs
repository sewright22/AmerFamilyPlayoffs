﻿using System.Linq.Expressions;
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

            var afcWildcardRound = new RoundViewModel
            {
                RoundNumber = 1,
                Conference = "AFC",
            };

            var nfcWildcardRound = new RoundViewModel
            {
                RoundNumber = 1,
                Conference = "NFC",
            };

            afcWildcardRound.Games.Add(this.BuildMatchup("AFC Wildcard Game 1", afcTeams, 1, 4, 5));
            afcWildcardRound.Games.Add(this.BuildMatchup("AFC Wildcard Game 2", afcTeams, 2, 3, 6));
            afcWildcardRound.Games.Add(this.BuildMatchup("AFC Wildcard Game 3", afcTeams, 3, 2, 7));

            nfcWildcardRound.Games.Add(this.BuildMatchup("NFC Wildcard Game 1", nfcTeams, 1, 4, 5));
            nfcWildcardRound.Games.Add(this.BuildMatchup("NFC Wildcard Game 2", nfcTeams, 2, 3, 6));
            nfcWildcardRound.Games.Add(this.BuildMatchup("NFC Wildcard Game 3", nfcTeams, 3, 2, 7));

            BracketViewModel.AfcRounds.Add(afcWildcardRound);
            BracketViewModel.NfcRounds.Add(nfcWildcardRound);

            return View(BracketViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(BracketViewModel BracketViewModel)
        {
            if (this.ModelState.IsValid)
            {
                var afcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("AFC");
                var nfcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("NFC");
                var afcRounds = new List<RoundViewModel>(BracketViewModel.AfcRounds);
                var nfcRounds = new List<RoundViewModel>(BracketViewModel.NfcRounds);

                if (afcRounds.Max(x => x.RoundNumber) == 1)
                {
                    var afcDivisionalRound = new RoundViewModel
                    {
                        RoundNumber = 2,
                        Conference = "AFC",
                    };

                    List<MatchupViewModel> afcWildcardGames = afcRounds.Single(x => x.RoundNumber == 1).Games.ToList();

                    var pickedAfcWinners = this.GetWinners(afcWildcardGames).OrderByDescending(x => x.Seed).ToList();

                    afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 1", afcTeams, 1, 1, pickedAfcWinners[0].Seed));
                    afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 2", afcTeams, 1, pickedAfcWinners[2].Seed, pickedAfcWinners[1].Seed));
                    BracketViewModel.AfcRounds.Add(afcDivisionalRound);
                }

                if (nfcRounds.Max(x => x.RoundNumber) == 1)
                {
                    var nfcDivisionalRound = new RoundViewModel
                    {
                        RoundNumber = 2,
                        Conference = "AFC",
                    };
                    List<MatchupViewModel> nfcWildcardGames = nfcRounds.Single(x => x.RoundNumber == 1).Games.ToList();
                    var pickedNfcWinners = this.GetWinners(nfcWildcardGames).OrderByDescending(x => x.Seed).ToList();
                    nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 1", nfcTeams, 1, 1, pickedNfcWinners[0].Seed));
                    nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 2", nfcTeams, 1, pickedNfcWinners[2].Seed, pickedNfcWinners[1].Seed));
                    BracketViewModel.NfcRounds.Add(nfcDivisionalRound);
                }

                if (afcRounds.Max(x => x.RoundNumber) == 2)
                {
                    var afcChampionship = new RoundViewModel
                    {
                        RoundNumber = 3,
                        Conference = "AFC",
                    };

                    List<MatchupViewModel> afcDivisionalGames = afcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                    var pickedAfcWinners = this.GetWinners(afcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                    afcChampionship.Games.Add(this.BuildMatchup("AFC Championship Game", afcTeams, 1, pickedAfcWinners[1].Seed, pickedAfcWinners[0].Seed));
                    BracketViewModel.AfcRounds.Add(afcChampionship);
                }

                if (nfcRounds.Max(x => x.RoundNumber) == 2)
                {
                    var nfcChampionship = new RoundViewModel
                    {
                        RoundNumber = 3,
                        Conference = "NFC",
                    };
                    List<MatchupViewModel> nfcDivisionalGames = nfcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                    var pickedNfcWinners = this.GetWinners(nfcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                    nfcChampionship.Games.Add(this.BuildMatchup("NFC Championship Game", nfcTeams, 1, pickedNfcWinners[1].Seed, pickedNfcWinners[0].Seed));
                    BracketViewModel.NfcRounds.Add(nfcChampionship);
                }

                if (afcRounds.Max(x => x.RoundNumber) == 3 && nfcRounds.Max(x => x.RoundNumber) == 3 && BracketViewModel.SuperBowl == null)
                {
                    List<MatchupViewModel> afcChampionshipGame = afcRounds.Single(x => x.RoundNumber == 3).Games.ToList();
                    List<MatchupViewModel> nfcChampionshipGame = nfcRounds.Single(x => x.RoundNumber == 3).Games.ToList();
                    var pickedAfcWinner = this.GetWinners(afcChampionshipGame).Single().Seed;
                    var pickedNfcWinner = this.GetWinners(nfcChampionshipGame).Single().Seed;
                    var homeTeam = this.Mapper.Map<TeamViewModel>(afcTeams.GetTeamFromSeed(pickedAfcWinner));
                    var awayTeam = this.Mapper.Map<TeamViewModel>(nfcTeams.GetTeamFromSeed(pickedNfcWinner));
                    BracketViewModel.SuperBowl = new MatchupViewModel
                    {
                        Name = "Super Bowl",
                        GameNumber = 1,
                        HomeTeam = homeTeam,
                        AwayTeam = awayTeam,
                    };
                }

                if (BracketViewModel.SuperBowl is not null && BracketViewModel.SuperBowl.SelectedWinner.HasValue)
                {
                    // Save to database.
                    if (BracketViewModel.Id == 0)
                    {
                        BracketPrediction prediction = this.Mapper.Map<BracketPrediction>(BracketViewModel);
                        prediction.Playoff = this.Context.Playoffs.FirstOrDefault(x => x.Season.Year == 2021);
                        prediction.MatchupPredictions = new List<MatchupPrediction>();
                        prediction.MatchupPredictions.Add(this.Mapper.Map<MatchupPrediction>(BracketViewModel.AfcRounds[0].Games[0]));
                        this.Context.Add(prediction);
                        this.Context.SaveChanges();
                        var test = prediction;
                    }
                }
            }

            return this.View(BracketViewModel);
        }

        private MatchupViewModel BuildMatchup(string name, IQueryable<PlayoffTeam> teams, int gameNumber, int seed1, int seed2)
        {
            return new MatchupViewModel
            {
                GameNumber = gameNumber,
                Name = name,
                HomeTeam = this.Mapper.Map<TeamViewModel>(teams.GetTeamFromSeed(Math.Min(seed1, seed2))),
                AwayTeam = this.Mapper.Map<TeamViewModel>(teams.GetTeamFromSeed(Math.Max(seed1, seed2))),
            };
        }

        private List<TeamViewModel> GetWinners(List<MatchupViewModel> games)
        {
            var winningIds = games.Select(x => x.SelectedWinner.Value).ToList();

            return games.Select(x => winningIds.Contains(x.HomeTeam.Id) ? x.HomeTeam : x.AwayTeam).ToList();
        }
    }
}
