namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ApiContextTest
    {
        protected ApiContextTest(DbContextOptions<AmerFamilyPlayoffContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<AmerFamilyPlayoffContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new AmerFamilyPlayoffContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                this.SeedConferences(context);
                this.SeedSeasons(context);
                this.SeedTeams(context);
                this.SeedSeasonTeams(context);
                this.SeedPlayoffs(context);
                this.SeedPlayoffTeams(context);
            }
        }

        public virtual void SeedConferences(AmerFamilyPlayoffContext context)
        {
            context.Conferences.Add(new Conference
            {
                Name = "AFC",
            });

            context.Conferences.Add(new Conference
            {
                Name = "NFC",
            });

            context.SaveChanges();
        }

        public virtual void SeedSeasons(AmerFamilyPlayoffContext context)
        {
            var seasonList = new List<Season>();
            var twentyEighteenSeason = new Season
            {
                Year = 2018,
                Description = "2018-2019"
            };

            var twentyNineteenSeason = new Season
            {
                Year = 2019,
                Description = "2019-2020"
            };
            var twentyTwentySeason = new Season
            {
                Year = 2020,
                Description = "2020-2021"
            };

            seasonList.Add(twentyEighteenSeason);
            seasonList.Add(twentyNineteenSeason);
            seasonList.Add(twentyTwentySeason);

            context.AddRange(seasonList);
            context.SaveChanges();
        }

        public virtual void SeedSeasonTeams(AmerFamilyPlayoffContext context)
        {
            var afcConferenceId = context.Conferences.FirstOrDefault(c => c.Name == "AFC").Id;
            var nfcConferenceId = context.Conferences.FirstOrDefault(c => c.Name == "NFC").Id;

            foreach (var season in context.Seasons)
            {
                SaveConferenceToTeam(context, "BAL", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "BUF", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "CIN", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "CLE", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "DEN", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "HOU", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "IND", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "JAX", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "KC", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "LAC", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "LV", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "MIA", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "NE", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "NYJ", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "PIT", afcConferenceId, season.Id);
                SaveConferenceToTeam(context, "TEN", afcConferenceId, season.Id);

                SaveConferenceToTeam(context, "ARI", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "ATL", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "CAR", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "CHI", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "DAL", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "DET", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "GB", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "LAR", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "MIN", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "NO", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "NYG", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "PHI", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "SEA", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "SF", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "TB", nfcConferenceId, season.Id);
                SaveConferenceToTeam(context, "WAS", nfcConferenceId, season.Id);
            }

            context.SaveChanges();
        }

        private static void SaveConferenceToTeam(AmerFamilyPlayoffContext context, string abbreviation, int conferenceId, int seasonId)
        {
            foreach (var team in context.Teams.Where(st => st.Abbreviation == abbreviation).ToList())
            {
                context.Add(new SeasonTeam
                {
                    SeasonId = seasonId,
                    Team = team,
                    ConferenceId = conferenceId
                });
            }
        }

        public virtual void SeedTeams(AmerFamilyPlayoffContext context)
        {
            var teamList = new List<Team>();

            teamList.Add(new Team { Abbreviation = "ARI", Location = "Arizona", Name = "Cardinals" });
            teamList.Add(new Team { Abbreviation = "ATL", Location = "Atlanta", Name = "Falcons" });
            teamList.Add(new Team { Abbreviation = "BAL", Location = "Baltimore", Name = "Ravens" });
            teamList.Add(new Team { Abbreviation = "BUF", Location = "Buffalo", Name = "Bills" });
            teamList.Add(new Team { Abbreviation = "CAR", Location = "Carolina", Name = "Panthers" });
            teamList.Add(new Team { Abbreviation = "CHI", Location = "Chicago", Name = "Bears" });
            teamList.Add(new Team { Abbreviation = "CIN", Location = "Cincinnati", Name = "Bengals" });
            teamList.Add(new Team { Abbreviation = "CLE", Location = "Cleveland", Name = "Browns" });
            teamList.Add(new Team { Abbreviation = "DAL", Location = "Dallas", Name = "Cowboys" });
            teamList.Add(new Team { Abbreviation = "DEN", Location = "Denver", Name = "Broncos" });
            teamList.Add(new Team { Abbreviation = "DET", Location = "Detroit", Name = "Lions" });
            teamList.Add(new Team { Abbreviation = "GB", Location = "Green Bay", Name = "Packers" });
            teamList.Add(new Team { Abbreviation = "HOU", Location = "Houston", Name = "Texans" });
            teamList.Add(new Team { Abbreviation = "IND", Location = "Indianapolis", Name = "Colts" });
            teamList.Add(new Team { Abbreviation = "JAX", Location = "Jacksonville", Name = "Jaguars" });
            teamList.Add(new Team { Abbreviation = "KC", Location = "Kansas City", Name = "Chiefs" });
            teamList.Add(new Team { Abbreviation = "LAC", Location = "Los Angeles", Name = "Chargers" });
            teamList.Add(new Team { Abbreviation = "LAR", Location = "Los Angeles", Name = "Rams" });
            teamList.Add(new Team { Abbreviation = "LV", Location = "Las Vegas", Name = "Raiders" });
            teamList.Add(new Team { Abbreviation = "MIA", Location = "Miami", Name = "Dolphins" });
            teamList.Add(new Team { Abbreviation = "MIN", Location = "Minnesota", Name = "Vikings" });
            teamList.Add(new Team { Abbreviation = "NE", Location = "New England", Name = "Patriots" });
            teamList.Add(new Team { Abbreviation = "NO", Location = "New Orleans", Name = "Saints" });
            teamList.Add(new Team { Abbreviation = "NYG", Location = "New York", Name = "Giants" });
            teamList.Add(new Team { Abbreviation = "NYJ", Location = "New York", Name = "Jets" });
            teamList.Add(new Team { Abbreviation = "PHI", Location = "Philadelphia", Name = "Eagles" });
            teamList.Add(new Team { Abbreviation = "PIT", Location = "Pittsburgh", Name = "Steelers" });
            teamList.Add(new Team { Abbreviation = "SEA", Location = "Seattle", Name = "Seahawks" });
            teamList.Add(new Team { Abbreviation = "SF", Location = "San Francisco", Name = "49ers" });
            teamList.Add(new Team { Abbreviation = "TB", Location = "Tampa Bay", Name = "Buccaneers" });
            teamList.Add(new Team { Abbreviation = "TEN", Location = "Tennessee", Name = "Titans" });
            teamList.Add(new Team { Abbreviation = "WAS", Location = "Washington", Name = "Football Team" });

            context.AddRange(teamList);
            context.SaveChanges();
        }

        public virtual void SeedPlayoffs(AmerFamilyPlayoffContext context)
        {
            context.Add(new Playoff
            {
                Season = context.Seasons.FirstOrDefault(s => s.Year == 2019)
            });

            context.SaveChanges();
        }

        public virtual void SeedPlayoffTeams(AmerFamilyPlayoffContext context)
        {
            context.Add(new PlayoffTeam
            {
                PlayoffId = 1,
                SeasonTeamId = 2,
                Seed = 3,
            });

            context.SaveChanges();
        }
    }
}
