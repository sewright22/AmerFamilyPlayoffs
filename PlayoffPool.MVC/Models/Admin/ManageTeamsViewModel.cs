using PlayoffPool.MVC.Models.Admin;

namespace PlayoffPool.MVC.Models
{
    public class ManageTeamsViewModel
    {
        public YearViewModel? YearViewModel { get; set; }
        public List<AdminRoundViewModel> RoundViewModel { get; } = new List<AdminRoundViewModel>();
    }
}
