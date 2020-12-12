namespace AmerFamilyPlayoffs.Api.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class TeamExtensions
    {
        public static void SavePlayoffTeam(this AmerFamilyPlayoffContext context, Team team, Playoff playoff, int seed)
        {
            if (team.PlayoffTeam == null)
            {
                team.PlayoffTeam = new PlayoffTeam
                {
                    Playoff = playoff,
                    Team = team,
                };
            }

            team.PlayoffTeam.Seed = seed;

            context.SaveChanges();
        }
        public static Playoff GetPlayoff(this AmerFamilyPlayoffContext context, int year)
        {
            var playoff = context.Playoffs.Include(p => p.Season).SingleOrDefault(p => p.Season.Year == year);

            if (playoff == null)
            {
                playoff = new Playoff
                {
                    Season = context.Seasons.Single(s => s.Year == year),
                };
            }

            return playoff;
        }
    }
}
