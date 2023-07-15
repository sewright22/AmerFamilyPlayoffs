using System.Diagnostics;
using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            this.SeedSeasonTeams();
            this.SeedRounds();
            this.SeedPlayoffs();
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

        private void SeedPlayoff(int year)
        {
            if (this.DataContext.Playoffs.Any(x => x.Season.Year == year))
            {
                return;
            }

            this.DataContext.Playoffs.Add(new Playoff()
            {
                SeasonId = this.DataContext.Seasons.Single(x => x.Year == year).Id,
            });

            this.DataContext.SaveChanges();
        }

        private void SeedPlayoffRounds()
        {
            foreach (var playoff in this.DataContext.Playoffs.AsNoTracking().Where(x=>x.PlayoffRounds.Any() == false).ToList())
            {
                foreach (var round in this.DataContext.Rounds.AsNoTracking().ToList())
                {
                    this.SeedPlayoffRound(playoff.Id, round.Id, this.GetRoundPointValue(round.Number));
                }
            }
        }

        private int GetRoundPointValue(int number)
        {
            switch (number)
            {
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 5;
                case 4:
                    return 8;
                default:
                    return 0;
            }
        }

        private void SeedPlayoffRound(int playoffId, int roundId, int pointValue)
        {
            if (this.DataContext.PlayoffRounds.AsNoTracking().Any(x => x.PlayoffId == playoffId && x.RoundId == roundId))
            {
                return;
            }

            this.DataContext.Add(new PlayoffRound
            {
                PlayoffId = playoffId,
                RoundId = roundId,
                PointValue = pointValue,
            });

            this.DataContext.SaveChanges();
        }

        private void SeedPlayoffs()
        {
            this.SeedPlayoff(2021);
            this.SeedPlayoff(2022);
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

            year = 2022;

            this.SeedPlayoffTeam(year, "KC", 1);
            this.SeedPlayoffTeam(year, "BUF", 2);
            this.SeedPlayoffTeam(year, "CIN", 3);
            this.SeedPlayoffTeam(year, "JAX", 4);
            this.SeedPlayoffTeam(year, "LAC", 5);
            this.SeedPlayoffTeam(year, "BAL", 6);
            this.SeedPlayoffTeam(year, "MIA", 7);

            this.SeedPlayoffTeam(year, "PHI", 1);
            this.SeedPlayoffTeam(year, "SF", 2);
            this.SeedPlayoffTeam(year, "MIN", 3);
            this.SeedPlayoffTeam(year, "TB", 4);
            this.SeedPlayoffTeam(year, "DAL", 5);
            this.SeedPlayoffTeam(year, "NYG", 6);
            this.SeedPlayoffTeam(year, "SEA", 7);
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
            if (this.DataContext.Rounds.FirstOrDefault(x => x.Name == roundName) is not null)
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
            this.SeedSeason(2022);
        }

        private void SeedSeasonTeam(string teamAbbreviation, string conferenceName, int seasonId)
        {
            var conference = this.DataContext.Conferences.AsNoTracking().FirstOrDefault(x => x.Name == conferenceName);
            var team = this.DataContext.Teams.AsNoTracking().FirstOrDefault(x => x.Abbreviation == teamAbbreviation);

            if (team == null)
            {
                return;
            }

            var existingRecord = this.DataContext.SeasonTeams.FirstOrDefault(x=>x.SeasonId==seasonId && x.TeamId == team.Id && x.ConferenceId == conference.Id);

            if (existingRecord != null)
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

        private void SeedSeasonTeams()
        {
            foreach (var season in this.DataContext.Seasons.AsNoTracking().ToList())
            {
                this.SeedSeasonTeam("BAL", "AFC", season.Id);
                this.SeedSeasonTeam("BUF", "AFC", season.Id);
                this.SeedSeasonTeam("CIN", "AFC", season.Id);
                this.SeedSeasonTeam("CLE", "AFC", season.Id);
                this.SeedSeasonTeam("DEN", "AFC", season.Id);
                this.SeedSeasonTeam("HOU", "AFC", season.Id);
                this.SeedSeasonTeam("IND", "AFC", season.Id);
                this.SeedSeasonTeam("JAX", "AFC", season.Id);
                this.SeedSeasonTeam("KC", "AFC", season.Id);
                this.SeedSeasonTeam("LAC", "AFC", season.Id);
                this.SeedSeasonTeam("LV", "AFC", season.Id);
                this.SeedSeasonTeam("MIA", "AFC", season.Id);
                this.SeedSeasonTeam("NE", "AFC", season.Id);
                this.SeedSeasonTeam("NYJ", "AFC", season.Id);
                this.SeedSeasonTeam("PIT", "AFC", season.Id);
                this.SeedSeasonTeam("TEN", "AFC", season.Id);

                this.SeedSeasonTeam("ARI", "NFC", season.Id);
                this.SeedSeasonTeam("ATL", "NFC", season.Id);
                this.SeedSeasonTeam("CAR", "NFC", season.Id);
                this.SeedSeasonTeam("CHI", "NFC", season.Id);
                this.SeedSeasonTeam("DAL", "NFC", season.Id);
                this.SeedSeasonTeam("DET", "NFC", season.Id);
                this.SeedSeasonTeam("GB", "NFC", season.Id);
                this.SeedSeasonTeam("LAR", "NFC", season.Id);
                this.SeedSeasonTeam("MIN", "NFC", season.Id);
                this.SeedSeasonTeam("NO", "NFC", season.Id);
                this.SeedSeasonTeam("NYG", "NFC", season.Id);
                this.SeedSeasonTeam("PHI", "NFC", season.Id);
                this.SeedSeasonTeam("SEA", "NFC", season.Id);
                this.SeedSeasonTeam("SF", "NFC", season.Id);
                this.SeedSeasonTeam("TB", "NFC", season.Id);
                this.SeedSeasonTeam("WAS", "NFC", season.Id);
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
