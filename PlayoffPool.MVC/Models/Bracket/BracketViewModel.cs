using System.ComponentModel.DataAnnotations;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class BracketViewModel
    {
        [Display(Name = "Bracket Name", Prompt = "Please name your bracket")]
        [Required]
        public string? Name { get; set; }

        public bool CanEdit { get; set; }
    }
}
