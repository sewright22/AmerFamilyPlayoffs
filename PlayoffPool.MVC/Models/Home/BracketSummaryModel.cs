using System.Drawing;
using PlayoffPool.MVC.Models.Bracket;

namespace PlayoffPool.MVC.Models.Home
{
    public class BracketSummaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CurrentScore { get; set; }
        public int MaxPossibleScore { get; set; }
        public TeamViewModel PredictedWinner { get; set; }
    }
}
