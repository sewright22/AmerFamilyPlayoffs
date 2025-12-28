// <copyright file="HomeController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NflPlayoffPool.Web.Areas.Admin.Models;
    using NflPlayoffPool.Web.Controllers;

    [Area("Admin")]
    public class HomeController : Controller
    {
        public HomeController(ILogger<AdminController> logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Logger = logger;
        }

        public ILogger<AdminController> Logger { get; }

        public IActionResult Index()
        {
            return this.View(new AdminModel());
        }
    }
}
