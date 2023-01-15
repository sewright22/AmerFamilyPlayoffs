using System.Drawing;

namespace PlayoffPool.MVC.Models.Home
{
    public class BracketSummaryModel
    {
        public int Id { get; set; }
        public string? Place { get; set; }
        public string Name { get; set; }
        public int CurrentScore { get; set; }
        public int MaxPossibleScore { get; set; }
        public PlayoffTeamViewModel PredictedWinner { get; set; }
    }
}
