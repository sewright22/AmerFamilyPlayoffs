// <copyright file="TeamController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Areas.Admin.ViewModels;
    using NflPlayoffPool.Web.Extensions;

    [Area("Admin")]
    public class TeamController : Controller
    {
        public ILogger<TeamController> Logger { get; }
        public PlayoffPoolContext DbContext { get; }

        public TeamController(ILogger<TeamController> logger, PlayoffPoolContext dbContext)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.Logger = logger;
            this.DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Create(string seasonId)
        {
            var model = new CreateTeamViewModel()
            {
                Team = new TeamModel()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    SeasonId = seasonId,
                },

                Teams = this.DbContext.GetTeamsAsSelectListItem(),
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Create(TeamModel team)
        {
            if (ModelState.IsValid)
            {
                var season = this.DbContext.Seasons.FirstOrDefault(x => x.Id.ToString() == team.SeasonId);
                var fullTeam = this.DbContext.GetTeamByCode(team.Code);
                if (season != null)
                {
                    season.Teams.Add(new PlayoffTeam
                    {
                        Id = team.Id,
                        City = fullTeam.City,
                        Name = fullTeam.Name,
                        Code = fullTeam.Code,
                        Conference = fullTeam.Conference,
                        Division = fullTeam.Division,
                        Seed = team.Seed,
                    });

                    this.DbContext.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Season not found.");
                }
            }
            return this.RedirectToAction(nameof(SeasonController.Details), nameof(SeasonController).GetControllerNameForUri(), new { id = team.SeasonId });
        }

        [HttpGet]
        public IActionResult Edit(string id, string seasonId)
        {
            var team = this.DbContext.Seasons.FirstOrDefault(x => x.Id.ToString() == seasonId)?.Teams.FirstOrDefault(x => x.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            var model = new CreateTeamViewModel()
            {
                Team = new TeamModel()
                {
                    Id = team.Id,
                    SeasonId = seasonId,
                    Code = team.Code,
                    Seed = team.Seed
                },
                Teams = this.DbContext.GetTeamsAsSelectListItem(),
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Edit(TeamModel team)
        {
            if (ModelState.IsValid)
            {
                var dbTeam = this.DbContext.Seasons.FirstOrDefault(x => x.Id.ToString() == team.SeasonId)?.Teams.FirstOrDefault(x => x.Id == team.Id);
                var existingTeam = this.DbContext.GetTeamByCode(team.Code);

                if (dbTeam != null)
                {
                    dbTeam.Name = existingTeam.Name;
                    dbTeam.City = existingTeam.City;
                    dbTeam.Division = existingTeam.Division;
                    dbTeam.Conference = existingTeam.Conference;
                    dbTeam.Code = team.Code;
                    dbTeam.Seed = team.Seed;
                    this.DbContext.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Team not found.");
                }
            }
            return this.RedirectToAction(nameof(SeasonController.Details), nameof(SeasonController).GetControllerNameForUri(), new { id = team.SeasonId });
        }
    }
}
