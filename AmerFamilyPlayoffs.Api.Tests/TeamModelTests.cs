namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Api.Controllers;
    using AmerFamilyPlayoffs.Api.Extensions;
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

    public class TeamModelTests : SqliteInMemoryItemsControllerTest
    {
        [Fact]
        public void GetSeasonTeams()
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
                    Conference = "NFC"
                });

                expected.Add(new TeamModel
                {
                    Id = 2,
                    Abbreviation = "BAL",
                    IsInPlayoffs = true,
                    Location = "Baltimore",
                    Name = "Ravens",
                    Year = 2019,
                    Seed = 3,
                    Conference = "AFC"
                });

                var actual = context.GetTeamsByYear(2019);

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
                        Conference = "NFC",
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
                        Conference = "AFC"
                    }
                };

                teamsController.Post(teams);

                context.Playoffs.Should().ContainSingle();
                context.PlayoffTeams.Should().HaveCount(2);
            }
        }

        [Fact]
        public void GetSeasonTeamsInConference()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var controller = new TeamsController(context);
                var expected = new List<TeamModel>();

                expected.Add(new TeamModel
                {
                    Id = 2,
                    Abbreviation = "BAL",
                    IsInPlayoffs = true,
                    Location = "Baltimore",
                    Name = "Ravens",
                    Year = 2019,
                    Seed = 3,
                    Conference = "AFC"
                });

                var actual = context.GetTeamsByYearAndConference(2019, 1);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void GetSeasonTeamsInConferenceReturnsMoreThanOneRecord()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var controller = new TeamsController(context);
                var expected = new List<TeamModel>();

                var titans = new Team { Abbreviation = "TEN", Location = "Tennessee", Name = "Titans" };
                context.Add(titans);
                context.Add(new SeasonTeam{ Season = context.GetSeasonByYear(2019), ConferenceId = 1, Team = titans, });
                context.SaveChanges();
                expected.Add(new TeamModel
                {
                    Id = 2,
                    Abbreviation = "BAL",
                    IsInPlayoffs = true,
                    Location = "Baltimore",
                    Name = "Ravens",
                    Year = 2019,
                    Seed = 3,
                    Conference = "AFC"
                });

                expected.Add(new TeamModel
                {
                    Id = 3,
                    Abbreviation = "TEN",
                    IsInPlayoffs = false,
                    Location = "Tennessee",
                    Name = "Titans",
                    Year = 2019,
                    Seed = null,
                    Conference = "AFC"
                });

                var actual = context.GetTeamsByYearAndConference(2019, 1);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public override void SeedTeams(AmerFamilyPlayoffContext context)
        {
            context.Add(new Team { Abbreviation = "BAL", Location = "Baltimore", Name = "Ravens" });
            context.Add(new Team { Abbreviation = "ARI", Location = "Arizona", Name = "Cardinals" });
            context.SaveChanges();
        }

        public override void SeedPlayoffTeams(AmerFamilyPlayoffContext context)
        {
            context.Add(new PlayoffTeam
            {
                PlayoffId = 1,
                SeasonTeam = context.SeasonTeams.Include(st => st.Team).Include(x => x.Season).FirstOrDefault(st => st.Team.Abbreviation == "BAL" && st.Season.Year == 2019),
                Seed = 3,
            });

            context.SaveChanges();
        }
    }
}
