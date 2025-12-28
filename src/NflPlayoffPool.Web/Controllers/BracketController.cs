// <copyright file="BracketController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Bson;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.Models.Bracket;
    using NflPlayoffPool.Web.ViewModels;

    public class BracketController : Controller
    {
        private readonly ILogger<BracketController> _logger;

        public PlayoffPoolContext DbContext { get; }

        public BracketController(ILogger<BracketController> logger, PlayoffPoolContext dbContext)
        {
            _logger = logger;
            this.DbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            var bracket = new BracketModel()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = string.Empty,
                CanEdit = true,
                UserId = this.User.GetUserId(),
            };

            var bracketViewModel = new BracketViewModel()
            {
                Bracket = bracket,
            };

            Season? currentSeason = this.DbContext.GetCurrentSeason();

            bracketViewModel.Bracket.CanEdit = currentSeason.IsStarted() == false;
            return View(bracketViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(BracketModel bracketModel)
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
            var newId = this.SaveBracket(bracketModel, afcTeams, nfcTeams);

            if (newId is not null)
            {
                return this.RedirectToAction(nameof(this.Update), new { id = newId });
            }

            return this.View(new BracketViewModel() { Bracket = bracketModel });
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

        [HttpGet]
        [Authorize]
        public IActionResult Update(string id)
        {
            Season? currentSeason = this.DbContext.GetCurrentSeason();

            if (currentSeason is null)
            {
                this.ModelState.AddModelError(string.Empty, "No current season found.");
            }

            Bracket? bracket = this.DbContext.Brackets.FirstOrDefault(x => x.Id == id);

            if (bracket == null)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            BracketViewModel? bracketPrediction = bracket.AsBracketViewModel();

            BuildWildcardRound(currentSeason, bracket, bracketPrediction);
            BuildDivisionalRound(currentSeason, bracket, bracketPrediction);
            BuildConferenceRound(currentSeason, bracket, bracketPrediction);
            BuildSuperBowlRound(currentSeason, bracket, bracketPrediction);

            bracketPrediction.Bracket.CanEdit = bracketPrediction.UserId == this.User.GetUserId() && currentSeason.IsStarted() == false;

            foreach (var round in bracketPrediction.Bracket.AfcRounds)
            {
                round.IsLocked = !bracketPrediction.Bracket.CanEdit;

                foreach (var game in round.Games)
                {
                    game.IsLocked = !bracketPrediction.Bracket.CanEdit;
                }
            }

            foreach (var round in bracketPrediction.Bracket.NfcRounds)
            {
                round.IsLocked = !bracketPrediction.Bracket.CanEdit;

                foreach (var game in round.Games)
                {
                    game.IsLocked = !bracketPrediction.Bracket.CanEdit;
                }
            }

            if (bracketPrediction.Bracket.SuperBowl is not null)
            {
                bracketPrediction.Bracket.SuperBowl.IsLocked = !bracketPrediction.Bracket.CanEdit;
            }

            return this.View(bracketPrediction);
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
            var bracketId = this.SaveBracket(bracketModel, afcTeams, nfcTeams);

            if (bracketId is not null && bracketModel.SuperBowl?.SelectedWinner is null)
            {

                return this.RedirectToAction(nameof(this.Update), new { id = bracketId });
            }

            if (bracketModel.SuperBowl.SelectedWinner is not null)
            {

                return this.RedirectToAction(nameof(HomeController.Index), nameof(HomeController).GetControllerNameForUri());
            }

            return this.RedirectToAction(nameof(this.Update), new { id = bracketId });
        }

        private string? SaveBracket(BracketModel bracketModel, IEnumerable<PlayoffTeam> afcTeams, IEnumerable<PlayoffTeam> nfcTeams)
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

            this.DbContext.SaveChanges();
            return existingBracket.Id;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetTimeZoneOffsetFromClaims()
        {
            var claim = User.FindFirst("TimeZoneOffset");
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private static void BuildWildcardRound(Season? currentSeason, Bracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            RoundModel afcWildcardRound = currentSeason.GenerateAfcWildcardRound();
            List<string?> masterAfcWildcardPicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 1).Select(x => x.PredictedWinningId).ToList();
            afcWildcardRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 1), masterAfcWildcardPicks);
            bracketPrediction.Bracket.AfcRounds.Add(afcWildcardRound);

            RoundModel nfcWildcardRound = currentSeason.GenerateNfcWildcardRound();
            List<string?> masterNfcWildcardPicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 1).Select(x => x.PredictedWinningId).ToList();
            nfcWildcardRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 1), masterNfcWildcardPicks);
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
                RoundModel afcDivisionalRound = currentSeason.GenerateAfcDivisionalRound(afcDivisionalRoundFromDb);
                List<string?> masterAfcDivisionalPicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 2).Select(x => x.PredictedWinningId).ToList();
                afcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 2), masterAfcDivisionalPicks);
                bracketPrediction.Bracket.AfcRounds.Add(afcDivisionalRound);
            }

            var nfcDivisionalRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 1);

            if (nfcDivisionalRoundFromDb is not null)
            {
                RoundModel nfcDivisionalRound = currentSeason.GenerateNfcDivisionalRound(nfcDivisionalRoundFromDb);
                List<string?> masterNfcDivisionalPicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 2).Select(x => x.PredictedWinningId).ToList();
                nfcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 2), masterNfcDivisionalPicks);
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
                RoundModel afcConferenceRound = currentSeason.GenerateAfcConferenceRound(afcConferenceRoundFromDb);
                List<string?> masterAfcConferencePicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 3).Select(x => x.PredictedWinningId).ToList();
                afcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 3), masterAfcConferencePicks);
                bracketPrediction.Bracket.AfcRounds.Add(afcConferenceRound);
            }
            var nfcConferenceRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 2);
            if (nfcConferenceRoundFromDb is not null)
            {
                RoundModel nfcConferenceRound = currentSeason.GenerateNfcConferenceRound(nfcConferenceRoundFromDb);
                List<string?> masterNfcConferencePicks = currentSeason.Bracket == null ? new List<string?>() : currentSeason.Bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 3).Select(x => x.PredictedWinningId).ToList();
                nfcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 3), masterNfcConferencePicks);
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
                RoundModel superBowlRound = currentSeason.GenerateSuperBowlRound(afcConferenceRoundFromDb, nfcConferenceRoundFromDb);
                List<string?> masterSuperBowlPicks = currentSeason.Bracket == null || currentSeason.Bracket.Winner == null ? new List<string?>() : new List<string?>
                {
                    currentSeason.Bracket.Winner,
                };
                superBowlRound.SetSelectedWinners(bracket.Picks.Where(x => x.RoundNumber == 4), masterSuperBowlPicks);
                bracketPrediction.Bracket.SuperBowl = superBowlRound.Games.FirstOrDefault();
            }
        }
    }
}
