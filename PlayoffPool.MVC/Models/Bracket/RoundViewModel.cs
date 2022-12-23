namespace PlayoffPool.MVC.Models.Bracket
{
    public class RoundViewModel
    {
        public int Id { get; set; }
        public required int RoundNumber { get; set; }
        public int PointValue { get; set; }
        public string Name { get; set; }
        public string? Conference { get; set; }
        public bool IsLocked { get; set; }
        public List<MatchupViewModel> Games { get; set; } = new List<MatchupViewModel>();
    }
}
