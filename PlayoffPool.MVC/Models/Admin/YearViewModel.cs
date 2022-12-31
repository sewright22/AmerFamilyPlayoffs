using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayoffPool.MVC.Models.Admin
{
    public class YearViewModel
    {
        public string? SelectedYear { get; set; }
        public List<SelectListItem> Years { get; } = new List<SelectListItem>();
    }
}
