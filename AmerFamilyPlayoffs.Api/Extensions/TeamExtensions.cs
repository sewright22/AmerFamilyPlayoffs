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

        public static SeasonTeam GetSeasonTeam(this AmerFamilyPlayoffContext context, int teamId, int year)
        {
            return context.SeasonTeams.Include(p => p.Season).SingleOrDefault(p => p.Season.Year == year && p.TeamId == teamId);
        }

        public static IQueryable<TeamModel> GetTeamsByYear(this AmerFamilyPlayoffContext context, int year)
        {
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team)
                                      .Include(st=>st.PlayoffTeam)
                                      .Include(st=>st.Conference)
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
                                          Conference = st.Conference.Name
                                      });
        }

        public static IQueryable<TeamModel> GetTeamsByYearAndConference(this AmerFamilyPlayoffContext context, int year, int conferenceId)
        {
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team)
                                      .Include(t => t.PlayoffTeam)
                                      .Include(st=>st.Conference)
                                      .Where(st => st.Season.Year == year && st.Conference.Id == conferenceId)
                                      .Select(st => new TeamModel
                                      {
                                          Id = st.Team.Id,
                                          Abbreviation = st.Team.Abbreviation,
                                          Location = st.Team.Location,
                                          Name = st.Team.Name,
                                          Year = st.Season.Year,
                                          IsInPlayoffs = st.PlayoffTeam != null && st.PlayoffTeam.Playoff.Season.Year == year,
                                          Seed = st.PlayoffTeam == null ? null as int? : st.PlayoffTeam.Seed,
                                          Conference = st.Conference.Name,
                                      });
        }
    }
}
