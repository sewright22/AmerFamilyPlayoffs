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
            await this.SeedConferences().ConfigureAwait(false);
            await this.SeedSeasonTeams().ConfigureAwait(false);
            await this.DataContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SeedRounds().ConfigureAwait(false);
            await this.SeedPlayoffs().ConfigureAwait(false);
            await this.DataContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SeedPlayoffTeams().ConfigureAwait(false);
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

        private async Task SeedConference(string conferenceName)
        {
            if (this.DataContext.Conferences.Any(x => x.Name == conferenceName))
            {
                return;
            }

            await this.DataContext.Conferences.AddAsync(new Conference()
            {
                Name = conferenceName,
            }).ConfigureAwait(false);
        }

        private async Task SeedConferences()
        {
            await this.SeedConference("AFC").ConfigureAwait(false);
            await this.SeedConference("NFC").ConfigureAwait(false);
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

        private async Task SeedPlayoff(int year)
        {
            if (this.DataContext.Playoffs.Any(x => x.Season.Year == year))
            {
                return;
            }

            this.DataContext.Playoffs.Add(new Playoff()
            {
                Season = await this.DataContext.Seasons.SingleAsync(x => x.Year == year).ConfigureAwait(false),
            });
        }

        private async Task SeedPlayoffs()
        {
            await this.SeedPlayoff(2021).ConfigureAwait(false);
        }

        private async Task SeedPlayoffTeam(int year, string abbreviation, int seed)
        {
            if (await this.DataContext.PlayoffTeams.AnyAsync(x=>x.Playoff.Season.Year==year && x.SeasonTeam.Team.Abbreviation == abbreviation).ConfigureAwait(false))
            {
                return;
            }

            var playoff = await this.DataContext.Playoffs.ToListAsync().ConfigureAwait(false);
            var seasonTeam = await this.DataContext.SeasonTeams.ToListAsync().ConfigureAwait(false);

            this.DataContext.PlayoffTeams.Add(new PlayoffTeam
            {
                Playoff = await this.DataContext.Playoffs.SingleAsync(x => x.Season.Year == year).ConfigureAwait(false),
                SeasonTeam = await this.DataContext.SeasonTeams.SingleAsync(x => x.Team.Abbreviation == abbreviation).ConfigureAwait(false),
                Seed = seed,
            });
        }

        private async Task SeedPlayoffTeams()
        {
            int year = 2021;

            await this.SeedPlayoffTeam(year, "TEN", 1).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "KC", 2).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "BUF", 3).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "CIN", 4).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "LV", 5).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "NE", 6).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "PIT", 7).ConfigureAwait(false);

            await this.SeedPlayoffTeam(year, "GB", 1).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "TB", 2).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "DAL", 3).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "LAR", 4).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "ARI", 5).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "SF", 6).ConfigureAwait(false);
            await this.SeedPlayoffTeam(year, "PHI", 7).ConfigureAwait(false);
        }

        private async Task SeedRole(string role)
        {
            if (await this.RoleManager.RoleExistsAsync(role).ConfigureAwait(false) == false)
            {
                await this.RoleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
            }
        }

        private async Task SeedRound(int roundNumber, string roundName)
        {
            if (this.DataContext.Rounds.Any(x => x.Name == roundName))
            {
                return;
            }

            this.DataContext.Rounds.Add(new Round()
            {
                Number = roundNumber,
                Name = roundName,
            });
        }

        private async Task SeedRounds()
        {
            await this.SeedRound(1, "Wild Card").ConfigureAwait(false);
            await this.SeedRound(2, "Divisional").ConfigureAwait(false);
            await this.SeedRound(3, "Conference Championship").ConfigureAwait(false);
            await this.SeedRound(4, "Super Bowl").ConfigureAwait(false);
        }

        private async Task SeedSeason(int year)
        {
            if (this.DataContext.Seasons.Any(x => x.Year == year) == false)
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

        private async Task SeedSeasonTeam(string teamAbbreviation, string conferenceName, int seasonId)
        {
            var conference = await this.DataContext.Conferences.FirstOrDefaultAsync(x => x.Name == conferenceName).ConfigureAwait(false);
            var team = await this.DataContext.Teams.FirstOrDefaultAsync(x => x.Abbreviation == teamAbbreviation).ConfigureAwait(false);

            if (team == null)
            {
                return;
            }

            this.DataContext.Add(new SeasonTeam
            {
                SeasonId = seasonId,
                Team = team,
                ConferenceId = conference.Id,
            });
        }

        private async Task SeedSeasonTeams()
        {
            foreach (var season in this.DataContext.Seasons.ToList())
            {
                await this.SeedSeasonTeam("BAL", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("BUF", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("CIN", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("CLE", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("DEN", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("HOU", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("IND", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("JAX", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("KC", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("LAC", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("LV", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("MIA", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("NE", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("NYJ", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("PIT", "AFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("TEN", "AFC", season.Id).ConfigureAwait(false);

                await this.SeedSeasonTeam("ARI", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("ATL", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("CAR", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("CHI", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("DAL", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("DET", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("GB", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("LAR", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("MIN", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("NO", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("NYG", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("PHI", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("SEA", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("SF", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("TB", "NFC", season.Id).ConfigureAwait(false);
                await this.SeedSeasonTeam("WAS", "NFC", season.Id).ConfigureAwait(false);
            }
        }

        private async Task SeedTeam(string abbreviation, string location, string name)
        {
            if (this.DataContext.Teams.Any(x => x.Abbreviation == abbreviation) == false)
            {
                this.DataContext.Teams.Add(new Team
                {
                    Abbreviation = abbreviation,
                    Location = location,
                    Name = name,
                });
            }

        }

        private async Task SeedTeams()
        {
            await this.SeedTeam("ARI", "Arizona", "Cardinals").ConfigureAwait(false);
            await this.SeedTeam("ATL", "Atlanta", "Falcons").ConfigureAwait(false);
            await this.SeedTeam("BAL", "Baltimore", "Ravens").ConfigureAwait(false);
            await this.SeedTeam("BUF", "Buffalo", "Bills").ConfigureAwait(false);
            await this.SeedTeam("CAR", "Carolina", "Panthers").ConfigureAwait(false);
            await this.SeedTeam("CHI", "Chicago", "Bears").ConfigureAwait(false);
            await this.SeedTeam("CIN", "Cincinnati", "Bengals").ConfigureAwait(false);
            await this.SeedTeam("CLE", "Cleveland", "Browns").ConfigureAwait(false);
            await this.SeedTeam("DAL", "Dallas", "Cowboys").ConfigureAwait(false);
            await this.SeedTeam("DEN", "Denver", "Broncos").ConfigureAwait(false);
            await this.SeedTeam("DET", "Detroit", "Lions").ConfigureAwait(false);
            await this.SeedTeam("GB", "Green Bay", "Packers").ConfigureAwait(false);
            await this.SeedTeam("HOU", "Houston", "Texans").ConfigureAwait(false);
            await this.SeedTeam("IND", "Indianapolis", "Colts").ConfigureAwait(false);
            await this.SeedTeam("JAX", "Jacksonville", "Jaguars").ConfigureAwait(false);
            await this.SeedTeam("KC", "Kansas City", "Chiefs").ConfigureAwait(false);
            await this.SeedTeam("LAC", "Los Angeles", "Chargers").ConfigureAwait(false);
            await this.SeedTeam("LAR", "Los Angeles", "Rams").ConfigureAwait(false);
            await this.SeedTeam("LV", "Las Vegas", "Raiders").ConfigureAwait(false);
            await this.SeedTeam("MIA", "Miami", "Dolphins").ConfigureAwait(false);
            await this.SeedTeam("MIN", "Minnesota", "Vikings").ConfigureAwait(false);
            await this.SeedTeam("NE", "New England", "Patriots").ConfigureAwait(false);
            await this.SeedTeam("NO", "New Orleans", "Saints").ConfigureAwait(false);
            await this.SeedTeam("NYG", "New York", "Giants").ConfigureAwait(false);
            await this.SeedTeam("NYJ", "New York", "Jets").ConfigureAwait(false);
            await this.SeedTeam("PHI", "Philadelphia", "Eagles").ConfigureAwait(false);
            await this.SeedTeam("PIT", "Pittsburgh", "Steelers").ConfigureAwait(false);
            await this.SeedTeam("SEA", "Seattle", "Seahawks").ConfigureAwait(false);
            await this.SeedTeam("SF", "San Francisco", "49ers").ConfigureAwait(false);
            await this.SeedTeam("TB", "Tampa Bay", "Buccaneers").ConfigureAwait(false);
            await this.SeedTeam("TEN", "Tennessee", "Titans").ConfigureAwait(false);
            await this.SeedTeam("WAS", "Washington", "Commanders").ConfigureAwait(false);
        }
    }
}
