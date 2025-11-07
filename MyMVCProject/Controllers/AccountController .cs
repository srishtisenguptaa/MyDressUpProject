using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using System.Linq;

namespace MyMVCProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Check if user exists in database
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                // Extract only first name from FullName
                var firstName = user.FullName.Split(' ')[0];

                // Save email and first name in session
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Username", firstName);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public IActionResult Register(string FullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered!";
                return View();
            }

            // Create a new User entity (not LoginViewModel)
            var user = new User
            {
                FullName = FullName,
                Email = email,
                PasswordHash = password,  // Later we’ll hash this
                CreatedAt = DateTime.Now
            };
            if (user != null)
            {
                // Extract only first name from FullName
                var firstName = user.FullName.Split(' ')[0];

                // Save email and first name in session
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Username", firstName);
            }
                _context.Users.Add(user);
            _context.SaveChanges();



            // Redirect to login after successful registration
            return RedirectToAction("Index", "Home");
        }





    }
}
