using System.ComponentModel.DataAnnotations;
using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class BracketViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bracket Name", Prompt = "Please name your bracket")]
        [Required(ErrorMessage = "Don't forget to name your bracket.")]
        public string? Name { get; set; }

        public bool CanEdit { get; set; }

        public List<RoundViewModel> AfcRounds { get; set; } = new List<RoundViewModel>();
        public List<RoundViewModel> NfcRounds { get; set; } = new List<RoundViewModel>();
        public MatchupViewModel? SuperBowl { get; set; }
    }
}
