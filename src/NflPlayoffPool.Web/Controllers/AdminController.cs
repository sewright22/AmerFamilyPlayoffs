// <copyright file="AdminController.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models;
    using NflPlayoffPool.Web.ViewModels;

    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public PlayoffPoolContext DbContext { get; }

        public AdminController(ILogger<AdminController> logger, PlayoffPoolContext dbContext)
        {
            _logger = logger;
            this.DbContext = dbContext;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
