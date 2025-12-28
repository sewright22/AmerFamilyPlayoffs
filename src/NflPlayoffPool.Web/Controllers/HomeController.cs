// <copyright file="HomeController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.Models.Home;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PlayoffPoolContext DbContext { get; }

        public HomeController(ILogger<HomeController> logger, PlayoffPoolContext dbContext)
        {
            _logger = logger;
            this.DbContext = dbContext;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Index()
        {
            if (this.User == null || this.User.Identity == null || !this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction(nameof(AccountController.Login), nameof(AccountController).GetControllerNameForUri());
            }

            Season? currentSeason = this.DbContext.GetCurrentSeason();

            var model = new HomeViewModel();

            if (currentSeason != null)
            {
                model.IsPlayoffStarted = currentSeason.IsStarted();
                model.CanSubmitBrackets = true;
                model.IncompleteBrackets = this.DbContext.Brackets
                    .Where(b => b.SeasonYear == currentSeason.Year)
                    .Where(b => !b.IsSubmitted)
                    .Where(b => b.UserId == this.User.GetUserId())
                .AsNoTracking()
                .ToList().AsBracketSummaryModels();

                model.CompletedBrackets = this.DbContext.Brackets
                    .Where(b => b.SeasonYear == currentSeason.Year)
                    .Where(b => b.IsSubmitted)
                    .Where(b => b.UserId == this.User.GetUserId())
                .AsNoTracking()
                .ToList().AsBracketSummaryModels();

                model.Leaderboard = new LeaderboardViewModel
                {
                    ShowLeaderboard = currentSeason.IsStarted(),
                    Brackets = currentSeason.IsStarted() ? this.BuildLeaderboard(currentSeason) : new List<BracketSummaryModel>(),
                };
            }
            else
            {
                model.CanSubmitBrackets = false;
            }
            return this.View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            // Sign out the user by clearing the authentication cookie
            await this.HttpContext.SignOutAsync();

            // Redirect to a specific page after logout
            return RedirectToAction(nameof(this.Index), nameof(HomeController).GetControllerNameForUri());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<BracketSummaryModel> BuildLeaderboard(Season currentSeason)
        {
            List<BracketSummaryModel> brackets = this.DbContext.Brackets
                .AsNoTracking()
                .Where(b => b.SeasonYear == currentSeason.Year)
                .Where(b => b.IsSubmitted)
                .OrderByDescending(b => b.CurrentScore)
                .ThenByDescending(b => b.MaxPossibleScore)
                .ToList().AsBracketSummaryModels();

            BracketSummaryModel? previousBracket = null;
            int count = 0;
            int thresholdScore = brackets.Count > 2 ? brackets.Skip(2).First().CurrentScore : 0;
            foreach (var bracket in brackets)
            {
                bool isTied = false;
                bool isEliminated = false;
                count++;

                if (previousBracket is null)
                {
                    bracket.Place = count;
                }
                else if (bracket.CurrentScore == previousBracket.CurrentScore && bracket.MaxPossibleScore == previousBracket.MaxPossibleScore)
                {
                    bracket.Place = previousBracket.Place;
                    isTied = true;
                }
                else
                {
                    bracket.Place = count;
                }

                if (bracket.MaxPossibleScore < thresholdScore)
                {
                    isEliminated = true;
                }

                bracket.PlaceAsString = this.BuildPlaceAsString(bracket.Place, isTied, isEliminated);

                if (previousBracket is not null && isTied)
                {
                    previousBracket.PlaceAsString = this.BuildPlaceAsString(previousBracket.Place, isTied, isEliminated);
                }

                previousBracket = bracket;
            }

            // Determine the selected winners of the top 3 brackets.
            var currentlyPlacingBrackets = brackets.Where(b => b.Place <= 3).ToList();
            if (currentlyPlacingBrackets.Any())
            {
                int lowestPlace = currentlyPlacingBrackets.Min(b => b.Place);

                // Get list of PredictedWinners from the lowest place bracket
                var lowestSelectedWinners = currentlyPlacingBrackets.Where(b => b.Place == lowestPlace).Select(b => b.PredictedWinner).ToList();

                foreach (var bracket in brackets.Where(b => b.Place > 3))
                {
                    if (lowestSelectedWinners.Contains(bracket.PredictedWinner))
                    {
                        // This bracket cannot win because it has the same predicted winner as the lowest placing bracket
                        bracket.PlaceAsString = this.BuildPlaceAsString(bracket.Place, false, true);
                    }
                }
            }

            return brackets;
        }

        private string? BuildPlaceAsString(int place, bool isTied, bool isEliminated)
        {
            // Get the last digit of the place number, except for numbers ending in 11, 12, and 13
            int lastDigit = place % 10;
            int lastTwoDigits = place % 100;
            if (lastTwoDigits == 11 || lastTwoDigits == 12 || lastTwoDigits == 13)
            {
                lastDigit = 0;
            }

            string placeAsString = lastDigit switch
            {
                1 => $"{place}st",
                2 => $"{place}nd",
                3 => $"{place}rd",
                _ => $"{place}th",
            };

            if (isEliminated)
            {
                placeAsString = "e-" + placeAsString;
            }
            else if (isTied)
            {
                placeAsString = "T-" + placeAsString;

            }

            return placeAsString;
        }

        private int GetTimeZoneOffsetFromClaims()
        {
            var claim = User.FindFirst("TimeZoneOffset");
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
