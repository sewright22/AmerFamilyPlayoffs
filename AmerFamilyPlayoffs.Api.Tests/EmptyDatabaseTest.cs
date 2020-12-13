namespace AmerFamilyPlayoffs.Api.Tests
{
    using AmerFamilyPlayoffs.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EmptyDatabaseTest : SqliteInMemoryItemsControllerTest
    {
        public override void SeedPlayoffs(AmerFamilyPlayoffContext context)
        {
        }

        public override void SeedPlayoffTeams(AmerFamilyPlayoffContext context)
        {
        }

        public override void SeedSeasons(AmerFamilyPlayoffContext context)
        {
        }

        public override void SeedSeasonTeams(AmerFamilyPlayoffContext context)
        {
        }

        public override void SeedTeams(AmerFamilyPlayoffContext context)
        {
        }

        public override void SeedConferences(AmerFamilyPlayoffContext context)
        {
        }
    }
}
