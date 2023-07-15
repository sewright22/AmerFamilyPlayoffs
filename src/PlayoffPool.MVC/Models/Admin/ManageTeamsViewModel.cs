using System.ComponentModel.DataAnnotations;
using PlayoffPool.MVC.Models.Admin;

namespace PlayoffPool.MVC.Models
{
    public class ManageTeamsViewModel
    {
        [Display(Name = "Year")]
        public string Year { get; set; } = string.Empty;
        public List<AdminRoundViewModel> Rounds { get; } = new List<AdminRoundViewModel>();
    }
}
