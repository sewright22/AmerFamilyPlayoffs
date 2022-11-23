using System.Diagnostics;
using AmerFamilyPlayoffs.Data;
using AmerFamilyPlayoffs.Data.SeedExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AmerFamilyPlayoffContext dataContext;

        public HomeController(ILogger<HomeController> logger, AmerFamilyPlayoffContext dataContext)
        {
            _logger = logger;
            this.dataContext = dataContext;
            this.dataContext.Database.Migrate();
            this.dataContext.SeedData();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}