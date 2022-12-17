using System.ComponentModel.DataAnnotations;
using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class BracketViewModel
    {
        [Display(Name = "Bracket Name", Prompt = "Please name your bracket")]
        [Required]
        public string? Name { get; set; }

        public List<Round> Rounds { get; set; }

        public bool CanEdit { get; set; }

        public Matchup? AfcWildCardGame1 { get; set; }
        public Matchup? NfcWildCardGame1 { get; set; }

        public Matchup? AfcWildCardGame2 { get; set; }
        public Matchup? NfcWildCardGame2 { get; set; }

        public Matchup? AfcWildCardGame3 { get; set; }
        public Matchup? NfcWildCardGame3 { get; set; }

        public Matchup? AfcDivisionalGame1 { get; set; }
        public Matchup? NfcDivisionalGame1 { get; set; }

        public Matchup? AfcDivisionalGame2 { get; set; }
        public Matchup? NfcDivisionalGame2 { get; set; }

        public Matchup? AfcChampionship { get; set; }
        public Matchup? NfcChampionship { get; set; }
        public Matchup? SuperBowl { get; set; }
    }
}
