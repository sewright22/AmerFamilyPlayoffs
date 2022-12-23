using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Extensions;
using PlayoffPool.MVC.Models.Bracket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PlayoffPool.MVC.Controllers
{
    public class BracketController : Controller
    {
        public BracketController(AmerFamilyPlayoffContext amerFamilyPlayoffContext, IMapper mapper, UserManager<User> userManager)
        {
            this.Context = amerFamilyPlayoffContext;
            this.Mapper = mapper;
            UserManager = userManager;
        }

        public AmerFamilyPlayoffContext Context { get; }
        public IMapper Mapper { get; }
        public UserManager<User> UserManager { get; }

        [HttpGet]
        public IActionResult Create()
        {
            var BracketViewModel = new BracketViewModel();

            var afcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("NFC");

            BracketViewModel.Name = "";
            BracketViewModel.CanEdit = true;

            var afcWildcardRound = this.Context.PlayoffRounds.Where(x => x.Round.Number == 1).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
            afcWildcardRound.Conference = "AFC";

            var nfcWildcardRound = this.Context.PlayoffRounds.Where(x => x.Round.Number == 1).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
            nfcWildcardRound.Conference = "NFC";

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
                var afcTeams = this.Context.PlayoffTeams.Include("SeasonTeam.Team").FilterConference("AFC");
                var nfcTeams = this.Context.PlayoffTeams.Include("SeasonTeam.Team").FilterConference("NFC");
                var afcRounds = new List<RoundViewModel>(BracketViewModel.AfcRounds);
                var nfcRounds = new List<RoundViewModel>(BracketViewModel.NfcRounds);

                if (afcRounds.Max(x => x.RoundNumber) == 1)
                {
                    foreach (var round in BracketViewModel.AfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    var afcDivisionalRound = this.Context.PlayoffRounds.Where(x => x.Round.Number == 2).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
                    afcDivisionalRound.Conference = "AFC";
                    afcDivisionalRound.IsLocked = false;

                    List<MatchupViewModel> afcWildcardGames = afcRounds.Single(x => x.RoundNumber == 1).Games.ToList();

                    var pickedAfcWinners = this.GetWinners(afcWildcardGames).OrderByDescending(x => x.Seed).ToList();

                    afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 1", afcTeams, 1, 1, pickedAfcWinners[0].Seed));
                    afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 2", afcTeams, 1, pickedAfcWinners[2].Seed, pickedAfcWinners[1].Seed));
                    BracketViewModel.AfcRounds.Add(afcDivisionalRound);
                }

                if (nfcRounds.Max(x => x.RoundNumber) == 1)
                {
                    foreach (var round in BracketViewModel.NfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    var nfcDivisionalRound = this.Context.PlayoffRounds.Where(x => x.Round.Number == 2).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
                    nfcDivisionalRound.Conference = "NFC";
                    nfcDivisionalRound.IsLocked = false;

                    List<MatchupViewModel> nfcWildcardGames = nfcRounds.Single(x => x.RoundNumber == 1).Games.ToList();
                    var pickedNfcWinners = this.GetWinners(nfcWildcardGames).OrderByDescending(x => x.Seed).ToList();
                    nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 1", nfcTeams, 1, 1, pickedNfcWinners[0].Seed));
                    nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 2", nfcTeams, 1, pickedNfcWinners[2].Seed, pickedNfcWinners[1].Seed));
                    BracketViewModel.NfcRounds.Add(nfcDivisionalRound);
                }

                if (afcRounds.Max(x => x.RoundNumber) == 2)
                {
                    // Lock previous rounds
                    foreach (var round in BracketViewModel.AfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    var afcChampionship = this.Context.PlayoffRounds.Where(x => x.Round.Number == 3).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
                    afcChampionship.Conference = "AFC";
                    afcChampionship.IsLocked = false;

                    List<MatchupViewModel> afcDivisionalGames = afcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                    var pickedAfcWinners = this.GetWinners(afcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                    afcChampionship.Games.Add(this.BuildMatchup("AFC Championship Game", afcTeams, 1, pickedAfcWinners[1].Seed, pickedAfcWinners[0].Seed));
                    BracketViewModel.AfcRounds.Add(afcChampionship);
                }

                if (nfcRounds.Max(x => x.RoundNumber) == 2)
                {
                    // Lock previous rounds
                    foreach (var round in BracketViewModel.NfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    var nfcChampionship = this.Context.PlayoffRounds.Where(x => x.Round.Number == 3).ProjectTo<RoundViewModel>(this.Mapper.ConfigurationProvider).FirstOrDefault();
                    nfcChampionship.Conference = "NFC";
                    nfcChampionship.IsLocked = false;

                    List<MatchupViewModel> nfcDivisionalGames = nfcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                    var pickedNfcWinners = this.GetWinners(nfcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                    nfcChampionship.Games.Add(this.BuildMatchup("NFC Championship Game", nfcTeams, 1, pickedNfcWinners[1].Seed, pickedNfcWinners[0].Seed));
                    BracketViewModel.NfcRounds.Add(nfcChampionship);
                }

                if (afcRounds.Max(x => x.RoundNumber) == 3 && nfcRounds.Max(x => x.RoundNumber) == 3 && BracketViewModel.SuperBowl == null)
                {
                    // Lock previous rounds
                    foreach (var round in BracketViewModel.AfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    foreach (var round in BracketViewModel.NfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

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
                    // Lock previous rounds
                    foreach (var round in BracketViewModel.AfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    foreach (var round in BracketViewModel.NfcRounds)
                    {
                        round.IsLocked = true;

                        foreach (var game in round.Games)
                        {
                            game.IsLocked = true;
                        }
                    }

                    // Save to database.
                    if (BracketViewModel.Id == 0)
                    {
                        BracketPrediction prediction = this.Mapper.Map<BracketPrediction>(BracketViewModel);
                        prediction.UserId = this.UserManager.GetUserId(this.User);
                        prediction.Playoff = this.Context.Playoffs.FirstOrDefault(x => x.Season.Year == 2021);
                        prediction.MatchupPredictions = new List<MatchupPrediction>();

                        foreach (var round in BracketViewModel.AfcRounds)
                        {
                            foreach (var afcGame in round.Games)
                            {
                                var afcMatchupPrediction = this.Mapper.Map<MatchupPrediction>(afcGame);
                                afcMatchupPrediction.PlayoffRoundId = round.Id;
                                afcMatchupPrediction.PredictedWinner = afcTeams.FirstOrDefault(x => x.Id == afcGame.SelectedWinner);
                                prediction.MatchupPredictions.Add(afcMatchupPrediction);
                            }
                        }

                        foreach (var round in BracketViewModel.NfcRounds)
                        {
                            foreach (var nfcGame in round.Games)
                            {
                                var nfcMatchupPrediction = this.Mapper.Map<MatchupPrediction>(nfcGame);
                                nfcMatchupPrediction.PlayoffRoundId = round.Id;
                                nfcMatchupPrediction.PredictedWinner = nfcTeams.FirstOrDefault(x => x.Id == nfcGame.SelectedWinner);
                                prediction.MatchupPredictions.Add(nfcMatchupPrediction);
                            }
                        }

                        var game = BracketViewModel.SuperBowl;

                        var matchupPrediction = this.Mapper.Map<MatchupPrediction>(game);
                        matchupPrediction.PlayoffRoundId = this.Context.PlayoffRounds.FirstOrDefault(x => x.Round.Number == 4).Id;
                        matchupPrediction.PredictedWinner = afcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner) == null ? nfcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner) : afcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner);
                        prediction.MatchupPredictions.Add(matchupPrediction);

                        this.Context.Add(prediction);
                        this.Context.SaveChanges();
                    }
                }
            }

            return this.View(BracketViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Update(int id)
        {
            var bracketPrediction = this.Context.BracketPredictions.FirstOrDefault(x => x.Id == id);

            if (bracketPrediction == null)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var bracketViewModel = this.Mapper.Map<BracketViewModel>(bracketPrediction);

            return this.View(bracketViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Update(int id, BracketViewModel BracketViewModel)
        {
            var bracketPrediction = this.Context.BracketPredictions.FirstOrDefault(x => x.Id == id);

            if (bracketPrediction == null)
            {

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
