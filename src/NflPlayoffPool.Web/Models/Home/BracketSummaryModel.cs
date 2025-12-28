// <copyright file="BracketSummaryModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Models.Home
{
    using NflPlayoffPool.Web.Models.Bracket;

    public class BracketSummaryModel
    {
        public string Id { get; set; }
        public int Place { get; set; }
        public string? PlaceAsString { get; set; }

        public string Name { get; set; }
        public int CurrentScore { get; set; }
        public int MaxPossibleScore { get; set; }
        public string? PredictedWinner { get; set; }
    }
}
