namespace AmerFamilyPlayoffs.Api.Seed
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class SeedExtension
    {
        public static void Seed(this AmerFamilyPlayoffContext context)
        {
            if (context.Seasons.Count() == 0)
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

            if (context.Teams.Count() == 0)
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

                foreach (var season in context.Seasons.ToList())
                {
                    foreach (var team in teamList)
                    {
                        context.Add(new SeasonTeam
                        {
                            Season = season,
                            Team = team,
                        });
                    }
                }

                context.SaveChanges();
            }

            if (context.Conferences.Any()==false)
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

            if (context.ConferenceTeams.Any() == false)
            {
                var afcConferenceId = context.Conferences.FirstOrDefault(c => c.Name == "AFC").Id;
                var nfcConferenceId = context.Conferences.FirstOrDefault(c => c.Name == "NFC").Id;
                SaveConferenceToTeam(context, "BAL", afcConferenceId);
                SaveConferenceToTeam(context, "BUF", afcConferenceId);
                SaveConferenceToTeam(context, "CIN", afcConferenceId);
                SaveConferenceToTeam(context, "CLE", afcConferenceId);
                SaveConferenceToTeam(context, "DEN", afcConferenceId);
                SaveConferenceToTeam(context, "HOU", afcConferenceId);
                SaveConferenceToTeam(context, "IND", afcConferenceId);
                SaveConferenceToTeam(context, "JAX", afcConferenceId);
                SaveConferenceToTeam(context, "KC", afcConferenceId);
                SaveConferenceToTeam(context, "LAC", afcConferenceId);
                SaveConferenceToTeam(context, "LV", afcConferenceId);
                SaveConferenceToTeam(context, "MIA", afcConferenceId);
                SaveConferenceToTeam(context, "NE", afcConferenceId);
                SaveConferenceToTeam(context, "NYJ", afcConferenceId);
                SaveConferenceToTeam(context, "PIT", afcConferenceId);
                SaveConferenceToTeam(context, "TEN", afcConferenceId);

                SaveConferenceToTeam(context, "ARI", nfcConferenceId);
                SaveConferenceToTeam(context, "ATL", nfcConferenceId);
                SaveConferenceToTeam(context, "CAR", nfcConferenceId);
                SaveConferenceToTeam(context, "CHI", nfcConferenceId);
                SaveConferenceToTeam(context, "DAL", nfcConferenceId);
                SaveConferenceToTeam(context, "DET", nfcConferenceId);
                SaveConferenceToTeam(context, "GB", nfcConferenceId);
                SaveConferenceToTeam(context, "LAR", nfcConferenceId);
                SaveConferenceToTeam(context, "MIN", nfcConferenceId);
                SaveConferenceToTeam(context, "NO", nfcConferenceId);
                SaveConferenceToTeam(context, "NYG", nfcConferenceId);
                SaveConferenceToTeam(context, "PHI", nfcConferenceId);
                SaveConferenceToTeam(context, "SEA", nfcConferenceId);
                SaveConferenceToTeam(context, "SF", nfcConferenceId);
                SaveConferenceToTeam(context, "TB", nfcConferenceId);
                SaveConferenceToTeam(context, "WAS", nfcConferenceId);

                context.SaveChanges();
            }

            //{
            //    Season = twentyNineteenSeason
            //};

            //entitiesToAdd.Add(twentyNineteenPlayoffs);

            //var wildCardRound = new Round
            //{
            //    Name = "Wild Card",
            //    Number = 1,
            //    Matchups = new List<Matchup>(),
            //};

            //wildCardRound.Matchups.Add(new Matchup
            //{
            //    AwayTeamId = 2,
            //    HomeTeamId = 4,
            //});

            //entitiesToAdd.Add(wildCardRound);

            //entitiesToAdd.Add(new PlayoffRound
            //{
            //    Playoff = twentyNineteenPlayoffs,
            //    Round = wildCardRound,
            //});
        }

        private static void SaveConferenceToTeam(AmerFamilyPlayoffContext context, string abbreviation, int conferenceId)
        {
            foreach (var seasonTeam in context.SeasonTeams.Include(st => st.Team).Where(st => st.Team.Abbreviation == abbreviation).ToList())
            {
                context.Add(new ConferenceTeam
                {
                    SeasonTeamId = seasonTeam.Id,
                    ConferenceId = conferenceId,
                });
            }
        }
    }
}
