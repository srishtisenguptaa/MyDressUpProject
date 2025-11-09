using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using System.Diagnostics;

namespace MyMVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,  ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public PartialViewResult WomenSection()
        {
            return PartialView();
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

        [HttpGet]
        [HttpGet]
        public IActionResult SearchProducts(string query)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim(); // remove extra spaces

                productsQuery = productsQuery.Where(p =>
                    EF.Functions.Like(p.Type, $"%{query}%") ||
                    EF.Functions.Like(p.Title, $"%{query}%")
                );
            }

            var products = productsQuery
                .Select(p => new
                {
                    p.ProductId,
                    p.Title,
                    p.ImageUrl,
                    p.Price,
                    p.Rating
                })
                .ToList();

            return Json(products);
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
