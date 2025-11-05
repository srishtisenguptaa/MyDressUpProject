using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Models;

namespace MyMVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult WomenSection()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult WishList()
        {
            return View();
        }
        public IActionResult AboutMe()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Cart()
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
