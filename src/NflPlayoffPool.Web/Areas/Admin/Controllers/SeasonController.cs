// <copyright file="SeasonController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
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
        /// Displays the season creation wizard.
        /// </summary>
        /// <returns>The view for the season wizard.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Wizard()
        {
            var model = new SeasonWizardViewModel
            {
                Year = DateTime.Now.Year,
                CutoffDateTime = DateTime.Now.AddDays(7)
            };
            return this.View(model);
        }

        /// <summary>
        /// Handles wizard step navigation and processing.
        /// </summary>
        /// <param name="model">The wizard model.</param>
        /// <param name="action">The action to perform (next, previous, finish).</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Wizard(SeasonWizardViewModel model, string action)
        {
            Logger.LogInformation("Wizard POST - Step: {Step}, Action: {Action}, Year: {Year}", 
                model.CurrentStep, action, model.Year);
            Logger.LogInformation("Scoring values - WC: {WC}, Div: {Div}, Conf: {Conf}, SB: {SB}", 
                model.WildcardPoints, model.DivisionalPoints, model.ConferencePoints, model.SuperBowlPoints);
            
            if (action == "previous")
            {
                model.CurrentStep = Math.Max(1, model.CurrentStep - 1);
                Logger.LogInformation("Moving to previous step: {Step}", model.CurrentStep);
                return this.View(model);
            }

            // Validate current step
            if (!ValidateWizardStep(model))
            {
                Logger.LogWarning("Validation failed for step {Step}, returning to same step", model.CurrentStep);
                return this.View(model);
            }

            if (action == "next")
            {
                // Handle step-specific processing
                await ProcessWizardStep(model);
                
                model.CurrentStep = Math.Min(model.TotalSteps, model.CurrentStep + 1);
                Logger.LogInformation("Moving to next step: {Step}", model.CurrentStep);
                return this.View(model);
            }

            if (action == "finish")
            {
                try
                {
                    var seasonModel = model.ToSeasonModel();
                    var seasonId = await this.DbContext.Create(seasonModel);
                    
                    // Create teams if specified
                    if (model.Teams.Any())
                    {
                        await CreateWizardTeams(seasonId, model.Teams);
                    }
                    
                    this.Logger.LogInformation("Season created successfully via wizard with ID {SeasonId}", seasonId);
                    return this.RedirectToAction(nameof(this.Details), new { area = "Admin", id = seasonId });
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error creating season via wizard.");
                    this.ModelState.AddModelError(string.Empty, "An error occurred while creating the season.");
                    return this.View(model);
                }
            }

            return this.View(model);
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

        private bool ValidateWizardStep(SeasonWizardViewModel model)
        {
            Logger.LogInformation("Validating wizard step {Step} for year {Year}", model.CurrentStep, model.Year);
            Logger.LogInformation("Model values - WildcardPoints: {WildcardPoints}, DivisionalPoints: {DivisionalPoints}, ConferencePoints: {ConferencePoints}, SuperBowlPoints: {SuperBowlPoints}", 
                model.WildcardPoints, model.DivisionalPoints, model.ConferencePoints, model.SuperBowlPoints);
            
            switch (model.CurrentStep)
            {
                case 1: // Basic Info
                    if (model.Year < 2020 || model.Year > 2050)
                    {
                        ModelState.AddModelError(nameof(model.Year), "Year must be between 2020 and 2050");
                        Logger.LogWarning("Year validation failed: {Year}", model.Year);
                        return false;
                    }
                    if (model.CutoffDateTime <= DateTime.Now)
                    {
                        ModelState.AddModelError(nameof(model.CutoffDateTime), "Cutoff date must be in the future");
                        Logger.LogWarning("Cutoff date validation failed: {CutoffDateTime}", model.CutoffDateTime);
                        return false;
                    }
                    // Check if year already exists
                    if (DbContext.Seasons.Any(s => s.Year == model.Year))
                    {
                        ModelState.AddModelError(nameof(model.Year), $"A season for {model.Year} already exists");
                        Logger.LogWarning("Year already exists: {Year}", model.Year);
                        return false;
                    }
                    break;
                    
                case 2: // Scoring Rules
                    if (model.WildcardPoints < 1 || model.WildcardPoints > 20)
                    {
                        ModelState.AddModelError(nameof(model.WildcardPoints), "Wildcard points must be between 1 and 20");
                        Logger.LogWarning("Wildcard points validation failed: {WildcardPoints}", model.WildcardPoints);
                        return false;
                    }
                    if (model.DivisionalPoints < 1 || model.DivisionalPoints > 20)
                    {
                        ModelState.AddModelError(nameof(model.DivisionalPoints), "Divisional points must be between 1 and 20");
                        Logger.LogWarning("Divisional points validation failed: {DivisionalPoints}", model.DivisionalPoints);
                        return false;
                    }
                    if (model.ConferencePoints < 1 || model.ConferencePoints > 20)
                    {
                        ModelState.AddModelError(nameof(model.ConferencePoints), "Conference points must be between 1 and 20");
                        Logger.LogWarning("Conference points validation failed: {ConferencePoints}", model.ConferencePoints);
                        return false;
                    }
                    if (model.SuperBowlPoints < 1 || model.SuperBowlPoints > 20)
                    {
                        ModelState.AddModelError(nameof(model.SuperBowlPoints), "Super Bowl points must be between 1 and 20");
                        Logger.LogWarning("Super Bowl points validation failed: {SuperBowlPoints}", model.SuperBowlPoints);
                        return false;
                    }
                    break;
                    
                case 3: // Teams Setup
                    if (model.TeamSetupMethod == TeamSetupMethod.ManualEntry && model.Teams.Count != 14)
                    {
                        ModelState.AddModelError(string.Empty, "You must add exactly 14 teams (7 AFC, 7 NFC)");
                        Logger.LogWarning("Teams count validation failed: {TeamCount}", model.Teams.Count);
                        return false;
                    }
                    break;
            }
            
            bool isValid = ModelState.IsValid;
            Logger.LogInformation("Step {Step} validation result: {IsValid}", model.CurrentStep, isValid);
            
            if (!isValid)
            {
                foreach (var error in ModelState)
                {
                    Logger.LogWarning("ModelState error - Key: {Key}, Errors: {Errors}", 
                        error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }
            }
            
            return isValid;
        }

        private async Task ProcessWizardStep(SeasonWizardViewModel model)
        {
            switch (model.CurrentStep)
            {
                case 2: // Moving from Basic Info to Scoring Rules
                    // Could add logic to suggest point values based on historical data
                    break;
                    
                case 3: // Moving from Scoring Rules to Teams Setup
                    if (model.TeamSetupMethod == TeamSetupMethod.UseTemplate)
                    {
                        model.Teams = GetNflTeamsTemplate();
                    }
                    break;
            }
        }

        private async Task CreateWizardTeams(string seasonId, List<TeamWizardModel> teams)
        {
            var season = await DbContext.Seasons.FirstOrDefaultAsync(s => s.Id == seasonId);
            if (season == null) return;

            foreach (var teamModel in teams)
            {
                var team = new PlayoffTeam
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = teamModel.Name,
                    Code = teamModel.Abbreviation, // PlayoffTeam uses Code, not Abbreviation
                    Conference = teamModel.Conference,
                    Seed = teamModel.Seed,
                    City = teamModel.City ?? string.Empty
                    // Note: PrimaryColor is not available in PlayoffTeam model
                };
                
                season.Teams.Add(team);
            }
            
            await DbContext.SaveChangesAsync();
        }

        private List<TeamWizardModel> GetNflTeamsTemplate()
        {
            return new List<TeamWizardModel>
            {
                // AFC Playoff Teams (7 teams, seeds 1-7)
                new() { Name = "Buffalo Bills", Abbreviation = "BUF", Conference = Conference.AFC, Seed = 1, City = "Buffalo" },
                new() { Name = "Kansas City Chiefs", Abbreviation = "KC", Conference = Conference.AFC, Seed = 2, City = "Kansas City" },
                new() { Name = "Baltimore Ravens", Abbreviation = "BAL", Conference = Conference.AFC, Seed = 3, City = "Baltimore" },
                new() { Name = "Houston Texans", Abbreviation = "HOU", Conference = Conference.AFC, Seed = 4, City = "Houston" },
                new() { Name = "Pittsburgh Steelers", Abbreviation = "PIT", Conference = Conference.AFC, Seed = 5, City = "Pittsburgh" },
                new() { Name = "Los Angeles Chargers", Abbreviation = "LAC", Conference = Conference.AFC, Seed = 6, City = "Los Angeles" },
                new() { Name = "Denver Broncos", Abbreviation = "DEN", Conference = Conference.AFC, Seed = 7, City = "Denver" },
                
                // NFC Playoff Teams (7 teams, seeds 1-7)
                new() { Name = "Detroit Lions", Abbreviation = "DET", Conference = Conference.NFC, Seed = 1, City = "Detroit" },
                new() { Name = "Philadelphia Eagles", Abbreviation = "PHI", Conference = Conference.NFC, Seed = 2, City = "Philadelphia" },
                new() { Name = "Los Angeles Rams", Abbreviation = "LAR", Conference = Conference.NFC, Seed = 3, City = "Los Angeles" },
                new() { Name = "Atlanta Falcons", Abbreviation = "ATL", Conference = Conference.NFC, Seed = 4, City = "Atlanta" },
                new() { Name = "Minnesota Vikings", Abbreviation = "MIN", Conference = Conference.NFC, Seed = 5, City = "Minneapolis" },
                new() { Name = "Washington Commanders", Abbreviation = "WAS", Conference = Conference.NFC, Seed = 6, City = "Washington" },
                new() { Name = "Green Bay Packers", Abbreviation = "GB", Conference = Conference.NFC, Seed = 7, City = "Green Bay" }
            };
        }
    }
}
