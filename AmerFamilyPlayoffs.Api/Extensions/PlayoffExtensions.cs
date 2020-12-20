namespace AmerFamilyPlayoffs.Api.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class PlayoffExtensions
    {
        public static void SavePlayoffTeam(this AmerFamilyPlayoffContext context, SeasonTeam seasonTeam, Playoff playoff, int seed)
        {
            if (seasonTeam.PlayoffTeam == null)
            {
                seasonTeam.PlayoffTeam = new PlayoffTeam
                {
                    Playoff = playoff,
                    SeasonTeam = seasonTeam,
                };
            }

            seasonTeam.PlayoffTeam.Seed = seed;

            context.SaveChanges();
        }

        public static Playoff GetPlayoffByYear(this AmerFamilyPlayoffContext context, int year)
        {
            var playoff = context.Playoffs.Include(p => p.Season).SingleOrDefault(p => p.Season.Year == year);

            if (playoff == null)
            {
                var season = context.Seasons.FirstOrDefault(s => s.Year == year);

                playoff = new Playoff
                {
                    Season = season,
                };

                context.Add(playoff);
                context.SaveChanges();
            }

            return playoff;
        }
    }
}
