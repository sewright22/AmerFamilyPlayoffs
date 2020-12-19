namespace AmerFamilyPlayoffs.Api.Extensions
{
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class BracketExtensions
    {
        public static PlayoffBracket GetBracketByYear(this AmerFamilyPlayoffContext context, int year)
        {
            var bracket = context.Brackets.SingleOrDefault(b => b.Playoff.Season.Year == year);

            if (bracket == null)
            {
                BuildBracketForYear(context, year);
                return GetBracketByYear(context, year);
            }
            else
            {
                return new PlayoffBracket
                {
                    Id = bracket.Id,
                };
            }
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
            if (year==2018)
            {
                var bracket = new Bracket();
                bracket.Playoff = context.GetPlayoffByYear(2018);

            }
        }
    }
}
