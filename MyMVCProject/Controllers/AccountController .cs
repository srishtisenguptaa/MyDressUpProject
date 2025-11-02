using Microsoft.AspNetCore.Mvc;

namespace MyMVCProject.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Dummy login check (replace with real DB validation)
            if (email == "test@dressup.com" && password == "1234")
            {
                // Extract username before '@'
                var username = email.Split('@')[0];

                // Save both email and username in session
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("Username", username);

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
    }
}
