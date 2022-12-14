using Microsoft.AspNetCore.Mvc;
using PlayoffPool.MVC.Models.Bracket;

namespace PlayoffPool.MVC.Controllers
{
    public class BracketController : Controller
    {
        [HttpGet]
        public IActionResult Index(int? bracketId)
        {
            var model = new BracketViewModel();

            if (bracketId is not null)
            {
                // get bracket info from database.
            }

            model.Name = "Test";
            model.CanEdit = true;

            return View(model);
        }
    }
}
