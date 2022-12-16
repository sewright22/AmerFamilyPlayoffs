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

        public Matchup AfcRound1Game1 { get; set; }
    }
}
