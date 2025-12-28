// <copyright file="BracketController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Admin.ViewModels;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models.Bracket;
    using NflPlayoffPool.Web.Services;

    [Area("Admin")]
    public class BracketController : Controller
    {
        private IBreadcrumbService _breadcrumbService;

        public BracketController(PlayoffPoolContext dbContext, IBreadcrumbService breadcrumbService)
        {
            this.DbContext = dbContext;
            this._breadcrumbService = breadcrumbService;
        }

        public PlayoffPoolContext DbContext { get; }

        [Authorize]
        public IActionResult Index(string seasonId, int seasonYear)
        {
            BracketsModel model = new BracketsModel();

            model.Brackets.AddRange(this.DbContext.Brackets
                .Where(b => b.SeasonYear == seasonYear)
                .AsNoTracking()
                .ToList()
                .AsBracketSummaryModels());

            this._breadcrumbService.AddAdminBreadcrumbs(model, seasonId, seasonYear);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Update(string id)
        {
            Bracket? bracket = this.DbContext.Brackets
                .Where(b => b.Id == id)
                .AsNoTracking()
                .FirstOrDefault();

            if (bracket == null)
            {
                return this.NotFound();
            }

            Season? season = this.DbContext.Seasons
                .Where(s => s.Year == bracket.SeasonYear)
                .AsNoTracking()
                .FirstOrDefault();

            if (season == null)
            {
                return this.NotFound();
            }

            BracketViewModel model = bracket.AsAdminBracketViewModel();
            model.Bracket.CanEdit = true;

            BuildWildcardRound(season, bracket, model);
            BuildDivisionalRound(season, bracket, model);
            BuildConferenceRound(season, bracket, model);
            BuildSuperBowlRound(season, bracket, model);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Update(BracketModel bracketModel)
        {
            Season? currentSeason = this.DbContext.GetCurrentSeason();

            if (currentSeason is null)
            {
                this.ModelState.AddModelError(string.Empty, "No current season found.");
            }

            if (this.ModelState.IsValid == false || currentSeason == null)
            {
                return this.View(bracketModel);
            }

            var afcTeams = currentSeason.Teams.Where(x => x.Conference == Conference.AFC);
            var nfcTeams = currentSeason.Teams.Where(x => x.Conference == Conference.NFC);

            // Save to database.
            var bracketId = this.SaveBracket(bracketModel, afcTeams, nfcTeams, currentSeason);

            if (bracketId is not null && bracketModel.SuperBowl?.SelectedWinner is null)
            {
                return this.RedirectToAction(nameof(this.Update), new { id = bracketId });
            }

            if (bracketModel.SuperBowl.SelectedWinner is not null)
            {
                return this.RedirectToAction(nameof(this.Index), new { seasonYear = bracketModel.SeasonYear });
            }

            return this.RedirectToAction(nameof(this.Update), new { id = bracketId });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Reset(string id)
        {
            var bracket = this.DbContext.Brackets.FirstOrDefault(x => x.Id == id);
            if (bracket is not null)
            {
                bracket.Picks.Clear();
                bracket.IsSubmitted = false;
                bracket.PredictedWinner = null;
                this.DbContext.SaveChanges();
            }
            return this.RedirectToAction(nameof(this.Update), new { id });
        }

        private static void BuildWildcardRound(Season? currentSeason, Bracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            Web.Models.Bracket.RoundModel afcWildcardRound = currentSeason.GenerateAfcWildcardRound();
            afcWildcardRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 1));
            bracketPrediction.Bracket.AfcRounds.Add(afcWildcardRound);

            Web.Models.Bracket.RoundModel nfcWildcardRound = currentSeason.GenerateNfcWildcardRound();
            nfcWildcardRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 1));
            bracketPrediction.Bracket.NfcRounds.Add(nfcWildcardRound);
        }

        private static void BuildDivisionalRound(Season? currentSeason, Bracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 1))
            {
                return;
            }

            var afcDivisionalRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 1);

            if (afcDivisionalRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel afcDivisionalRound = currentSeason.GenerateAfcDivisionalRound(afcDivisionalRoundFromDb);
                afcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 2));
                bracketPrediction.Bracket.AfcRounds.Add(afcDivisionalRound);
            }

            var nfcDivisionalRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 1);

            if (nfcDivisionalRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel nfcDivisionalRound = currentSeason.GenerateNfcDivisionalRound(nfcDivisionalRoundFromDb);
                nfcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 2));
                bracketPrediction.Bracket.NfcRounds.Add(nfcDivisionalRound);
            }
        }

        private static void BuildConferenceRound(Season? currentSeason, Bracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 2))
            {
                return;
            }

            var afcConferenceRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 2);
            if (afcConferenceRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel afcConferenceRound = currentSeason.GenerateAfcConferenceRound(afcConferenceRoundFromDb);
                afcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 3));
                bracketPrediction.Bracket.AfcRounds.Add(afcConferenceRound);
            }
            var nfcConferenceRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 2);
            if (nfcConferenceRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel nfcConferenceRound = currentSeason.GenerateNfcConferenceRound(nfcConferenceRoundFromDb);
                nfcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 3));
                bracketPrediction.Bracket.NfcRounds.Add(nfcConferenceRound);
            }
        }

        private static void BuildSuperBowlRound(Season? currentSeason, Bracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 3))
            {
                return;
            }

            var afcConferenceRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 3);
            var nfcConferenceRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 3);

            if (afcConferenceRoundFromDb is not null && nfcConferenceRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel superBowlRound = currentSeason.GenerateSuperBowlRound(afcConferenceRoundFromDb, nfcConferenceRoundFromDb);
                superBowlRound.SetSelectedWinners(bracket.Picks.Where(x => x.RoundNumber == 4));
                bracketPrediction.Bracket.SuperBowl = superBowlRound.Games.FirstOrDefault();
            }
        }

        private string? SaveBracket(Season season, BracketModel bracketModel, IEnumerable<PlayoffTeam> afcTeams, IEnumerable<PlayoffTeam> nfcTeams)
        {
            // Check for existing bracket
            var existingBracket = season.Bracket;

            if (existingBracket is null)
            {
                existingBracket = new MasterBracket()
                {
                    Id = season.Id,
                    Name = bracketModel.Name,
                    UserId = this.User.GetUserId(),
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    SeasonYear = this.DbContext.GetCurrentSeason().Year,
                    Picks = new List<BracketPick>(),
                };

                season.Bracket = existingBracket;
            }

            season.Bracket.Name = bracketModel.Name;
            season.Bracket.Picks.Clear();
            season.Bracket.Picks = bracketModel.ExtractPicks();
            season.Bracket.LastModified = DateTime.UtcNow;
            season.Bracket.IsSubmitted = existingBracket.Picks.Count(x => x.PredictedWinningId != null) == 13;
            season.Bracket.Winner = existingBracket.Picks?.FirstOrDefault(x => x.RoundNumber == 4)?.PredictedWinningTeam;

            this.DbContext.SaveChanges();
            return existingBracket.Id;
        }

        private string? SaveBracket(BracketModel bracketModel, IEnumerable<PlayoffTeam> afcTeams, IEnumerable<PlayoffTeam> nfcTeams, Season currentSeason)
        {
            // Check for existing bracket
            var existingBracket = this.DbContext.Brackets
                .FirstOrDefault(x => x.Id == bracketModel.Id);

            if (existingBracket is null)
            {
                existingBracket = new Bracket()
                {
                    Id = bracketModel.Id,
                    Name = bracketModel.Name,
                    UserId = this.User.GetUserId(),
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    SeasonYear = this.DbContext.GetCurrentSeason().Year,
                    MaxPossibleScore = 42,
                    Picks = new List<BracketPick>(),
                };

                this.DbContext.Brackets.Add(existingBracket);
            }

            existingBracket.Name = bracketModel.Name;
            existingBracket.Picks.Clear();
            existingBracket.Picks = bracketModel.ExtractPicks();
            existingBracket.LastModified = DateTime.UtcNow;
            existingBracket.IsSubmitted = existingBracket.Picks.Count(x => x.PredictedWinningId != null) == 13;
            existingBracket.PredictedWinner = existingBracket.Picks?.FirstOrDefault(x => x.RoundNumber == 4)?.PredictedWinningTeam;

            if (existingBracket.IsSubmitted)
            {
                existingBracket.CalculateScores(currentSeason);
            }

            this.DbContext.SaveChanges();
            return existingBracket.Id;
        }
    }
}
