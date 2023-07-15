namespace PlayoffPool.MVC.Controllers
{
    using AmerFamilyPlayoffs.Data;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using NuGet.Packaging;
    using PlayoffPool.MVC.Helpers;
    using PlayoffPool.MVC.Models;
    using PlayoffPool.MVC.Models.Admin;
    using PlayoffPool.MVC.Models.Bracket;
    using System;
    using System.Security.Claims;
    using System.Security.Principal;

    public class AdminController : Controller
    {
        public AdminController(ILogger<AdminController> logger, IDataManager dataManager, IMapper mapper)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.Logger = logger;
            this.DataManager = dataManager;
            this.Mapper = mapper;
        }

        public ILogger<AdminController> Logger { get; }
        public IDataManager DataManager { get; }
        public IMapper Mapper { get; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel();
            model.ManageUsersViewModel = new ManageUsersViewModel();
            model.ManageRolesViewModel = new ManageRolesViewModel();

            var users = await this.DataManager.DataContext.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);
            var roles = this.DataManager.RoleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();

            foreach (var role in roles)
            {
                model.ManageRolesViewModel.Roles.Add(new RoleModel
                {
                    Id = role.Value,
                    Name = role.Text,
                });
            }

            foreach (var user in users)
            {
                var userRoles = await this.DataManager.UserManager.GetRolesAsync(user).ConfigureAwait(false);

                model.ManageUsersViewModel.Users.Add(new Models.UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles,
                    RoleId = roles.Where(x => userRoles.Contains(x.Text)).Select(x => x.Value).FirstOrDefault(),
                });
            }

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageTeams(ManageTeamsViewModel ManageTeamsViewModel)
        {
            var model = ManageTeamsViewModel;

            if (string.IsNullOrEmpty(model.Year) == false)
            {
                var teams = this.DataManager.DataContext.PlayoffTeams
                    .Where(x => x.SeasonTeam.Season.Year.ToString() == model.Year).ProjectTo<PlayoffTeamViewModel>(this.Mapper.ConfigurationProvider).ToList();

                var rounds = this.DataManager.DataContext.PlayoffRounds.Include("Round")
                    .Include(x => x.RoundWinners)
                    .ThenInclude(x => x.PlayoffTeam)
                    .Where(x => x.Playoff.Season.Year.ToString() == model.Year)
                    .OrderBy(x => x.Round.Number);

                foreach (var round in rounds)
                {
                    var vm = new AdminRoundViewModel();
                    vm.Teams = new List<PlayoffTeamViewModel>(teams.Select(x => this.Mapper.Map<PlayoffTeamViewModel>(x)));
                    if (round.RoundWinners.Any())
                    {
                        vm.Teams.ForEach(team =>
                        {
                            if (round.RoundWinners.Any(x => x.PlayoffTeamId == team.Id))
                            {
                                team.Selected = true;
                            }
                        });
                    }

                    vm.Id = round.Id;
                    vm.Name = round.Round.Name;
                    vm.Number = round.Round.Number;
                    vm.PointValue = round.PointValue;
                    model.Rounds.Add(vm);
                }
            }

            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveTeams(ManageTeamsViewModel ManageTeamsViewModel)
        {
            var model = ManageTeamsViewModel;

            if (ModelState.IsValid)
            {
                foreach (var roundViewModel in model.Rounds)
                {
                    var dbRound = this.DataManager.DataContext.PlayoffRounds.Include(x => x.RoundWinners).FirstOrDefault(x => x.Id == roundViewModel.Id);
                    var selectedWinners = roundViewModel.Teams.Where(x => x.Selected).Select(x => x.Id).ToList();
                    dbRound.PointValue = roundViewModel.PointValue;
                    dbRound.RoundWinners.Clear();
                    dbRound.RoundWinners.AddRange(selectedWinners.Select(x => new RoundWinner
                    {
                        PlayoffTeamId = x,
                    }));
                }

                this.DataManager.DataContext.SaveChanges();
            }

            model.Rounds.Clear();
            return this.RedirectToAction(nameof(this.ManageTeams), model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUsers(ManageUsersViewModel model)
        {
            if (this.ModelState.IsValid == false)
            {
                return this.View(model);
            }

            try
            {
                foreach (var modelUser in model.Users)
                {
                    var dbUser = await this.DataManager.UserManager.FindByIdAsync(modelUser.Id).ConfigureAwait(false);

                    this.Logger.LogDebug($"Got user from db: {dbUser.Id}");

                    if (dbUser == null)
                    {
                        return this.View(model);
                    }

                    dbUser.FirstName = modelUser.FirstName;
                    dbUser.LastName = modelUser.LastName;
                    dbUser.Email = modelUser.Email;

                    await this.DataManager.UserManager.UpdateAsync(dbUser).ConfigureAwait(false);

                    this.Logger.LogDebug($"Updated user info.");

                    var userRoles = await this.DataManager.UserManager.GetRolesAsync(dbUser).ConfigureAwait(false);

                    this.Logger.LogDebug($"Got roles for user.");

                    foreach (var role in userRoles)
                    {
                        this.Logger.LogDebug($"Role: {role})");
                    }

                    if (userRoles.Contains(modelUser.RoleId))
                    {
                        this.Logger.LogDebug($"User already contains role.)");
                        return this.View(model);
                    }

                    if (userRoles.Any())
                    {
                        var firstRoleForUser = userRoles.First();
                        this.Logger.LogDebug($"First role: {firstRoleForUser}");
                        var result = await this.DataManager.UserManager.RemoveFromRoleAsync(dbUser, userRoles.First()).ConfigureAwait(false);

                        if (result.Succeeded)
                        {
                            this.Logger.LogDebug($"Removed role.");
                            var newRole = await this.DataManager.RoleManager.FindByIdAsync(modelUser.RoleId).ConfigureAwait(false);
                            await this.DataManager.UserManager.AddToRoleAsync(dbUser, newRole.Name).ConfigureAwait(false);
                            this.Logger.LogDebug($"Added new role: {newRole.Name}");
                        }
                        else
                        {
                            this.Logger.LogDebug($"Unable to remove role.");
                        }
                    }
                    else
                    {
                        this.Logger.LogDebug($"No roles found.");
                    }

                    await this.DataManager.DataContext.SaveChangesAsync().ConfigureAwait(false);

                    if (modelUser.ShouldResetPassword)
                    {
                        var removeResult = await this.DataManager.UserManager.RemovePasswordAsync(dbUser);

                        if (removeResult.Errors.Any())
                        {
                            this.ModelState.AddModelError(modelUser.Id, removeResult.Errors.FirstOrDefault().Description);
                            return this.View(model);
                        }

                        var result = await this.DataManager.UserManager.AddPasswordAsync(dbUser, "Password#123").ConfigureAwait(false);

                        if (result.Errors.Any())
                        {
                            this.ModelState.AddModelError(modelUser.Id, result.Errors.FirstOrDefault().Description);
                            return this.View(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateRoles(RoleModel model)
        {
            return this.View(model);
        }

        private Task Seed()
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}