namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
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
                Assert.True(false);
            }
        }
    }
}
