namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Api.Controllers;
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class TeamModelTests : SqliteInMemoryItemsControllerTest
    {
        [Fact]
        public void AllJoinsAreSuccessful()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var controller = new TeamsController(context);
                var expected = new List<TeamModel>();

                expected.Add(new TeamModel
                {
                    Id = 1,
                    Abbreviation = "ARI",
                    IsInPlayoffs = false,
                    Location = "Arizona",
                    Name = "Cardinals",
                    Year = 2019,
                    Seed = null,
                });

                expected.Add(new TeamModel
                {
                    Id = 2,
                    Abbreviation = "ATL",
                    IsInPlayoffs = true,
                    Location = "Atlanta",
                    Name = "Falcons",
                    Year = 2019,
                    Seed = 3,
                });

                var actual = controller.Get(new Queries.TeamQuery
                {
                    Season = 2019
                }).Result;

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public override void SeedTeams(AmerFamilyPlayoffContext context)
        {
            context.Add(new Team { Abbreviation = "ATL", Location = "Atlanta", Name = "Falcons" });
            context.Add(new Team { Abbreviation = "ARI", Location = "Arizona", Name = "Cardinals" });
            context.SaveChanges();
        }
    }
}
