namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
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

                var entitiesToAdd = new List<object>();

                entitiesToAdd.Add(new Season
                {
                    Year = 2018,
                    Description = "2018-2019"
                });

                entitiesToAdd.Add(new Season
                {
                    Year = 2019,
                    Description = "2019-2020"
                });

                entitiesToAdd.Add(new Season
                {
                    Year = 2020,
                    Description = "2020-2021"
                });

                entitiesToAdd.Add(new Team { Abbreviation = "ARI", Location = "Arizona", Name = "Cardinals" });
                entitiesToAdd.Add(new Team { Abbreviation = "ATL", Location = "Atlanta", Name = "Falcons" });
                entitiesToAdd.Add(new Team { Abbreviation = "BAL", Location = "Baltimore", Name = "Ravens" });
                entitiesToAdd.Add(new Team { Abbreviation = "BUF", Location = "Buffalo", Name = "Bills" });
                entitiesToAdd.Add(new Team { Abbreviation = "CAR", Location = "Carolina", Name = "Panthers" });
                entitiesToAdd.Add(new Team { Abbreviation = "CHI", Location = "Chicago", Name = "Bears" });
                entitiesToAdd.Add(new Team { Abbreviation = "CIN", Location = "Cincinnati", Name = "Bengals" });
                entitiesToAdd.Add(new Team { Abbreviation = "CLE", Location = "Cleveland", Name = "Browns" });
                entitiesToAdd.Add(new Team { Abbreviation = "DAL", Location = "Dallas", Name = "Cowboys" });
                entitiesToAdd.Add(new Team { Abbreviation = "DEN", Location = "Denver", Name = "Broncos" });
                entitiesToAdd.Add(new Team { Abbreviation = "DET", Location = "Detroit", Name = "Lions" });
                entitiesToAdd.Add(new Team { Abbreviation = "GB", Location = "Green Bay", Name = "Packers" });
                entitiesToAdd.Add(new Team { Abbreviation = "HOU", Location = "Houston", Name = "Texans" });
                entitiesToAdd.Add(new Team { Abbreviation = "IND", Location = "Indianapolis", Name = "Colts" });
                entitiesToAdd.Add(new Team { Abbreviation = "JAX", Location = "Jacksonville", Name = "Jaguars" });
                entitiesToAdd.Add(new Team { Abbreviation = "KC", Location = "Kansas City", Name = "Chiefs" });
                entitiesToAdd.Add(new Team { Abbreviation = "LAC", Location = "Los Angeles", Name = "Chargers" });
                entitiesToAdd.Add(new Team { Abbreviation = "LAR", Location = "Los Angeles", Name = "Rams" });
                entitiesToAdd.Add(new Team { Abbreviation = "LV", Location = "Las Vegas", Name = "Raiders" });
                entitiesToAdd.Add(new Team { Abbreviation = "MIA", Location = "Miami", Name = "Dolphins" });
                entitiesToAdd.Add(new Team { Abbreviation = "MIN", Location = "Minnesota", Name = "Vikings" });
                entitiesToAdd.Add(new Team { Abbreviation = "NE", Location = "New England", Name = "Patriots" });
                entitiesToAdd.Add(new Team { Abbreviation = "NO", Location = "New Orleans", Name = "Saints" });
                entitiesToAdd.Add(new Team { Abbreviation = "NYG", Location = "New York", Name = "Giants" });
                entitiesToAdd.Add(new Team { Abbreviation = "NYJ", Location = "New York", Name = "Jets" });
                entitiesToAdd.Add(new Team { Abbreviation = "PHI", Location = "Philadelphia", Name = "Eagles" });
                entitiesToAdd.Add(new Team { Abbreviation = "PIT", Location = "Pittsburgh", Name = "Steelers" });
                entitiesToAdd.Add(new Team { Abbreviation = "SEA", Location = "Seattle", Name = "Seahawks" });
                entitiesToAdd.Add(new Team { Abbreviation = "SF", Location = "San Francisco", Name = "49ers" });
                entitiesToAdd.Add(new Team { Abbreviation = "TB", Location = "Tampa Bay", Name = "Buccaneers" });
                entitiesToAdd.Add(new Team { Abbreviation = "TEN", Location = "Tennessee", Name = "Titans" });
                entitiesToAdd.Add(new Team { Abbreviation = "WAS", Location = "Washington", Name = "Football Team" });

                context.AddRange(entitiesToAdd);

                context.SaveChanges();
            }
        }
    }
}
