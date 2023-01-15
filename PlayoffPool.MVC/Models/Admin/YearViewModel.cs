using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayoffPool.MVC.Models.Admin
{
    public class YearViewModel
    {
        [Display(Name = "Year")]
        public string Year { get; set; }
    }
}
