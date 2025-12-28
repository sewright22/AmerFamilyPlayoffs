// <copyright file="TeamExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using System.Runtime.CompilerServices;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Models.Bracket;

    public static class TeamExtensions
    {
        private static readonly List<Team> Teams = new List<Team>
            {
                new Team { Code = "ARI", Name = "Cardinals", City = "Arizona", Conference = Conference.NFC, Division = "West", LogoUrl = "https://example.com/logo/ari.png" },
                new Team { Code = "ATL", Name = "Falcons", City = "Atlanta", Conference = Conference.NFC, Division = "South", LogoUrl = "https://example.com/logo/atl.png" },
                new Team { Code = "BAL", Name = "Ravens", City = "Baltimore", Conference = Conference.AFC, Division = "North", LogoUrl = "https://example.com/logo/bal.png" },
                new Team { Code = "BUF", Name = "Bills", City = "Buffalo", Conference = Conference.AFC, Division = "East", LogoUrl = "https://example.com/logo/buf.png" },
                new Team { Code = "CAR", Name = "Panthers", City = "Carolina", Conference = Conference.NFC, Division = "South", LogoUrl = "https://example.com/logo/car.png" },
                new Team { Code = "CHI", Name = "Bears", City = "Chicago", Conference = Conference.NFC, Division = "North", LogoUrl = "https://example.com/logo/chi.png" },
                new Team { Code = "CIN", Name = "Bengals", City = "Cincinnati", Conference = Conference.AFC, Division = "North", LogoUrl = "https://example.com/logo/cin.png" },
                new Team { Code = "CLE", Name = "Browns", City = "Cleveland", Conference = Conference.AFC, Division = "North", LogoUrl = "https://example.com/logo/cle.png" },
                new Team { Code = "DAL", Name = "Cowboys", City = "Dallas", Conference = Conference.NFC, Division = "East", LogoUrl = "https://example.com/logo/dal.png" },
                new Team { Code = "DEN", Name = "Broncos", City = "Denver", Conference = Conference.AFC, Division = "West", LogoUrl = "https://example.com/logo/den.png" },
                new Team { Code = "DET", Name = "Lions", City = "Detroit", Conference = Conference.NFC, Division = "North", LogoUrl = "https://example.com/logo/det.png" },
                new Team { Code = "GB", Name = "Packers", City = "Green Bay", Conference = Conference.NFC, Division = "North", LogoUrl = "https://example.com/logo/gb.png" },
                new Team { Code = "HOU", Name = "Texans", City = "Houston", Conference = Conference.AFC, Division = "South", LogoUrl = "https://example.com/logo/hou.png" },
                new Team { Code = "IND", Name = "Colts", City = "Indianapolis", Conference = Conference.AFC, Division = "South", LogoUrl = "https://example.com/logo/ind.png" },
                new Team { Code = "JAX", Name = "Jaguars", City = "Jacksonville", Conference = Conference.AFC, Division = "South", LogoUrl = "https://example.com/logo/jax.png" },
                new Team { Code = "KC", Name = "Chiefs", City = "Kansas City", Conference = Conference.AFC, Division = "West", LogoUrl = "https://example.com/logo/kc.png" },
                new Team { Code = "LV", Name = "Raiders", City = "Las Vegas", Conference = Conference.AFC, Division = "West", LogoUrl = "https://example.com/logo/lv.png" },
                new Team { Code = "LAC", Name = "Chargers", City = "Los Angeles", Conference = Conference.AFC, Division = "West", LogoUrl = "https://example.com/logo/lac.png" },
                new Team { Code = "LAR", Name = "Rams", City = "Los Angeles", Conference = Conference.NFC, Division = "West", LogoUrl = "https://example.com/logo/lar.png" },
                new Team { Code = "MIA", Name = "Dolphins", City = "Miami", Conference = Conference.AFC, Division = "East", LogoUrl = "https://example.com/logo/mia.png" },
                new Team { Code = "MIN", Name = "Vikings", City = "Minnesota", Conference = Conference.NFC, Division = "North", LogoUrl = "https://example.com/logo/min.png" },
                new Team { Code = "NE", Name = "Patriots", City = "New England", Conference = Conference.AFC, Division = "East", LogoUrl = "https://example.com/logo/ne.png" },
                new Team { Code = "NO", Name = "Saints", City = "New Orleans", Conference = Conference.NFC, Division = "South", LogoUrl = "https://example.com/logo/no.png" },
                new Team { Code = "NYG", Name = "Giants", City = "New York", Conference = Conference.NFC, Division = "East", LogoUrl = "https://example.com/logo/nyg.png" },
                new Team { Code = "NYJ", Name = "Jets", City = "New York", Conference = Conference.AFC, Division = "East", LogoUrl = "https://example.com/logo/nyj.png" },
                new Team { Code = "PHI", Name = "Eagles", City = "Philadelphia", Conference = Conference.NFC, Division = "East", LogoUrl = "https://example.com/logo/phi.png" },
                new Team { Code = "PIT", Name = "Steelers", City = "Pittsburgh", Conference = Conference.AFC, Division = "North", LogoUrl = "https://example.com/logo/pit.png" },
                new Team { Code = "SF", Name = "49ers", City = "San Francisco", Conference = Conference.NFC, Division = "West", LogoUrl = "https://example.com/logo/sf.png" },
                new Team { Code = "SEA", Name = "Seahawks", City = "Seattle", Conference = Conference.NFC, Division = "West", LogoUrl = "https://example.com/logo/sea.png" },
                new Team { Code = "TB", Name = "Buccaneers", City = "Tampa Bay", Conference = Conference.NFC, Division = "South", LogoUrl = "https://example.com/logo/tb.png" },
                new Team { Code = "TEN", Name = "Titans", City = "Tennessee", Conference = Conference.AFC, Division = "South", LogoUrl = "https://example.com/logo/ten.png" },
                new Team { Code = "WAS", Name = "Commanders", City = "Washington", Conference = Conference.NFC, Division = "East", LogoUrl = "https://example.com/logo/was.png" }
            };

        public static List<Team> GetTeams(this PlayoffPoolContext dbContext)
        {
            return Teams;
        }

        public static List<SelectListItem> GetTeamsAsSelectListItem(this PlayoffPoolContext dbContext)
        {
            return Teams.Select(team => new SelectListItem
            {
                Text = $"{team.City} {team.Name}",
                Value = team.Code
            }).ToList();
        }

        public static TeamModel AsTeamModel(this PlayoffTeam team, string seasonId)
        {
            return new TeamModel
            {
                Id = team.Id,
                SeasonId = seasonId,
                Code = team.Code,
                Seed = team.Seed,
            };
        }

        public static Team GetTeamByCode(this PlayoffPoolContext dbContext, string code)
        {
            return Teams.FirstOrDefault(team => team.Code == code);
        }

        public static PlayoffTeamModel ToPlayoffTeamModel(this PlayoffTeam team)
        {
            return new PlayoffTeamModel
            {
                Id = team.Id,
                Name = team.Name,
                Seed = team.Seed
            };
        }
    }
}
