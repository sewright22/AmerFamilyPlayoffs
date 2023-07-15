namespace PlayoffPool.MVC.Models.Home
{
    public class HomeViewModel
    {
        public List<BracketSummaryModel> CompletedBrackets { get; set; } = new List<BracketSummaryModel>();
        public List<BracketSummaryModel> IncompleteBrackets { get; set; } = new List<BracketSummaryModel>();
        public LeaderboardViewModel? Leaderboard { get; set; }
    }
}
