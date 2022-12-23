namespace AmerFamilyPlayoffs.Api.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Model.Extensions;
    using AmerFamilyPlayoffs.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class BracketPredictionExtensions
    {
        public static async Task<IEnumerable<PlayoffBracketPrediction>> GetBrackets(this AmerFamilyPlayoffContext context)
        {
            var retVal = new List<PlayoffBracketPrediction>();
            var brackets = await context.BracketPredictions.Include(x => x.Playoff).ThenInclude(x => x.PlayoffRounds).Where(x => x.Playoff.Season.Year == 2018).ToListAsync();

            foreach (var bracket in brackets)
            {
                retVal.Add(BuildBracketPrediction(bracket));
            }

            return retVal;
        }

        public static async Task<PlayoffBracketPrediction> CreateBracketPrediction(this AmerFamilyPlayoffContext context, string name)
        {
            var playoff = await context.Playoffs.SingleAsync(x => x.Season.Year == 2018).ConfigureAwait(false);

            var bracket = await context.BracketPredictions.FirstOrDefaultAsync(x => x.PlayoffId == playoff.Id && x.Name.ToUpper() == name.ToUpper()).ConfigureAwait(false);

            if (bracket == null)
            {
                var predication = new BracketPrediction
                {
                    Playoff = playoff,
                    Name = name,
                };

                await context.AddAsync(predication).ConfigureAwait(false);

                await context.SaveChangesAsync();

                return new PlayoffBracketPrediction
                {
                    Id = predication.Id,
                    Name = name,
                };
            }
            else
            {
                throw new Exception();
            }
        }

        public static async Task<PlayoffBracketPrediction> GetBracketPrediction(this AmerFamilyPlayoffContext context, int id)
        {
            var bracket = await context.BracketPredictions.SingleAsync(x => x.Id == id);

            return BuildBracketPrediction(bracket);
        }

        public static RoundModel GetWildCardRoundByPlayoffId(this AmerFamilyPlayoffContext context, int playoffId)
        {
            var playoffRound = context.PlayoffRounds.FirstOrDefault(x => x.Round.Number == 1 && x.PlayoffId == playoffId);

            //var afcGames = playoffRound.Matchups

            return new RoundModel
            {
                PointValue = playoffRound.PointValue,
            };
        }

        public static void BuildBracketForYear(this AmerFamilyPlayoffContext context, int year)
        {
            if (year == 2018)
            {
                var bracket = new Bracket();
                bracket.Playoff = context.GetPlayoffByYear(2018);
            }
        }

        private static PlayoffBracketPrediction BuildBracketPrediction(BracketPrediction bracket)
        {
            var wildCardRound = bracket.Playoff.PlayoffRounds.FirstOrDefault(x => x.Round.Number == 1);
            var divisionalRound = bracket.Playoff.PlayoffRounds.FirstOrDefault(x => x.Round.Number == 2);

            var afcWildCardMatchups = new List<GameModel>();
            var nfcWildCardMatchups = new List<GameModel>();
            var afcDivisionalMatchups = new List<GameModel>();
            var nfcDivisionalMatchups = new List<GameModel>();
            var afcConferenceMatchups = new List<GameModel>();
            var nfcConferenceMatchups = new List<GameModel>();
            //wildCardRound.Matchups.Where(x => x.HomeTeam.SeasonTeam.Conference.Name == "AFC").ToList().ForEach(game => afcWildCardMatchups.Add(BuildGameModel(game)));
            //wildCardRound.Matchups.Where(x => x.HomeTeam.SeasonTeam.Conference.Name == "NFC").ToList().ForEach(game => nfcWildCardMatchups.Add(BuildGameModel(game)));
            //divisionalRound.Matchups.Where(x => x.HomeTeam.SeasonTeam.Conference.Name == "AFC").ToList().ForEach(game => afcDivisionalMatchups.Add(BuildGameModel(game)));
            //divisionalRound.Matchups.Where(x => x.HomeTeam.SeasonTeam.Conference.Name == "NFC").ToList().ForEach(game => nfcDivisionalMatchups.Add(BuildGameModel(game)));

            return new PlayoffBracketPrediction
            {
                Id = bracket.Id,
                Name = bracket.Name,
                WildCardRound = new RoundModel
                {
                    PointValue = wildCardRound.PointValue,
                    AFCGames = afcWildCardMatchups,
                    NFCGames = nfcWildCardMatchups,
                },
                DivisionalRound = new RoundModel
                {
                    PointValue = divisionalRound.PointValue,
                    AFCGames = afcDivisionalMatchups,
                    NFCGames = nfcDivisionalMatchups,
                },
            };
        }

        private static GameModel BuildGameModel(Matchup matchup)
        {
            var homeTeam = new TeamModel();
            var awayTeam = new TeamModel();

            if (matchup.HomeTeam != null)
            {
                homeTeam = matchup.HomeTeam.BuildTeamModel();
            }

            if (matchup.AwayTeam != null)
            {
                awayTeam = matchup.AwayTeam.BuildTeamModel();
            }

            return new GameModel
            {
                Id = matchup.Id,
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
            };
        }
    }
}
