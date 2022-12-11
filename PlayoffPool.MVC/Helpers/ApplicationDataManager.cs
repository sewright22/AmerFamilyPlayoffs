using System.Diagnostics;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlayoffPool.MVC.Helpers
{
    public class ApplicationDataManager : IDataManager
    {
        public ApplicationDataManager(
            AmerFamilyPlayoffContext dataContext, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<User> signInManager, 
            IConfiguration configuration)
        {
            this.DataContext = dataContext;
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.SignInManager = signInManager;
            this.Configuration = configuration;
        }

        public UserManager<User> UserManager { get; }

        public SignInManager<User> SignInManager { get; }
        public IConfiguration Configuration { get; }
        public AmerFamilyPlayoffContext DataContext { get; }

        public RoleManager<IdentityRole> RoleManager { get; }

        public async virtual Task Seed()
        {
#if DEBUG
            await this.DataContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
#endif
            await this.DataContext.Database.MigrateAsync().ConfigureAwait(false);
            await this.SeedRole(Constants.Roles.Admin).ConfigureAwait(false);
            await this.SeedRole(Constants.Roles.Player).ConfigureAwait(false);
            await this.SeedAdminUser().ConfigureAwait(false);
            await this.SeedSeasons().ConfigureAwait(false);
            await this.SeedTeams().ConfigureAwait(false);
            await this.DataContext.SaveChangesAsync().ConfigureAwait(false);
#if DEBUG
            await this.SeedPlayerUser().ConfigureAwait(false);
#endif
        }

        private async Task SeedAdminUser()
        {
            var seedDataSection = this.Configuration.GetSection("SeedData");
            var adminUser = seedDataSection.GetSection("AdminUser");

            var userToAdd = new User
            {
                UserName = adminUser["Email"],
                Email = adminUser["Email"],
                FirstName = adminUser["FirstName"],
                LastName = adminUser["LastName"],
            };

            var result = await this.UserManager.CreateAsync(userToAdd, adminUser["Password"]).ConfigureAwait(false);

            if (result.Succeeded)
            {
                await this.UserManager.AddToRoleAsync(userToAdd, Constants.Roles.Admin).ConfigureAwait(false);
            }
        }

        private async Task SeedPlayerUser()
        {
            var seedDataSection = this.Configuration.GetSection("SeedData");

            var userToAdd = new User
            {
                UserName = "player@email.com",
                Email = "player@email.com",
                FirstName = "Player",
                LastName = "User",
            };

            var result = await this.UserManager.CreateAsync(userToAdd, "P@ssword!23").ConfigureAwait(false);

            if (result.Succeeded)
            {
                await this.UserManager.AddToRoleAsync(userToAdd, Constants.Roles.Player).ConfigureAwait(false);
            }
        }

        private async Task SeedRole(string role)
        {
            if (await this.RoleManager.RoleExistsAsync(role).ConfigureAwait(false) == false)
            {
                await this.RoleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
            }
        }

        private async Task SeedSeason(int year)
        {
            if (this.DataContext.Seasons.Any(x=>x.Year==year) == false)
            {
                this.DataContext.Seasons.Add(new Season
                {
                    Year = year,
                    Description = $"{year}-{year + 1}",
                });
            }
        }

        private async Task SeedSeasons()
        {
            await this.SeedSeason(2018).ConfigureAwait(false);
            await this.SeedSeason(2019).ConfigureAwait(false);
            await this.SeedSeason(2020).ConfigureAwait(false);
            await this.SeedSeason(2021).ConfigureAwait(false);
        }

        private async Task SeedTeam(string abbreviation, string location, string name)
        {
            if (this.DataContext.Teams.Any(x => x.Abbreviation == abbreviation) == false)
            {
                this.DataContext.Teams.Add(new Team
                {
                    Abbreviation= abbreviation,
                    Location= location,
                    Name= name,
                });
            }

        }

        private async Task SeedTeams()
        {
            await this.SeedTeam("ARI","Arizona", "Cardinals").ConfigureAwait(false);
            await this.SeedTeam("ATL","Atlanta", "Falcons").ConfigureAwait(false);
            await this.SeedTeam("BAL","Baltimore", "Ravens").ConfigureAwait(false);
            await this.SeedTeam("BUF","Buffalo", "Bills").ConfigureAwait(false);
            await this.SeedTeam("CAR","Carolina", "Panthers").ConfigureAwait(false);
            await this.SeedTeam("CHI","Chicago", "Bears").ConfigureAwait(false);
            await this.SeedTeam("CIN","Cincinnati", "Bengals").ConfigureAwait(false);
            await this.SeedTeam("CLE","Cleveland", "Browns").ConfigureAwait(false);
            await this.SeedTeam("DAL","Dallas", "Cowboys").ConfigureAwait(false);
            await this.SeedTeam("DEN","Denver", "Broncos").ConfigureAwait(false);
            await this.SeedTeam("DET","Detroit", "Lions").ConfigureAwait(false);
            await this.SeedTeam("GB", "Green Bay", "Packers").ConfigureAwait(false);
            await this.SeedTeam("HOU","Houston", "Texans").ConfigureAwait(false);
            await this.SeedTeam("IND","Indianapolis", "Colts").ConfigureAwait(false);
            await this.SeedTeam("JAX","Jacksonville", "Jaguars").ConfigureAwait(false);
            await this.SeedTeam("KC", "Kansas City", "Chiefs").ConfigureAwait(false);
            await this.SeedTeam("LAC","Los Angeles", "Chargers").ConfigureAwait(false);
            await this.SeedTeam("LAR","Los Angeles", "Rams").ConfigureAwait(false);
            await this.SeedTeam("LV", "Las Vegas", "Raiders").ConfigureAwait(false);
            await this.SeedTeam("MIA","Miami", "Dolphins").ConfigureAwait(false);
            await this.SeedTeam("MIN","Minnesota", "Vikings").ConfigureAwait(false);
            await this.SeedTeam("NE", "New England", "Patriots").ConfigureAwait(false);
            await this.SeedTeam("NO", "New Orleans", "Saints").ConfigureAwait(false);
            await this.SeedTeam("NYG","New York", "Giants").ConfigureAwait(false);
            await this.SeedTeam("NYJ","New York", "Jets").ConfigureAwait(false);
            await this.SeedTeam("PHI","Philadelphia", "Eagles").ConfigureAwait(false);
            await this.SeedTeam("PIT","Pittsburgh", "Steelers").ConfigureAwait(false);
            await this.SeedTeam("SEA","Seattle", "Seahawks").ConfigureAwait(false);
            await this.SeedTeam("SF", "San Francisco", "49ers").ConfigureAwait(false);
            await this.SeedTeam("TB", "Tampa Bay", "Buccaneers").ConfigureAwait(false);
            await this.SeedTeam("TEN","Tennessee", "Titans").ConfigureAwait(false);
            await this.SeedTeam("WAS","Washington", "Commanders").ConfigureAwait(false);
        }
    }
}
