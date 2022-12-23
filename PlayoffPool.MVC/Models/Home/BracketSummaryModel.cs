using System.Drawing;

namespace PlayoffPool.MVC.Models.Home
{
    public class BracketSummaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PredictedWinner { get; set; }
    }
}
