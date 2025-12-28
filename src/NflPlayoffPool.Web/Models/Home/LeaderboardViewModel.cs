// <copyright file="LeaderboardViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Home
{
    public class LeaderboardViewModel
    {
        public bool ShowLeaderboard { get; set; }
        public List<BracketSummaryModel> Brackets { get; set; } = new List<BracketSummaryModel>();
    }
}