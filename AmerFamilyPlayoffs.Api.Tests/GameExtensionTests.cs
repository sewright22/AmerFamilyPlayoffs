namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GameExtensionTests : EmptyDatabaseTest
    {
        [Fact]
        public void BuildGameModelTest()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var playoff = fixture.Build<Playoff>()
                                     .Without(x => x.PlayoffRounds)
                                     .Without(x => x.PlayoffTeams)
                                     .Create();

                var season = fixture.Build<Season>().With(x => x.Playoff, playoff).Create();
                var afcTeam1 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var afcTeam2 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var afcTeam3 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var afcTeam4 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var afcTeam5 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var afcTeam6 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam1 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam2 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam3 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam4 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam5 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();
                var nfcTeam6 = fixture.Build<SeasonTeam>().With(x => x.Season, season).Without(x => x.PlayoffTeam).Create();

                context.Add(afcTeam1);
                context.Add(afcTeam2);
                context.Add(afcTeam3);
                context.Add(afcTeam4);
                context.Add(afcTeam5);
                context.Add(afcTeam6);
                context.Add(nfcTeam1);
                context.Add(nfcTeam2);
                context.Add(nfcTeam3);
                context.Add(nfcTeam4);
                context.Add(nfcTeam5);
                context.Add(nfcTeam6);
                context.SaveChanges();

                context.SaveChanges();

                var wildCardRound = fixture.Build<Round>().With(x => x.Number, 1).Create();
                var divisionalRound = fixture.Build<Round>().With(x => x.Number, 2).Create();
                var championshipRound = fixture.Build<Round>().With(x => x.Number, 3).Create();
                var superBowl = fixture.Build<Round>().With(x => x.Number, 4).Create();

                context.Add(wildCardRound);
                context.Add(divisionalRound);
                context.Add(championshipRound);
                context.Add(superBowl);
                context.SaveChanges();

                var afcSeed1 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 1).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam1).Create();
                var afcSeed2 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 2).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam2).Create();
                var afcSeed3 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 3).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam3).Create();
                var afcSeed4 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 4).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam4).Create();
                var afcSeed5 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 5).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam5).Create();
                var afcSeed6 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 6).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, afcTeam6).Create();
                var nfcSeed1 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 1).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam1).Create();
                var nfcSeed2 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 2).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam2).Create();
                var nfcSeed3 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 3).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam3).Create();
                var nfcSeed4 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 4).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam4).Create();
                var nfcSeed5 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 5).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam5).Create();
                var nfcSeed6 = fixture.Build<PlayoffTeam>().With(x => x.Seed, 6).With(x => x.Playoff, playoff).With(x=>x.SeasonTeam, nfcTeam6).Create();

                context.Add(afcSeed1);
                context.Add(afcSeed2);
                context.Add(afcSeed3);
                context.Add(afcSeed4);
                context.Add(afcSeed5);
                context.Add(afcSeed6);
                context.Add(nfcSeed1);
                context.Add(nfcSeed2);
                context.Add(nfcSeed3);
                context.Add(nfcSeed4);
                context.Add(nfcSeed5);
                context.Add(nfcSeed6);
                context.SaveChanges();

                var afcMatchups = new List<Matchup>();
                var nfcMatchups = new List<Matchup>();
                afcMatchups.Add(fixture.Build<Matchup>().With(x => x.HomeTeam, afcSeed3)
                                                        .With(x => x.AwayTeam, afcSeed6)
                                                        .Without(x => x.Winner)
                                                        .Create());

                afcMatchups.Add(fixture.Build<Matchup>().With(x => x.HomeTeam, afcSeed4)
                                                        .With(x => x.AwayTeam, afcSeed5)
                                                        .Without(x => x.Winner)
                                                        .Create());

                nfcMatchups.Add(fixture.Build<Matchup>().With(x => x.HomeTeam, nfcSeed3)
                                                        .With(x => x.AwayTeam, nfcSeed6)
                                                        .Without(x => x.Winner)
                                                        .Create());

                nfcMatchups.Add(fixture.Build<Matchup>().With(x => x.HomeTeam, nfcSeed4)
                                                        .With(x => x.AwayTeam, nfcSeed5)
                                                        .Without(x => x.Winner)
                                                        .Create());

                var wildCardPlayoffRound = fixture.Build<PlayoffRound>()
                                                  .With(x => x.Playoff, playoff)
                                                  .With(x => x.Round, wildCardRound)
                                                  .With(x => x.PointValue, 2)
                                                  .With(x => x.AFCMatchups, afcMatchups)
                                                  .With(x => x.NFCMatchups, nfcMatchups)
                                                  .Create();

                context.Add(wildCardPlayoffRound);
                context.SaveChanges();

                context.Playoffs.Should().NotBeEmpty();
            }
        }
    }
}
