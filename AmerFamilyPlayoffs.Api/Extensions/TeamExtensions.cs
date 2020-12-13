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
        public static Playoff GetPlayoff(this AmerFamilyPlayoffContext context, int year)
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

        public static SeasonTeam GetSeasonTeam(this AmerFamilyPlayoffContext context, int teamId, int year)
        {
            return context.SeasonTeams.Include(p => p.Season).SingleOrDefault(p => p.Season.Year == year && p.TeamId == teamId);
        }

        public static IQueryable<TeamModel> GetTeamsByYear(this AmerFamilyPlayoffContext context, int year)
        {
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team)
                                      .Include(st=>st.PlayoffTeam)
                                      .Include(st=>st.ConferenceTeam)
                                      .Where(st => st.Season.Year == year)
                                      .Select(st => new TeamModel
                                      {
                                          Id = st.Team.Id,
                                          Abbreviation = st.Team.Abbreviation,
                                          Location = st.Team.Location,
                                          Name = st.Team.Name,
                                          Year = st.Season.Year,
                                          IsInPlayoffs = st.PlayoffTeam != null && st.PlayoffTeam.Playoff.Season.Year == year,
                                          Seed = st.PlayoffTeam == null ? null as int? : st.PlayoffTeam.Seed,
                                          Conference = st.ConferenceTeam.Conference.Name
                                      });
        }

        public static IQueryable<TeamModel> GetTeamsByYearAndConference(this AmerFamilyPlayoffContext context, int year, int conferenceId)
        {
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team)
                                      .Include(t => t.PlayoffTeam)
                                      .Include(st=>st.ConferenceTeam).ThenInclude(ct=>ct.Conference)
                                      .Where(st => st.Season.Year == year && st.ConferenceTeam.Id == conferenceId)
                                      .Select(st => new TeamModel
                                      {
                                          Id = st.Team.Id,
                                          Abbreviation = st.Team.Abbreviation,
                                          Location = st.Team.Location,
                                          Name = st.Team.Name,
                                          Year = st.Season.Year,
                                          IsInPlayoffs = st.PlayoffTeam != null && st.PlayoffTeam.Playoff.Season.Year == year,
                                          Seed = st.PlayoffTeam == null ? null as int? : st.PlayoffTeam.Seed,
                                          Conference = st.ConferenceTeam.Conference.Name,
                                      });
        }
    }
}
