using System.Diagnostics;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asp_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            TempData["Message"] = "Witaj w aplikacji!";
            return RedirectToAction("Login", "Account");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
         public IActionResult Error()
         {
            return View("Error");
        }
    }
}
