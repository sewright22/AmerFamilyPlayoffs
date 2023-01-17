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
using Microsoft.IdentityModel.Tokens;
using PlayoffPool.MVC.Models;

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
        [Authorize]
        public IActionResult Create()
        {
            var BracketViewModel = new BracketViewModel();

            var afcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.AsNoTracking().Include("SeasonTeam.Team").FilterConference("NFC");

            BracketViewModel.Name = string.Empty;
            BracketViewModel.CanEdit = true;

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

                if (afcRounds.IsNullOrEmpty() && nfcRounds.IsNullOrEmpty())
                {
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

                    // Save to database.
                    var newId = this.SaveBracket(BracketViewModel, afcTeams, nfcTeams);

                    if (newId.HasValue)
                    {
                        return this.RedirectToAction(nameof(this.Update), new { id = newId });
                    }
                }
            }

            return this.View(BracketViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Update(int id)
        {
            BracketPrediction? bracketPrediction = this.GetBracketPrediction(id, false);

            if (bracketPrediction == null)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            BracketViewModel bracketViewModel = this.BuildBracketViewModel(bracketPrediction);

            if (bracketPrediction.UserId != this.UserManager.GetUserId(this.User) || true)
            {
                bracketViewModel.CanEdit = false;
            }
            else
            {
                bracketViewModel.CanEdit = true;
            }

            return this.View(bracketViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Update(int id, BracketViewModel BracketViewModel)
        {
            bool isEditable = true;
            BracketViewModel.CanEdit = true;
            if (this.ModelState.IsValid == false)
            {
                return this.View(BracketViewModel);
            }

            var bracketPrediction = this.Context.BracketPredictions.FirstOrDefault(x => x.Id == id);

            if (bracketPrediction == null)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            if (bracketPrediction.UserId != this.UserManager.GetUserId(this.User))
            {
                isEditable = false;
            }

            var afcTeams = this.Context.PlayoffTeams.Where(x => x.Playoff.Season.Year == 2022).Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.Where(x => x.Playoff.Season.Year == 2022).Include("SeasonTeam.Team").FilterConference("NFC");

            this.BuildDivisionalRound(BracketViewModel, afcTeams, nfcTeams);
            this.BuildChampionshipRound(BracketViewModel, afcTeams, nfcTeams);
            this.BuildSuperBowl(BracketViewModel, afcTeams, nfcTeams);

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
            }

            if (isEditable)
            {
                this.SaveBracket(BracketViewModel, afcTeams, nfcTeams);
            }
            else
            {
                BracketViewModel.CanEdit = false;
            }

            if (BracketViewModel.SuperBowl is not null && BracketViewModel.SuperBowl.SelectedWinner.HasValue)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View(BracketViewModel);

        }

        [HttpGet]
        [Authorize]
        public IActionResult Reset(int id)
        {
            BracketPrediction? bracketPrediction = this.GetBracketPrediction(id, false);

            if (bracketPrediction == null || bracketPrediction.UserId == this.UserManager.GetUserId(this.User))
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var predictionsToDelete = bracketPrediction.MatchupPredictions.ToList();

            predictionsToDelete.ForEach(
                x =>
                {
                    this.Context.Remove(x);
                    this.Context.SaveChanges();
                });

            return this.RedirectToAction(nameof(this.Update), new { id = id });
        }

        private MatchupViewModel BuildMatchup(string name, IQueryable<PlayoffTeam> teams, int gameNumber, int seed1, int seed2)
        {
            return new MatchupViewModel
            {
                GameNumber = gameNumber,
                Name = name,
                HomeTeam = this.Mapper.Map<PlayoffTeamViewModel>(teams.GetTeamFromSeed(Math.Min(seed1, seed2))),
                AwayTeam = this.Mapper.Map<PlayoffTeamViewModel>(teams.GetTeamFromSeed(Math.Max(seed1, seed2))),
            };
        }

        private List<PlayoffTeamViewModel> GetWinners(List<MatchupViewModel> games)
        {
            var winningIds = games.Select(x => x.SelectedWinner.Value).ToList();

            return games.Select(x => winningIds.Contains(x.HomeTeam.Id) ? x.HomeTeam : x.AwayTeam).ToList();
        }

        private BracketViewModel BuildBracketViewModel(BracketPrediction? bracketPrediction)
        {
            var bracketViewModel = this.Mapper.Map<BracketViewModel>(bracketPrediction);

            var afcTeams = this.Context.PlayoffTeams.Where(x => x.Playoff.Season.Year == 2022).Include("SeasonTeam.Team").FilterConference("AFC");
            var nfcTeams = this.Context.PlayoffTeams.Where(x => x.Playoff.Season.Year == 2022).Include("SeasonTeam.Team").FilterConference("NFC");
            var afcRounds = new List<RoundViewModel>(bracketViewModel.AfcRounds);
            var nfcRounds = new List<RoundViewModel>(bracketViewModel.NfcRounds);
            var winners = this.Context.RoundWinners.Where(x => x.PlayoffRound.Playoff.Season.Year == 2022).Select(x => new { RoundNumber = x.PlayoffRound.Round.Number, x.PlayoffTeamId }).ToList();

            this.BuildWildCardRound(bracketViewModel, afcTeams, nfcTeams);
            this.UpdateRoundPicks(bracketViewModel, 1, bracketPrediction.MatchupPredictions, winners.Where(x => x.RoundNumber == 1).Select(x => x.PlayoffTeamId).ToList());
            this.BuildDivisionalRound(bracketViewModel, afcTeams, nfcTeams);
            this.UpdateRoundPicks(bracketViewModel, 2, bracketPrediction.MatchupPredictions, winners.Where(x => x.RoundNumber == 2).Select(x => x.PlayoffTeamId).ToList());
            this.BuildChampionshipRound(bracketViewModel, afcTeams, nfcTeams);
            this.UpdateRoundPicks(bracketViewModel, 3, bracketPrediction.MatchupPredictions, winners.Where(x => x.RoundNumber == 3).Select(x => x.PlayoffTeamId).ToList());
            this.BuildSuperBowl(bracketViewModel, afcTeams, nfcTeams);
            this.UpdateSuperBowlPicks(bracketViewModel, bracketPrediction.MatchupPredictions);

            return bracketViewModel;
        }

        private BracketPrediction? GetBracketPrediction(int id, bool enableTracking)
        {
            var predictions = this.Context.BracketPredictions.AsQueryable();

            if (enableTracking)
            {
                predictions = predictions.AsNoTracking();
            }

            return predictions
                .Include("MatchupPredictions.PredictedWinner.SeasonTeam.Conference")
                .Include("MatchupPredictions.PredictedWinner.SeasonTeam.Team")
                .Include("MatchupPredictions.PlayoffRound.Round")
                .FirstOrDefault(x => x.Id == id);
        }

        private void BuildDivisionalRound(BracketViewModel bracketViewModel, IQueryable<PlayoffTeam> afcTeams, IQueryable<PlayoffTeam> nfcTeams)
        {
            var previousAfcRound = bracketViewModel.AfcRounds.Single(x => x.RoundNumber == 1);
            var previousNfcRound = bracketViewModel.NfcRounds.Single(x => x.RoundNumber == 1);

            var currentAfcRound = bracketViewModel.AfcRounds.SingleOrDefault(x => x.RoundNumber == 2);
            var currentNfcRound = bracketViewModel.NfcRounds.SingleOrDefault(x => x.RoundNumber == 2);

            if (currentAfcRound == null && previousAfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false)
            {
                foreach (var round in bracketViewModel.AfcRounds)
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

                List<MatchupViewModel> afcWildcardGames = bracketViewModel.AfcRounds.Single(x => x.RoundNumber == 1).Games.ToList();

                var pickedAfcWinners = this.GetWinners(afcWildcardGames).OrderByDescending(x => x.Seed).ToList();

                afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 1", afcTeams, 1, 1, pickedAfcWinners[0].Seed));
                afcDivisionalRound.Games.Add(this.BuildMatchup("AFC Divisional Game 2", afcTeams, 1, pickedAfcWinners[2].Seed, pickedAfcWinners[1].Seed));
                bracketViewModel.AfcRounds.Add(afcDivisionalRound);
            }

            if (currentNfcRound == null && previousNfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false)
            {
                foreach (var round in bracketViewModel.NfcRounds)
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

                List<MatchupViewModel> nfcWildcardGames = bracketViewModel.NfcRounds.Single(x => x.RoundNumber == 1).Games.ToList();
                var pickedNfcWinners = this.GetWinners(nfcWildcardGames).OrderByDescending(x => x.Seed).ToList();
                nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 1", nfcTeams, 1, 1, pickedNfcWinners[0].Seed));
                nfcDivisionalRound.Games.Add(this.BuildMatchup("NFC Divisional Game 2", nfcTeams, 1, pickedNfcWinners[2].Seed, pickedNfcWinners[1].Seed));
                bracketViewModel.NfcRounds.Add(nfcDivisionalRound);
            }
        }

        private void UpdateRoundPicks(BracketViewModel bracketViewModel, int roundNumber, List<MatchupPrediction> matchupPredictions, List<int> roundWinners)
        {
            var afcRound = bracketViewModel.AfcRounds.FirstOrDefault(x => x.Conference == "AFC" && x.RoundNumber == roundNumber);
            var nfcRound = bracketViewModel.NfcRounds.FirstOrDefault(x => x.Conference == "NFC" && x.RoundNumber == roundNumber);

            if (afcRound == null || nfcRound == null)
            {
                return;
            }

            foreach (var game in afcRound.Games)
            {
                if (game == null) continue;
                var teams = matchupPredictions.Where(x => x.PlayoffRound.Round.Number == afcRound.RoundNumber).Where(x => x.PredictedWinner != null && x.PredictedWinner.SeasonTeam.Conference.Name == "AFC");
                if (teams.Any())
                {
                    var selectedWinner = teams.FirstOrDefault(x => x.PredictedWinner != null && (x.PredictedWinner.Id == game.HomeTeam.Id || x.PredictedWinner.Id == game.AwayTeam.Id))?.PredictedWinner;
                    if (selectedWinner == null)
                    {
                        continue;
                    }
                    game.SelectedWinner = selectedWinner.Id;

                    if (roundWinners.Contains(game.HomeTeam.Id) || roundWinners.Contains(game.AwayTeam.Id))
                    {
                        game.IsCorrect = roundWinners.Contains(selectedWinner.Id);
                    }
                }
            }

            foreach (var game in nfcRound.Games)
            {
                if (game == null) continue;
                var teams = matchupPredictions.Where(x => x.PlayoffRound.Round.Number == afcRound.RoundNumber).Where(x => x.PredictedWinner != null && x.PredictedWinner.SeasonTeam.Conference.Name == "NFC");
                if (teams.Any())
                {
                    var selectedWinner = teams.FirstOrDefault(x => x.PredictedWinner != null && (x.PredictedWinner.Id == game.HomeTeam.Id || x.PredictedWinner.Id == game.AwayTeam.Id))?.PredictedWinner;
                    if (selectedWinner == null)
                    {
                        continue;
                    }
                    game.SelectedWinner = selectedWinner.Id;

                    if (roundWinners.Contains(game.HomeTeam.Id) || roundWinners.Contains(game.AwayTeam.Id))
                    {
                        game.IsCorrect = roundWinners.Contains(selectedWinner.Id);
                    }
                }
            }
        }

        private void UpdateSuperBowlPicks(BracketViewModel bracketViewModel, List<MatchupPrediction> matchupPredictions)
        {
            if (bracketViewModel.SuperBowl == null)
            {
                return;
            }

            var teams = matchupPredictions.Where(x => x.PlayoffRound.Round.Number == 4);
            var selectedWinner = teams.Where(x => x.PredictedWinner != null).FirstOrDefault(x => x.PredictedWinner.Id == bracketViewModel.SuperBowl.HomeTeam.Id || x.PredictedWinner.Id == bracketViewModel.SuperBowl.AwayTeam.Id).PredictedWinner;

            if (selectedWinner != null)
            {
                bracketViewModel.SuperBowl.SelectedWinner = selectedWinner.Id;
            }
        }

        private void BuildWildCardRound(BracketViewModel bracketViewModel, IQueryable<PlayoffTeam> afcTeams, IQueryable<PlayoffTeam> nfcTeams)
        {
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

            bracketViewModel.AfcRounds.Add(afcWildcardRound);
            bracketViewModel.NfcRounds.Add(nfcWildcardRound);
        }

        private int? SaveBracket(BracketViewModel BracketViewModel, IQueryable<PlayoffTeam> afcTeams, IQueryable<PlayoffTeam> nfcTeams)
        {
            BracketPrediction? prediction = null;

            if (BracketViewModel.Id != 0)
            {
                prediction = this.Context.BracketPredictions.Include(x => x.MatchupPredictions).FirstOrDefault(x => x.Id == BracketViewModel.Id);
            }

            if (prediction is null)
            {
                prediction = this.Mapper.Map<BracketPrediction>(BracketViewModel);
                prediction.UserId = this.UserManager.GetUserId(this.User);
                prediction.Playoff = this.Context.Playoffs.FirstOrDefault(x => x.Season.Year == 2022);
                prediction.MatchupPredictions = new List<MatchupPrediction>();
            }
            else
            {
                if (prediction.UserId != this.UserManager.GetUserId(this.User))
                {
                    throw new UnauthorizedAccessException();
                }

                prediction.MatchupPredictions.Clear();
            }

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

            if (BracketViewModel.SuperBowl is not null)
            {
                var game = BracketViewModel.SuperBowl;

                var matchupPrediction = this.Mapper.Map<MatchupPrediction>(game);
                matchupPrediction.PlayoffRoundId = this.Context.PlayoffRounds.FirstOrDefault(x => x.Round.Number == 4).Id;
                matchupPrediction.PredictedWinner = afcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner) == null ? nfcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner) : afcTeams.FirstOrDefault(x => x.Id == game.SelectedWinner);
                prediction.MatchupPredictions.Add(matchupPrediction);
            }

            if (prediction.Id == 0)
            {
                this.Context.Add(prediction);
            }

            this.Context.SaveChanges();

            return prediction == null ? null : prediction.Id;
        }

        private void BuildSuperBowl(BracketViewModel BracketViewModel, IQueryable<PlayoffTeam> afcTeams, IQueryable<PlayoffTeam> nfcTeams)
        {
            var previousAfcRound = BracketViewModel.AfcRounds.SingleOrDefault(x => x.RoundNumber == 3);
            var previousNfcRound = BracketViewModel.NfcRounds.SingleOrDefault(x => x.RoundNumber == 3);

            if (previousAfcRound == null || previousNfcRound == null)
            {
                return;
            }

            if (previousAfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false
             && previousNfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false
             && (BracketViewModel.SuperBowl == null || BracketViewModel.SuperBowl.HomeTeam == null || BracketViewModel.SuperBowl.AwayTeam == null))
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

                List<MatchupViewModel> afcChampionshipGame = previousAfcRound.Games.ToList();
                List<MatchupViewModel> nfcChampionshipGame = previousNfcRound.Games.ToList();
                var pickedAfcWinner = this.GetWinners(afcChampionshipGame).Single().Seed;
                var pickedNfcWinner = this.GetWinners(nfcChampionshipGame).Single().Seed;
                var homeTeam = this.Mapper.Map<PlayoffTeamViewModel>(afcTeams.GetTeamFromSeed(pickedAfcWinner));
                var awayTeam = this.Mapper.Map<PlayoffTeamViewModel>(nfcTeams.GetTeamFromSeed(pickedNfcWinner));
                BracketViewModel.SuperBowl = new MatchupViewModel
                {
                    Name = "Super Bowl",
                    GameNumber = 1,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                };
            }
        }

        private void BuildChampionshipRound(BracketViewModel BracketViewModel, IQueryable<PlayoffTeam> afcTeams, IQueryable<PlayoffTeam> nfcTeams)
        {
            var previousAfcRound = BracketViewModel.AfcRounds.SingleOrDefault(x => x.RoundNumber == 2);
            var previousNfcRound = BracketViewModel.NfcRounds.SingleOrDefault(x => x.RoundNumber == 2);

            if (previousAfcRound == null || previousNfcRound == null)
            {
                return;
            }

            var currentAfcRound = BracketViewModel.AfcRounds.SingleOrDefault(x => x.RoundNumber == 3);
            var currentNfcRound = BracketViewModel.NfcRounds.SingleOrDefault(x => x.RoundNumber == 3);

            if (currentAfcRound == null && previousAfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false)
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

                List<MatchupViewModel> afcDivisionalGames = BracketViewModel.AfcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                var pickedAfcWinners = this.GetWinners(afcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                afcChampionship.Games.Add(this.BuildMatchup("AFC Championship Game", afcTeams, 1, pickedAfcWinners[1].Seed, pickedAfcWinners[0].Seed));
                BracketViewModel.AfcRounds.Add(afcChampionship);
            }

            if (currentNfcRound == null && previousNfcRound.Games.Any(x => x.SelectedWinner.HasValue == false) == false)
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

                List<MatchupViewModel> nfcDivisionalGames = BracketViewModel.NfcRounds.Single(x => x.RoundNumber == 2).Games.ToList();
                var pickedNfcWinners = this.GetWinners(nfcDivisionalGames).OrderByDescending(x => x.Seed).ToList();
                nfcChampionship.Games.Add(this.BuildMatchup("NFC Championship Game", nfcTeams, 1, pickedNfcWinners[1].Seed, pickedNfcWinners[0].Seed));
                BracketViewModel.NfcRounds.Add(nfcChampionship);
            }
        }
    }
}
