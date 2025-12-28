// <copyright file="BracketExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models.Bracket;
    using NflPlayoffPool.Web.Models.Home;
    using NflPlayoffPool.Web.ViewModels;

    public static class BracketExtensions
    {
        public static List<BracketSummaryModel> AsBracketSummaryModels(this List<Bracket> brackets)
        {
            return brackets.Select(b => new BracketSummaryModel
            {
                Id = b.Id,
                Name = b.Name,
                CurrentScore = b.CurrentScore,
                MaxPossibleScore = b.MaxPossibleScore,
                PredictedWinner = b.PredictedWinner,
            }).ToList();
        }

        public static BracketViewModel AsBracketViewModel(this MasterBracket bracket)
        {
            return new BracketViewModel
            {
                UserId = bracket.UserId,
                Bracket = new BracketModel
                {
                    Id = bracket.Id,
                    Name = bracket.Name,
                    UserId = bracket.UserId,
                },
            };
        }

        public static BracketViewModel AsBracketViewModel(this Bracket bracket)
        {
            return new BracketViewModel
            {
                UserId = bracket.UserId,
                Bracket = new BracketModel
                {
                    Id = bracket.Id,
                    Name = bracket.Name,
                    UserId = bracket.UserId,
                },
            };
        }

        public static NflPlayoffPool.Web.Admin.ViewModels.BracketViewModel AsAdminBracketViewModel(this Bracket bracket)
        {
            return new NflPlayoffPool.Web.Admin.ViewModels.BracketViewModel
            {
                UserId = bracket.UserId,
                Bracket = new BracketModel
                {
                    Id = bracket.Id,
                    Name = bracket.Name,
                    UserId = bracket.UserId,
                    SeasonYear = bracket.SeasonYear,
                },
            };
        }

        public static List<BracketPick> ExtractPicks(this BracketModel bracket)
        {
            List<BracketPick> picks = new List<BracketPick>();
            foreach (var round in bracket.AfcRounds)
            {
                foreach (var matchup in round.Games)
                {
                    picks.Add(new BracketPick
                    {
                        Conference = "AFC",
                        RoundNumber = round.RoundNumber,
                        PointValue = round.PointValue,
                        GameNumber = matchup.GameNumber,
                        PredictedWinningId = matchup.SelectedWinner,
                        PredictedWinningTeam = matchup.SelectedWinner == null ? null : matchup.HomeTeam.Id == matchup.SelectedWinner ? matchup.HomeTeam.Name : matchup.AwayTeam.Name,
                    });
                }
            }

            foreach (var round in bracket.NfcRounds)
            {
                foreach (var matchup in round.Games)
                {
                    picks.Add(new BracketPick
                    {
                        Conference = "NFC",
                        RoundNumber = round.RoundNumber,
                        PointValue = round.PointValue,
                        GameNumber = matchup.GameNumber,
                        PredictedWinningId = matchup.SelectedWinner,
                        PredictedWinningTeam = matchup.SelectedWinner == null ? null : matchup.HomeTeam.Id == matchup.SelectedWinner ? matchup.HomeTeam.Name : matchup.AwayTeam.Name,
                    });
                }
            }

            if (bracket.SuperBowl?.SelectedWinner is not null)
            {
                picks.Add(new BracketPick
                {
                    Conference = "Super Bowl",
                    RoundNumber = 4,
                    PointValue = 5,
                    GameNumber = 13,
                    PredictedWinningId = bracket.SuperBowl.SelectedWinner,
                    PredictedWinningTeam = bracket.SuperBowl.SelectedWinner == null ? null : bracket.SuperBowl.HomeTeam.Id == bracket.SuperBowl.SelectedWinner ? bracket.SuperBowl.HomeTeam.Name : bracket.SuperBowl.AwayTeam.Name,
                });
            }

            return picks;
        }

        public static void SetSelectedWinners(this RoundModel round, IEnumerable<BracketPick> picks)
        {
            round.SetSelectedWinners(picks, new List<string?>());
        }

        public static void SetSelectedWinners(this RoundModel round, IEnumerable<BracketPick> picks, List<string?> masterPicks)
        {
            foreach (var game in round.Games)
            {
                var pick = picks.FirstOrDefault(x => x.Conference == round.Conference && x.RoundNumber == round.RoundNumber && x.GameNumber == game.GameNumber);

                if (pick != null)
                {
                    game.SelectedWinner = pick.PredictedWinningId;
                    game.HomeTeam.Selected = game.HomeTeam.Id == pick.PredictedWinningId;
                    game.AwayTeam.Selected = game.AwayTeam.Id == pick.PredictedWinningId;

                    var nonNullPicks = masterPicks.Where(x => x != null).ToList();
                    if (pick.PredictedWinningId is not null && nonNullPicks.Any())
                    {
                        game.IsCorrect = nonNullPicks.Contains(pick.PredictedWinningId);
                    }
                }
            }
        }

        public static void CalculateScores(this Bracket bracket, Season season)
        {
            bracket.CalculateScores(season.Bracket, season);
        }

        public static void CalculateScores(this Bracket bracket, MasterBracket masterBracket, Season season)
        {
            bracket.CurrentScore = 0;
            bracket.MaxPossibleScore = 42;

            List<string> eliminatedTeams = new List<string>();

            if (masterBracket.Picks.Any(x => x.RoundNumber == 1))
            {
                // Get wildcard picks
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 1))
                {
                    bool pickIsCorrect = masterBracket.Picks.Any(x => x.RoundNumber == pick.RoundNumber && x.Conference == pick.Conference && x.PredictedWinningId == pick.PredictedWinningId);
                    if (pickIsCorrect)
                    {
                        bracket.CurrentScore += season.WildcardPoints;
                    }
                    else
                    {
                        eliminatedTeams.Add(pick.PredictedWinningId);
                        bracket.MaxPossibleScore -= season.WildcardPoints;
                    }
                }
            }

            if (masterBracket.Picks.Any(x => x.RoundNumber == 2))
            {
                // Get divisional picks
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 2))
                {
                    bool pickIsCorrect = masterBracket.Picks.Any(x => x.RoundNumber == pick.RoundNumber && x.Conference == pick.Conference && x.PredictedWinningId == pick.PredictedWinningId);
                    if (pickIsCorrect)
                    {
                        bracket.CurrentScore += season.DivisionalPoints;
                    }
                    else
                    {
                        eliminatedTeams.Add(pick.PredictedWinningId);
                        bracket.MaxPossibleScore -= season.DivisionalPoints;
                    }
                }
            }
            else
            {
                // Get divisional picks
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 2))
                {
                    // Check if the team was eliminated in the wildcard round
                    if (eliminatedTeams.Contains(pick.PredictedWinningId))
                    {
                        bracket.MaxPossibleScore -= season.DivisionalPoints;
                        continue;
                    }
                }
            }

            if (masterBracket.Picks.Any(x => x.RoundNumber == 3))
            {
                // Get conference picks
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 3))
                {
                    bool pickIsCorrect = masterBracket.Picks.Any(x => x.RoundNumber == pick.RoundNumber && x.Conference == pick.Conference && x.PredictedWinningId == pick.PredictedWinningId);
                    if (pickIsCorrect)
                    {
                        bracket.CurrentScore += season.ConferencePoints;
                    }
                    else
                    {
                        eliminatedTeams.Add(pick.PredictedWinningId);
                        bracket.MaxPossibleScore -= season.ConferencePoints;
                    }
                }
            }
            else
            {
                // Get conference picks
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 3))
                {
                    // Check if the team was eliminated in the divisional round
                    if (eliminatedTeams.Contains(pick.PredictedWinningId))
                    {
                        bracket.MaxPossibleScore -= season.ConferencePoints;
                        continue;
                    }
                }
            }

            if (masterBracket.Picks.Any(x => x.RoundNumber == 4))
            {
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 4))
                {
                    bool pickIsCorrect = masterBracket.Picks.Any(x => x.RoundNumber == pick.RoundNumber && x.Conference == pick.Conference && x.PredictedWinningId == pick.PredictedWinningId);
                    if (pickIsCorrect)
                    {
                        bracket.CurrentScore += season.SuperBowlPoints;
                    }
                    else
                    {
                        eliminatedTeams.Add(pick.PredictedWinningId);
                        bracket.MaxPossibleScore -= season.SuperBowlPoints;
                    }
                }
            }
            else
            {
                // Get super bowl pick
                foreach (var pick in bracket.Picks.Where(x => x.RoundNumber == 4))
                {
                    // Check if the team was eliminated in the divisional round
                    if (eliminatedTeams.Contains(pick.PredictedWinningId))
                    {
                        bracket.MaxPossibleScore -= season.SuperBowlPoints;
                        continue;
                    }
                }
            }
        }
    }
}
