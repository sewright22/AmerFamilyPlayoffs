// <copyright file="HomeViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Home
{
    public class HomeViewModel
    {
        public List<BracketSummaryModel> CompletedBrackets { get; set; } = new List<BracketSummaryModel>();
        public List<BracketSummaryModel> IncompleteBrackets { get; set; } = new List<BracketSummaryModel>();
        public LeaderboardViewModel? Leaderboard { get; set; }
        public string CurrentServerTime { get; set; }
        public bool IsPlayoffStarted { get; set; }
        public bool CanSubmitBrackets { get; set; }
    }
}
