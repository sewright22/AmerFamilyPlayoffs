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
            // this.DataContext.Database.EnsureDeleted();
#endif
            this.DataContext.Database.Migrate();
            await this.SeedRole(Constants.Roles.Admin).ConfigureAwait(true);
            await this.SeedRole(Constants.Roles.Player).ConfigureAwait(true);
            await this.SeedAdminUser().ConfigureAwait(true);
            this.SeedSeasons();
            this.SeedTeams();
            this.SeedConferences();
            await this.SeedSeasonTeams().ConfigureAwait(false);
            this.SeedRounds();
            await this.SeedPlayoffs().ConfigureAwait(false);
            this.SeedPlayoffRounds();
            this.SeedPlayoffTeams();
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

        private void SeedConference(string conferenceName)
        {
            if (this.DataContext.Conferences.Any(x => x.Name == conferenceName))
            {
                return;
            }

            this.DataContext.Conferences.Add(new Conference()
            {
                Name = conferenceName,
            });
            this.DataContext.SaveChanges();
        }

        private void SeedConferences()
        {
            this.SeedConference("AFC");
            this.SeedConference("NFC");
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

            await this.DataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private void SeedPlayoffRounds()
        {
            foreach (var playoff in this.DataContext.Playoffs.ToList())
            {
                foreach (var round in this.DataContext.Rounds)
                {
                    this.DataContext.Add(new PlayoffRound
                    {
                        PlayoffId = playoff.Id,
                        RoundId = round.Id,
                        PointValue = round.Number,
                    });
                }
            }

            this.DataContext.SaveChanges();
        }

        private async Task SeedPlayoffs()
        {
            await this.SeedPlayoff(2021).ConfigureAwait(false);
        }

        private void SeedPlayoffTeam(int year, string abbreviation, int seed)
        {
            if (this.DataContext.PlayoffTeams.AsNoTracking().Any(x => x.Playoff.Season.Year == year && x.SeasonTeam.Team.Abbreviation == abbreviation))
            {
                return;
            }

            var playoff = this.DataContext.Playoffs.Single(x => x.Season.Year == year);
            var seasonTeam = this.DataContext.SeasonTeams.Single(x => x.Team.Abbreviation == abbreviation && x.Season.Year == year);

            this.DataContext.PlayoffTeams.Add(new PlayoffTeam
            {
                PlayoffId = playoff.Id,
                SeasonTeamId = seasonTeam.Id,
                Seed = seed,
            });

            this.DataContext.SaveChanges();
        }

        private void SeedPlayoffTeams()
        {
            int year = 2021;

            this.SeedPlayoffTeam(year, "TEN", 1);
            this.SeedPlayoffTeam(year, "KC", 2);
            this.SeedPlayoffTeam(year, "BUF", 3);
            this.SeedPlayoffTeam(year, "CIN", 4);
            this.SeedPlayoffTeam(year, "LV", 5);
            this.SeedPlayoffTeam(year, "NE", 6);
            this.SeedPlayoffTeam(year, "PIT", 7);

            this.SeedPlayoffTeam(year, "GB", 1);
            this.SeedPlayoffTeam(year, "TB", 2);
            this.SeedPlayoffTeam(year, "DAL", 3);
            this.SeedPlayoffTeam(year, "LAR", 4);
            this.SeedPlayoffTeam(year, "ARI", 5);
            this.SeedPlayoffTeam(year, "SF", 6);
            this.SeedPlayoffTeam(year, "PHI", 7);
        }

        private async Task SeedRole(string role)
        {
            if (await this.RoleManager.RoleExistsAsync(role).ConfigureAwait(false) == false)
            {
                await this.RoleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
            }
        }

        private void SeedRound(int roundNumber, string roundName)
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

            this.DataContext.SaveChanges();
        }

        private void SeedRounds()
        {
            this.SeedRound(1, "Wild Card");
            this.SeedRound(2, "Divisional");
            this.SeedRound(3, "Conference Championship");
            this.SeedRound(4, "Super Bowl");
        }

        private void SeedSeason(int year)
        {
            if (this.DataContext.Seasons.AsNoTracking().Any(x => x.Year == year) == false)
            {
                this.DataContext.Seasons.Add(new Season
                {
                    Year = year,
                    Description = $"{year}-{year + 1}",
                });

                this.DataContext.SaveChanges();
            }
        }

        private void SeedSeasons()
        {
            this.SeedSeason(2018);
            this.SeedSeason(2019);
            this.SeedSeason(2020);
            this.SeedSeason(2021);
        }

        private async Task SeedSeasonTeam(string teamAbbreviation, string conferenceName, int seasonId)
        {
            var conference = await this.DataContext.Conferences.AsNoTracking().FirstOrDefaultAsync(x => x.Name == conferenceName).ConfigureAwait(false);
            var team = await this.DataContext.Teams.AsNoTracking().FirstOrDefaultAsync(x => x.Abbreviation == teamAbbreviation).ConfigureAwait(false);

            if (team == null)
            {
                return;
            }

            this.DataContext.Add(new SeasonTeam
            {
                SeasonId = seasonId,
                TeamId = team.Id,
                ConferenceId = conference.Id,
            });

            this.DataContext.SaveChanges();
        }

        private async Task SeedSeasonTeams()
        {
            foreach (var season in this.DataContext.Seasons.AsNoTracking().ToList())
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

        private void SeedTeam(string abbreviation, string location, string name)
        {
            if (this.DataContext.Teams.AsNoTracking().Any(x => x.Abbreviation == abbreviation) == false)
            {
                this.DataContext.Teams.Add(new Team
                {
                    Abbreviation = abbreviation,
                    Location = location,
                    Name = name,
                });

                this.DataContext.SaveChanges();
            }

        }

        private void SeedTeams()
        {
            this.SeedTeam("ARI", "Arizona", "Cardinals");
            this.SeedTeam("ATL", "Atlanta", "Falcons");
            this.SeedTeam("BAL", "Baltimore", "Ravens");
            this.SeedTeam("BUF", "Buffalo", "Bills");
            this.SeedTeam("CAR", "Carolina", "Panthers");
            this.SeedTeam("CHI", "Chicago", "Bears");
            this.SeedTeam("CIN", "Cincinnati", "Bengals");
            this.SeedTeam("CLE", "Cleveland", "Browns");
            this.SeedTeam("DAL", "Dallas", "Cowboys");
            this.SeedTeam("DEN", "Denver", "Broncos");
            this.SeedTeam("DET", "Detroit", "Lions");
            this.SeedTeam("GB", "Green Bay", "Packers");
            this.SeedTeam("HOU", "Houston", "Texans");
            this.SeedTeam("IND", "Indianapolis", "Colts");
            this.SeedTeam("JAX", "Jacksonville", "Jaguars");
            this.SeedTeam("KC", "Kansas City", "Chiefs");
            this.SeedTeam("LAC", "Los Angeles", "Chargers");
            this.SeedTeam("LAR", "Los Angeles", "Rams");
            this.SeedTeam("LV", "Las Vegas", "Raiders");
            this.SeedTeam("MIA", "Miami", "Dolphins");
            this.SeedTeam("MIN", "Minnesota", "Vikings");
            this.SeedTeam("NE", "New England", "Patriots");
            this.SeedTeam("NO", "New Orleans", "Saints");
            this.SeedTeam("NYG", "New York", "Giants");
            this.SeedTeam("NYJ", "New York", "Jets");
            this.SeedTeam("PHI", "Philadelphia", "Eagles");
            this.SeedTeam("PIT", "Pittsburgh", "Steelers");
            this.SeedTeam("SEA", "Seattle", "Seahawks");
            this.SeedTeam("SF", "San Francisco", "49ers");
            this.SeedTeam("TB", "Tampa Bay", "Buccaneers");
            this.SeedTeam("TEN", "Tennessee", "Titans");
            this.SeedTeam("WAS", "Washington", "Commanders");
        }
    }
}
