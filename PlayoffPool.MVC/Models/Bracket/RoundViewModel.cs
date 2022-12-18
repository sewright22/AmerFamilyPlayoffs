namespace PlayoffPool.MVC.Models.Bracket
{
    public class RoundViewModel
    {
        public required int RoundNumber { get; set; }
        public string? Conference { get; set; }
        public List<MatchupViewModel> Games { get; set; } = new List<MatchupViewModel>();
    }
}
