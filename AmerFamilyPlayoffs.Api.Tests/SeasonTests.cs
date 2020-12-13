namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Api.Controllers;
    using AmerFamilyPlayoffs.Api.Extensions;
    using AmerFamilyPlayoffs.Api.Queries;
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class SeasonTests : SqliteInMemoryItemsControllerTest
    {
        [Fact]
        public void FirstTest()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                Assert.True(context.Teams.Count()==32);
                context.Teams.Add(new Team
                {
                    Abbreviation = "",
                    Location = "",
                    Name = "",
                });
                context.SaveChanges();
                Assert.True(context.Teams.Count()==33);
            }
        }

        [Fact]
        public void GetSeasonTeamsTest()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var count = context.SeasonTeams.Count();
                Assert.True(count == (32 * 3));

                var teamsController = new TeamsController(context);

                var actual = teamsController.Get(new TeamQuery
                {
                    Season = 2019,
                }).Result;

                Assert.True(actual.Count() == 32);
            }
        }

        [Fact]
        public void GetSeasonByYearTest()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var actual = context.GetSeasonByYear(2019);

                actual.Year.Should().Be(2019);
                actual.Description.Should().Be("2019-2020");
            }
        }
    }
}
