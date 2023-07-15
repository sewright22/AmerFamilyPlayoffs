using System.ComponentModel.DataAnnotations;
using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class MatchupViewModel
    {
        public int GameNumber { get; set; }
        public string Name { get; set; }
        public PlayoffTeamViewModel HomeTeam { get; set; }
        public PlayoffTeamViewModel AwayTeam { get; set; }

        [Required(ErrorMessage = "Pick a winner for this game")]
        public int? SelectedWinner { get; set; }
        public bool? IsCorrect { get; set; }
        public bool IsLocked { get; set; }
    }
}
