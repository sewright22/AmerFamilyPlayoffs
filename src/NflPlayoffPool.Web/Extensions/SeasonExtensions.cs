// <copyright file="SeasonExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Models.Bracket;

    public static class SeasonExtensions
    {
        public static List<SeasonSummaryModel> AsSeasonSummaryList(this IQueryable<Season> seasons)
        {
            List<SeasonSummaryModel> list = new List<SeasonSummaryModel>();
            foreach (var season in seasons)
            {
                list.Add(new SeasonSummaryModel
                {
                    Id = season.Id.ToString(),
                    Name = season.Year.ToString(),
                });
            }
            return list;
        }

        public static SeasonModel AsSeasonModel(this Season season, int bracketCount, int timezoneOffset)
        {
            SeasonModel seasonModel = new SeasonModel
            {
                Id = season.Id.ToString(),
                Year = season.Year.ToString(),
                CutoffDateTime = season.SubmissionDeadline == DateTime.MinValue ? DateTime.Now : season.SubmissionDeadline.AddMinutes(360 - timezoneOffset),
                Status = season.Status,
                IsCurrent = season.IsCurrent,
                CurrentRound = season.CurrentRound,
                WildcardPoints = season.WildcardPoints,
                DivisionalPoints = season.DivisionalPoints,
                ConferencePoints = season.ConferencePoints,
                SuperBowlPoints = season.SuperBowlPoints,
                BracketCount = bracketCount,
            };

            foreach (var team in season.Teams.OrderBy(x => x.Conference).ThenBy(x => x.Seed))
            {
                seasonModel.Teams.Add(team.AsTeamModel(season.Id));
            }

            return seasonModel;
        }

        public static async Task<string> Create(this PlayoffPoolContext playoffPoolContext, SeasonModel seasonModel)
        {
            Season season = new Season();
            season.Update(seasonModel);
            await playoffPoolContext.Seasons.AddAsync(season);
            await playoffPoolContext.SaveChangesAsync();
            return season.Id;
        }

        public static Season? GetCurrentSeason(this PlayoffPoolContext playoffPoolContext, bool canEdit = false)
        {
            IQueryable<Season> seasons = playoffPoolContext.Seasons.Include(s => s.Teams);

            if (!canEdit)
            {
                seasons = seasons.AsNoTracking();
            }

            return seasons.FirstOrDefault(x => x.IsCurrent);
        }

        public static void Update(this Season? seasonToUpdate, SeasonModel seasonModel)
        {
            if (seasonToUpdate == null)
            {
                return;
            }

            seasonToUpdate.Year = int.Parse(seasonModel.Year);
            seasonToUpdate.SubmissionDeadline = seasonModel.CutoffDateTime ?? DateTime.Now;
            seasonToUpdate.Status = seasonModel.Status;
            seasonToUpdate.WildcardPoints = seasonModel.WildcardPoints;
            seasonToUpdate.DivisionalPoints = seasonModel.DivisionalPoints;
            seasonToUpdate.ConferencePoints = seasonModel.ConferencePoints;
            seasonToUpdate.SuperBowlPoints = seasonModel.SuperBowlPoints;
        }

        public static PlayoffTeam? GetTeamByConferenceAndSeed(this Season season, Conference conference, int seed)
        {
            return season.Teams.FirstOrDefault(t => t.Conference == conference && t.Seed == seed);
        }

        public static Models.Bracket.RoundModel GenerateAfcWildcardRound(this Season season)
        {
            var round = new Models.Bracket.RoundModel
            {
                Conference = "AFC",
                Name = "Wildcard",
                PointValue = 1,
                RoundNumber = 1,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, 2, 7, Conference.AFC, 1),
                    CreateMatchup(season, 3, 6, Conference.AFC, 2),
                    CreateMatchup(season, 4, 5, Conference.AFC, 3),
                }
            };

            return round;
        }

        public static Models.Bracket.RoundModel GenerateNfcWildcardRound(this Season season)
        {
            var round = new Models.Bracket.RoundModel
            {
                Conference = "NFC",
                Name = "Wildcard",
                PointValue = 1,
                RoundNumber = 1,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, 2, 7, Conference.NFC, 4),
                    CreateMatchup(season, 3, 6, Conference.NFC, 5),
                    CreateMatchup(season, 4, 5, Conference.NFC, 6),
                }
            };
            return round;
        }

        public static Models.Bracket.RoundModel GenerateAfcDivisionalRound(this Season season, Models.Bracket.RoundModel afcWildcardRound)
        {
            // Get the winners of the wildcard round
            var winners = afcWildcardRound.Games.Select(x=> x.HomeTeam.Id == x.SelectedWinner ? x.HomeTeam.Seed : x.AwayTeam.Seed).ToList();

            // Get the lowest seed team
            var lowestSeed = winners.Max();

            // Remove the lowest seed team
            winners.RemoveAll(t => t == lowestSeed);

            // Get the next lowest seed team
            var nextLowestSeed = winners.Max();

            // Remove the next lowest seed team
            winners.RemoveAll(t => t == nextLowestSeed);

            // Get the next lowest seed team
            var thirdLowestSeed = winners.Max();

            // Remove the next lowest seed team
            winners.RemoveAll(t => t == thirdLowestSeed);

            var round = new Models.Bracket.RoundModel
            {
                Conference = "AFC",
                Name = "Divisional",
                PointValue = 2,
                RoundNumber = 2,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, 1, lowestSeed, Conference.AFC, 7),
                    CreateMatchup(season, thirdLowestSeed, nextLowestSeed, Conference.AFC, 8),
                }
            };
            return round;
        }

        public static Models.Bracket.RoundModel GenerateNfcDivisionalRound(this Season season, Models.Bracket.RoundModel nfcWildcardRound)
        {
            // Get the winners of the wildcard round
            var winners = nfcWildcardRound.Games.Select(x => x.HomeTeam.Id == x.SelectedWinner ? x.HomeTeam.Seed : x.AwayTeam.Seed).ToList();
            // Get the lowest seed team
            var lowestSeed = winners.Max();
            // Remove the lowest seed team
            winners.RemoveAll(t => t == lowestSeed);
            // Get the next lowest seed team
            var nextLowestSeed = winners.Max();
            // Remove the next lowest seed team
            winners.RemoveAll(t => t == nextLowestSeed);
            // Get the next lowest seed team
            var thirdLowestSeed = winners.Max();
            // Remove the next lowest seed team
            winners.RemoveAll(t => t == thirdLowestSeed);
            var round = new Models.Bracket.RoundModel
            {
                Conference = "NFC",
                Name = "Divisional",
                PointValue = 2,
                RoundNumber = 2,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, 1, lowestSeed, Conference.NFC, 9),
                    CreateMatchup(season, thirdLowestSeed, nextLowestSeed, Conference.NFC, 10),
                }
            };
            return round;
        }

        public static Models.Bracket.RoundModel GenerateAfcConferenceRound(this Season season, Models.Bracket.RoundModel afcDivisionalRound)
        {
            // Get the winners of the divisional round
            var winners = afcDivisionalRound.Games.Select(x => x.HomeTeam.Id == x.SelectedWinner ? x.HomeTeam.Seed : x.AwayTeam.Seed).ToList();
            // Get the lowest seed team
            var lowestSeed = winners.Max();
            // Remove the lowest seed team
            winners.RemoveAll(t => t == lowestSeed);
            // Get the next lowest seed team
            var nextLowestSeed = winners.Max();

            var round = new Models.Bracket.RoundModel
            {
                Conference = "AFC",
                Name = "Conference",
                PointValue = 3,
                RoundNumber = 3,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, nextLowestSeed, lowestSeed, Conference.AFC, 11),
                }
            };
            return round;
        }

        public static Models.Bracket.RoundModel GenerateNfcConferenceRound(this Season season, Models.Bracket.RoundModel nfcDivisionalRound)
        {
            // Get the winners of the divisional round
            var winners = nfcDivisionalRound.Games.Select(x => x.HomeTeam.Id == x.SelectedWinner ? x.HomeTeam.Seed : x.AwayTeam.Seed).ToList();
            // Get the lowest seed team
            var lowestSeed = winners.Max();
            // Remove the lowest seed team
            winners.RemoveAll(t => t == lowestSeed);
            // Get the next lowest seed team
            var nextLowestSeed = winners.Max();
            var round = new Models.Bracket.RoundModel
            {
                Conference = "NFC",
                Name = "Conference",
                PointValue = 3,
                RoundNumber = 3,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateMatchup(season, nextLowestSeed, lowestSeed, Conference.NFC, 12),
                }
            };
            return round;
        }

        public static Models.Bracket.RoundModel GenerateSuperBowlRound(this Season season, Models.Bracket.RoundModel afcConferenceRound, Models.Bracket.RoundModel nfcConferenceRound)
        {
            // Get the winners of the conference round
            var afcWinner = afcConferenceRound.Games[0].SelectedWinner;
            var nfcWinner = nfcConferenceRound.Games[0].SelectedWinner;

            // Get seed of the winners
            int afcSeed = afcConferenceRound.Games[0].HomeTeam.Id == afcWinner ? afcConferenceRound.Games[0].HomeTeam.Seed : afcConferenceRound.Games[0].AwayTeam.Seed;
            int nfcSeed = nfcConferenceRound.Games[0].HomeTeam.Id == nfcWinner ? nfcConferenceRound.Games[0].HomeTeam.Seed : nfcConferenceRound.Games[0].AwayTeam.Seed;

            var round = new Models.Bracket.RoundModel
            {
                Conference = "Super Bowl",
                Name = "Super Bowl",
                PointValue = season.SuperBowlPoints,
                RoundNumber = 4,
                IsLocked = false,
                Games = new List<MatchupModel>
                {
                    CreateSuperBowlMatchup(season, afcSeed, nfcSeed),
                }
            };
            return round;
        }

        public static bool IsStarted(this Season season)
        {
            return season.Status == SeasonStatus.InProgress;
        }

        private static MatchupModel CreateMatchup(Season season, int homeSeed, int awaySeed, Conference conference, int gameNumber)
        {
            var homeTeam = season.GetTeamByConferenceAndSeed(conference, homeSeed);
            var awayTeam = season.GetTeamByConferenceAndSeed(conference, awaySeed);

            return new MatchupModel
            {
                GameNumber = gameNumber, // Assuming GameNumber is based on the home seed
                Name = $"Game {gameNumber}",
                HomeTeam = homeTeam?.ToPlayoffTeamModel(),
                AwayTeam = awayTeam?.ToPlayoffTeamModel()
            };
        }

        private static MatchupModel CreateSuperBowlMatchup(Season season, int afcSeed, int nfcSeed)
        {
            var afcTeam = season.GetTeamByConferenceAndSeed(Conference.AFC, afcSeed);
            var nfcTeam = season.GetTeamByConferenceAndSeed(Conference.NFC, nfcSeed);
            return new MatchupModel
            {
                GameNumber = 13,
                Name = "Super Bowl",
                HomeTeam = afcTeam?.ToPlayoffTeamModel(),
                AwayTeam = nfcTeam?.ToPlayoffTeamModel()
            };
        }
    }
}
