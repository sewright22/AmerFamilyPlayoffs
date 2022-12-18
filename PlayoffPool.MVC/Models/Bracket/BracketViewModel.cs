using System.ComponentModel.DataAnnotations;
using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class BracketViewModel
    {
        [Display(Name = "Bracket Name", Prompt = "Please name your bracket")]
        [Required(ErrorMessage = "Don't forget to name your bracket.")]
        public string? Name { get; set; }

        public bool CanEdit { get; set; }

        public MatchupViewModel? AfcWildCardGame1 { get; set; }
        public MatchupViewModel? NfcWildCardGame1 { get; set; }

        public MatchupViewModel? AfcWildCardGame2 { get; set; }
        public MatchupViewModel? NfcWildCardGame2 { get; set; }

        public MatchupViewModel? AfcWildCardGame3 { get; set; }
        public MatchupViewModel? NfcWildCardGame3 { get; set; }

        public MatchupViewModel? AfcDivisionalGame1 { get; set; }
        public MatchupViewModel? NfcDivisionalGame1 { get; set; }

        public MatchupViewModel? AfcDivisionalGame2 { get; set; }
        public MatchupViewModel? NfcDivisionalGame2 { get; set; }

        public MatchupViewModel? AfcChampionship { get; set; }
        public MatchupViewModel? NfcChampionship { get; set; }
        public MatchupViewModel? SuperBowl { get; set; }
    }
}
