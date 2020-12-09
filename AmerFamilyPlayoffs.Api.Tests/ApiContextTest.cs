namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
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

                var twentyEighteen = new Season
                {
                    Year = 2018,
                    Description = "2018-2019"
                };

                var twentyNineteen = new Season
                {
                    Year = 2019,
                    Description = "2019-2020"
                };

                var twentyTwenty = new Season
                {
                    Year = 2020,
                    Description = "2020-2021"
                };

                context.AddRange(twentyEighteen, twentyNineteen, twentyTwenty);

                context.SaveChanges();
            }
        }
    }
}
