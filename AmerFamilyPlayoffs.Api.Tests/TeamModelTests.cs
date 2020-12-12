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

        [Fact]
        public void SavePlayoffTeamWhenPlayoffDoesNotExists()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                context.RemoveRange(context.Playoffs);
                context.RemoveRange(context.PlayoffTeams);
                context.RemoveRange(context.Rounds);
                context.SaveChanges();
                context.Playoffs.Should().HaveCount(0);

                var teamsController = new TeamsController(context);

                teamsController.Post(new List<TeamModel>() {new TeamModel
                {
                    Id = 2,
                    Abbreviation = "ATL",
                    IsInPlayoffs = true,
                    Location = "Atlanta",
                    Name = "Falcons",
                    Year = 2019,
                    Seed = 3,
                }});

                context.Playoffs.Should().ContainSingle();
                context.PlayoffTeams.Should().ContainSingle();
            }
        }

        [Fact]
        public void SaveMultipleTeamsWhenPlayoffDoesNotExists()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                context.RemoveRange(context.Playoffs);
                context.RemoveRange(context.PlayoffTeams);
                context.RemoveRange(context.Rounds);
                context.SaveChanges();
                context.Playoffs.Should().HaveCount(0);

                var teamsController = new TeamsController(context);

                var teams = new List<TeamModel>()
                {
                    new TeamModel
                    {
                        Id = 2,
                        Abbreviation = "ATL",
                        IsInPlayoffs = true,
                        Location = "Atlanta",
                        Name = "Falcons",
                        Year = 2019,
                        Seed = 3,
                    },
                    new TeamModel
                    {
                        Id = 1,
                        Abbreviation = "ARI",
                        IsInPlayoffs = true,
                        Location = "Arizona",
                        Name = "Cardinals",
                        Year = 2019,
                        Seed = 4,
                    }
                };

                teamsController.Post(teams);

                context.Playoffs.Should().ContainSingle();
                context.PlayoffTeams.Should().HaveCount(2);
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
