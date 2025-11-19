using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using MyMVCProject.ViewModels;
using System.Diagnostics;

namespace MyMVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger,  ApplicationDbContext context, ApplicationDbContext db)
        {
            _logger = logger;
            _context = context;
            _db = db;
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

        public async Task<IActionResult> Cart()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim);

            var user = await _db.Users
                .Include(u => u.UserAddress)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            var items = await _db.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();

            // Get previously selected address from session
            var selectedAddressId = HttpContext.Session.GetInt32("SelectedAddressId");
            ViewBag.SelectedAddressId = selectedAddressId;

            var vm = items.Select(i => new CartItemViewModel
            {
                CartItemId = i.CartItemId,
                Title = i.Product.Title,
                ImageUrl = i.Product.ImageUrl,
                Size = i.Size,
                Quantity = i.Quantity,
                Price = i.Price,

                Addresses = user.UserAddress?
                    .Select(a => new UserAddressViewModel
                    {
                        AddressId = a.AddressId,
                        UserName = user.FullName,
                        FullAddress = a.FullAddress
                    }).ToList() ?? new List<UserAddressViewModel>()
            }).ToList();

            return View("~/Views/Home/Cart.cshtml", vm);
        }

        public IActionResult MyOrders()
        {
            return View("~/Views/MyOrder/MyOrders.cshtml");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
