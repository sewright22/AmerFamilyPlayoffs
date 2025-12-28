// <copyright file="SeasonController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Areas.Admin.ViewModels;
    using NflPlayoffPool.Web.Extensions;
    using NflPlayoffPool.Web.Models.Bracket;
    using NflPlayoffPool.Web.Services;
    using NflPlayoffPool.Web.ViewModels;

    /// <summary>
    /// Controller for managing NFL playoff pool seasons.
    /// </summary>
    [Area("Admin")]
    public class SeasonController : Controller
    {
        private readonly SuperGridImporter _superGridImporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dbContext">The database context.</param>
        public SeasonController(ILogger<SeasonController> logger, PlayoffPoolContext dbContext)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.Logger = logger;
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Gets the logger instance.
        /// </summary>
        public ILogger<SeasonController> Logger { get; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        public PlayoffPoolContext DbContext { get; }

        /// <summary>
        /// Displays the list of seasons.
        /// </summary>
        /// <returns>The view for the list of seasons.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            SeasonsModel model = new SeasonsModel();

            model.Seasons.AddRange(this.DbContext.Seasons
                .OrderBy(x => x.Year).AsSeasonSummaryList());

            return this.View(model);
        }

        /// <summary>
        /// Displays the create season form.
        /// </summary>
        /// <returns>The partial view for creating a season.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            CreateSeasonViewModel model = new CreateSeasonViewModel()
            {
                Season = new SeasonModel()
                {
                    Year = DateTime.Now.Year.ToString(),
                },
            };

            return this.PartialView(model);
        }

        /// <summary>
        /// Handles the creation of a new season.
        /// </summary>
        /// <param name="model">The season model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(SeasonModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.Logger.LogWarning("Invalid model state for creating season.");
                return this.PartialView(model);
            }

            try
            {
                model.Id = await this.DbContext.Create(model);
                this.Logger.LogInformation("Season created successfully with ID {SeasonId}", model.Id);
                return this.RedirectToAction(nameof(this.Details), new { area = "Admin", id = model.Id });
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error creating season.");
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the season.");
                return this.PartialView(model);
            }
        }

        /// <summary>
        /// Displays the details of a season.
        /// </summary>
        /// <param name="id">The season ID.</param>
        /// <returns>The view for the season details.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Details(string? id)
        {
            Season? season = this.DbContext.Seasons.FirstOrDefault(x => x.Id == id);
            int bracketCount = this.DbContext.Brackets.Count(x => x.SeasonYear == season.Year);

            UpdateSeasonViewModel model = new UpdateSeasonViewModel()
            {
                Season = season.AsSeasonModel(bracketCount, this.GetTimeZoneOffsetFromClaims()),
            };

            model.AddBreadcrumb(model.Season.Year);

            return this.View(model);
        }

        /// <summary>
        /// Displays the edit season form.
        /// </summary>
        /// <param name="id">The season ID.</param>
        /// <returns>The partial view for editing a season.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string? id)
        {
            Season? season = this.DbContext.Seasons.FirstOrDefault(x => x.Id == id);
            int bracketCount = this.DbContext.Brackets.Count(x => x.SeasonYear == season.Year);

            CreateSeasonViewModel model = new CreateSeasonViewModel()
            {
                Season = season.AsSeasonModel(bracketCount, this.GetTimeZoneOffsetFromClaims()),
            };

            return this.PartialView(model);
        }

        /// <summary>
        /// Handles the editing of a season.
        /// </summary>
        /// <param name="model">The season model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(SeasonModel model)
        {
            if (this.ModelState.IsValid)
            {
                Season? season = this.DbContext.Seasons.FirstOrDefault(x => x.Id.ToString() == model.Id);
                if (season != null)
                {
                    bool wasCurrent = season.IsCurrent;

                    season.Year = int.Parse(model.Year);
                    season.Status = model.Status;
                    season.IsCurrent = model.IsCurrent;
                    season.CurrentRound = model.CurrentRound;
                    season.WildcardPoints = model.WildcardPoints;
                    season.DivisionalPoints = model.DivisionalPoints;
                    season.ConferencePoints = model.ConferencePoints;
                    season.SuperBowlPoints = model.SuperBowlPoints;

                    // Update all other seasons to not be current.
                    if (season.IsCurrent && !wasCurrent)
                    {
                        IQueryable<Season> otherSeasons = this.DbContext.Seasons.Where(x => x.Id != season.Id);
                        foreach (Season? otherSeason in otherSeasons)
                        {
                            otherSeason.IsCurrent = false;
                        }
                    }

                    this.DbContext.SaveChanges();

                    return this.RedirectToAction(nameof(this.Details), nameof(SeasonController).GetControllerNameForUri(), new { area = "Admin", id = model.Id });
                }
                else
                {
                    this.ModelState.AddModelError("", "Season not found.");
                }
            }

            return this.View(model);
        }

        /// <summary>
        /// Displays the bracket for a season.
        /// </summary>
        /// <param name="id">The season ID.</param>
        /// <returns>The view for the bracket.</returns>
        [HttpGet("Admin/Season/Details/{id}/Bracket")]
        [Authorize(Roles = "Admin")]
        public IActionResult Bracket(string? id)
        {
            Season? season = this.DbContext.Seasons.FirstOrDefault(x => x.Id == id);

            if (season == null)
            {
                return this.NotFound();
            }

            if (season.Bracket == null)
            {
                season.Bracket = new MasterBracket()
                {
                    Id = season.Id,
                    SeasonYear = season.Year,
                    Name = $"{season.Year} Master",
                    UserId = this.User.GetUserId(),
                };
            }

            BracketViewModel model = season.Bracket.AsBracketViewModel();
            model.Bracket.CanEdit = true;

            IEnumerable<PlayoffTeam> afcTeams = season.Teams.Where(x => x.Conference == Conference.AFC);
            IEnumerable<PlayoffTeam> nfcTeams = season.Teams.Where(x => x.Conference == Conference.NFC);
            List<RoundModel> afcRounds = new List<Web.Models.Bracket.RoundModel>(model.Bracket.AfcRounds);
            List<RoundModel> nfcRounds = new List<Web.Models.Bracket.RoundModel>(model.Bracket.NfcRounds);
            BuildWildcardRound(season, season.Bracket, model);
            BuildDivisionalRound(season, season.Bracket, model);
            BuildConferenceRound(season, season.Bracket, model);
            BuildSuperBowlRound(season, season.Bracket, model);

            model.AddBreadcrumb(season.Year.ToString());

            return this.View(model);
        }

        /// <summary>
        /// Handles the submission of a bracket.
        /// </summary>
        /// <param name="id">The season ID.</param>
        /// <param name="bracketModel">The bracket model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("Admin/Season/Details/{id}/Bracket")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Bracket(string? id, BracketModel bracketModel)
        {
            Season? currentSeason = this.DbContext.GetCurrentSeason(true);

            if (currentSeason is null)
            {
                this.ModelState.AddModelError(string.Empty, "No current season found.");
            }

            if (this.ModelState.IsValid == false || currentSeason == null)
            {
                return this.View(bracketModel);
            }

            IEnumerable<PlayoffTeam> afcTeams = currentSeason.Teams.Where(x => x.Conference == Conference.AFC);
            IEnumerable<PlayoffTeam> nfcTeams = currentSeason.Teams.Where(x => x.Conference == Conference.NFC);

            // Save to database.
            string? bracketId = this.SaveBracket(currentSeason, bracketModel, afcTeams, nfcTeams);

            this.UpdateScores(currentSeason);

            if (bracketId is not null)
            {
                return this.RedirectToAction(nameof(this.Bracket), new { id = currentSeason.Id });
            }

            return this.View(new BracketViewModel() { Bracket = bracketModel });
        }

        /// <summary>
        /// Resets the bracket for a season.
        /// </summary>
        /// <param name="id">The season ID.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Reset(string id)
        {
            MasterBracket? bracket = this.DbContext.Seasons.Where(x => x.Id == id).First().Bracket;
            if (bracket is not null)
            {
                bracket.Picks.Clear();
                bracket.IsSubmitted = false;
                bracket.Winner = null;
                this.DbContext.SaveChanges();
            }

            return this.RedirectToAction(nameof(this.Bracket), new { id });
        }

        /// <summary>
        /// Displays the SuperGrid import form.
        /// </summary>
        /// <returns>The view for importing SuperGrid data.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult SuperGridImport()
        {
            return this.View(new SuperGridImportViewModel());
        }

        /// <summary>
        /// Handles the SuperGrid import.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuperGridImport(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                this.ModelState.AddModelError("", "Please upload a valid Excel file.");
                return this.View();
            }

            Stream fileStream = file.OpenReadStream();
            SuperGridImporter importer = new SuperGridImporter(this.DbContext, fileStream);
            SuperGridRawImport importResult = importer.Import(file.FileName);

            return this.RedirectToAction(nameof(this.MatchUsers), new { importId = importResult.Id, rawImport = importResult });
        }

        /// <summary>
        /// Matches imported users with existing users.
        /// </summary>
        /// <param name="rawImport">The raw import data.</param>
        /// <returns>The view for matching users.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MatchUsers(SuperGridRawImport? rawImport)
        {
            if (rawImport != null && rawImport.Name == string.Empty)
            {
                rawImport = this.DbContext.SuperGridRawImports.FirstOrDefault(x => x.Id == rawImport.Id);
            }

            if (rawImport == null)
            {
                return this.NotFound();
            }

            MatchUsersViewModel model = new MatchUsersViewModel()
            {
                ExistingUsers = this.DbContext.Users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList(),
                ImportedUsers = new List<SuperGridImportUserModel>(),
            };

            // Get row 1
            SuperGridRawImportRow row1 = rawImport.Rows[0];

            // Users should start at column 3
            int userColumnIndex = 2;
            for (int cellIndex = userColumnIndex; cellIndex < row1.Cells.Count; cellIndex++)
            {
                string? userName = row1.Cells[cellIndex].Value;

                if (string.IsNullOrEmpty(userName))
                {
                    continue;
                }

                SuperGridImportUserModel importUser = new SuperGridImportUserModel()
                {
                    Name = userName,
                    MatchedUserId = FindMatchingUser(model, userName),
                };

                model.ImportedUsers.Add(importUser);
            }

            return this.View("MatchUsers", model);
        }

        private static string? FindMatchingUser(MatchUsersViewModel model, string userName)
        {
            string? userWithMatchingAlias = model.ExistingUsers.FirstOrDefault(x => x.Aliases != null && x.Aliases.Contains(userName))?.Id;

            if (userWithMatchingAlias != null)
            {
                return userWithMatchingAlias;
            }

            string? userWithMatchingName = model.ExistingUsers.FirstOrDefault(x => x.FullName.ToUpper().Contains(userName))?.Id;

            if (userWithMatchingName != null)
            {
                return userWithMatchingName;
            }

            return null;
        }

        private static void BuildWildcardRound(Season? currentSeason, MasterBracket? bracket, BracketViewModel bracketPrediction)
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

        private static void BuildDivisionalRound(Season? currentSeason, MasterBracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 1))
            {
                return;
            }

            RoundModel? afcDivisionalRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 1);

            if (afcDivisionalRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel afcDivisionalRound = currentSeason.GenerateAfcDivisionalRound(afcDivisionalRoundFromDb);
                afcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 2));
                bracketPrediction.Bracket.AfcRounds.Add(afcDivisionalRound);
            }

            RoundModel? nfcDivisionalRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 1);

            if (nfcDivisionalRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel nfcDivisionalRound = currentSeason.GenerateNfcDivisionalRound(nfcDivisionalRoundFromDb);
                nfcDivisionalRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 2));
                bracketPrediction.Bracket.NfcRounds.Add(nfcDivisionalRound);
            }
        }

        private static void BuildConferenceRound(Season? currentSeason, MasterBracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 2))
            {
                return;
            }

            RoundModel? afcConferenceRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 2);
            if (afcConferenceRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel afcConferenceRound = currentSeason.GenerateAfcConferenceRound(afcConferenceRoundFromDb);
                afcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "AFC" && x.RoundNumber == 3));
                bracketPrediction.Bracket.AfcRounds.Add(afcConferenceRound);
            }
            RoundModel? nfcConferenceRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 2);
            if (nfcConferenceRoundFromDb is not null)
            {
                Web.Models.Bracket.RoundModel nfcConferenceRound = currentSeason.GenerateNfcConferenceRound(nfcConferenceRoundFromDb);
                nfcConferenceRound.SetSelectedWinners(bracket.Picks.Where(x => x.Conference == "NFC" && x.RoundNumber == 3));
                bracketPrediction.Bracket.NfcRounds.Add(nfcConferenceRound);
            }
        }

        private static void BuildSuperBowlRound(Season? currentSeason, MasterBracket? bracket, BracketViewModel bracketPrediction)
        {
            if (currentSeason is null || bracket is null)
            {
                return;
            }

            if (!bracket.Picks.Any(x => x.RoundNumber == 3))
            {
                return;
            }

            RoundModel? afcConferenceRoundFromDb = bracketPrediction.Bracket.AfcRounds.FirstOrDefault(x => x.RoundNumber == 3);
            RoundModel? nfcConferenceRoundFromDb = bracketPrediction.Bracket.NfcRounds.FirstOrDefault(x => x.RoundNumber == 3);

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
            MasterBracket? existingBracket = season.Bracket;

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

        private void UpdateScores(Season season)
        {
            List<Bracket> brackets = this.DbContext.Brackets.Where(x => x.SeasonYear == season.Year).ToList();
            MasterBracket? masterBracket = season.Bracket;
            if (masterBracket is null)
            {
                return;
            }

            foreach (Bracket? bracket in brackets)
            {
                bracket.CalculateScores(masterBracket, season);
            }

            this.DbContext.SaveChanges();
        }

        private int GetTimeZoneOffsetFromClaims()
        {
            System.Security.Claims.Claim? claim = this.User.FindFirst("TimeZoneOffset");
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
