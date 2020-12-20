namespace AmerFamilyPlayoffs.Model.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using System;

    public static class TeamExtensions
    {
        public static TeamModel BuildTeamModel(this PlayoffTeam playoffTeam)
        {
            return new TeamModel
            {
                Id = playoffTeam.Id,
                Abbreviation = playoffTeam.SeasonTeam.Team.Abbreviation,
                Location = playoffTeam.SeasonTeam.Team.Location,
                Name = playoffTeam.SeasonTeam.Team.Name,
                Year = playoffTeam.Playoff.Season.Year,
                IsInPlayoffs = true,
                Seed = playoffTeam.Seed,
                Conference = playoffTeam.SeasonTeam.Conference.Name
            };
        }
    }
}
