using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class Matchup
    {
        public string Name { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public int SelectedWinner { get; set; }
        public int ActualWinner { get; set; }
    }
}
