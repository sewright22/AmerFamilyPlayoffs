namespace AmerFamilyPlayoffs.Api.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class SeasonExtensions
    {
        public static Season GetSeasonByYear(this AmerFamilyPlayoffContext context, int year)
        {
            return context.Seasons.FirstOrDefault(s => s.Year == year);
        }
    }
}
