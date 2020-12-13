namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Api.Extensions;
    using AmerFamilyPlayoffs.Data;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetPlayoffTest : SqliteInMemoryItemsControllerTest
    {
        [Fact]
        public void GetPlayoffByYear()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                var actual = context.GetPlayoffByYear(2019);

                actual.Season.Year.Should().Be(2019);
            }
        }

        [Fact]
        public void PlayoffIsCreatedAndReturned()
        {
            using (var context = new AmerFamilyPlayoffContext(this.ContextOptions))
            {
                context.RemoveRange(context.Playoffs.ToList()); //Make sure no playoffs exist.
                context.SaveChanges();

                context.Playoffs.Should().BeEmpty();

                var actual = context.GetPlayoffByYear(2019);

                actual.Season.Year.Should().Be(2019);
            }
        }
    }
}
