using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayoffPool.MVC.Models.Admin
{
    public class AdminRoundViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PointValue { get;set; }
        public List<string>? SelectedTeams { get; set; }
        public List<SelectListItem> Teams { get; set; } = new List<SelectListItem>();
    }
}
