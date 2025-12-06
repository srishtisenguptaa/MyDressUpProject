using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using MyUserProject.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace MyMVCProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;

        public AccountController(ApplicationDbContext context, EmailService emailService, JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        // ---------------- LOGIN NORMAL ----------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(null, user.PasswordHash, password);

            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            await SignInUser(user);
            return RedirectToAction("Index", "Home");

            
        }

        // ---------------- LOGOUT ----------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ---------------- REGISTER ----------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string FullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered!";
                return View();
            }

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                FullName = FullName,
                Email = email,
                PasswordHash = hasher.HashPassword(null, password),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }

        // ---------------- OTP LOGIN ----------------
        [HttpGet]
        public IActionResult OtpLogin()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult SendOtp(string email)
        {
            string otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTPEmail", email);
            HttpContext.Session.SetString("OTPExpiry", DateTime.UtcNow.AddMinutes(2).ToString());

            _emailService.SendOtpAsync(email, otp);

            return Json(new { success = true });
        }


        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");
            var storedEmail = HttpContext.Session.GetString("OTPEmail");
            var expiryStr = HttpContext.Session.GetString("OTPExpiry");

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(expiryStr) || string.IsNullOrEmpty(storedEmail))
                return Json(new { success = false, message = "OTP expired or not generated." });

            if (!storedEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                return Json(new { success = false, message = "Email mismatch — request a new OTP." });

            // validate expiry
            if (!DateTime.TryParse(expiryStr, out var expiry))
                return Json(new { success = false, message = "Invalid OTP session data." });

            if (DateTime.UtcNow > expiry)
                return Json(new { success = false, message = "OTP expired. Please request a new one." });

            // validate OTP
            if (otp == storedOtp)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                    return Json(new { success = false, message = "User not found." });

                await SignInUser(user);

                // clear session OTP after success
                HttpContext.Session.Remove("OTP");
                HttpContext.Session.Remove("OTPExpiry");
                HttpContext.Session.Remove("OTPEmail");

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }

            return Json(new { success = false, message = "Invalid OTP" });
        }





        // ---------------- PRIVATE SIGN-IN METHOD ----------------
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMonths(6)
                });
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> SendResetLink(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Message = "If this email exists, a reset link was sent.";
                return View("ForgetPassword");
            }

            string token = Guid.NewGuid().ToString();

            user.PasswordResetToken = token;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(30);

            _context.SaveChanges();

            string resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token },
                Request.Scheme);

            await _emailService.SendResetLinkAsync(email, resetLink);

            ViewBag.Message = "Reset link sent to your email.";
            return View("ForgetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgetPassword");
            }

            var user = _context.Users.FirstOrDefault(x =>
                x.PasswordResetToken == token &&
                x.PasswordResetExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return RedirectToAction("ForgetPassword");
            }

            ViewBag.Token = token; // ✅ send token to view
            return View();
        }

        [HttpPost]
        
        public IActionResult ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Token = token;
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            var user = _context.Users.FirstOrDefault(x =>
                x.PasswordResetToken == token &&
                x.PasswordResetExpiry > DateTime.UtcNow);

            if (user == null)
                return RedirectToAction("ForgetPassword"); // typo fixed

            // Hash and save password
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(null, newPassword);

            // Clear token
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            _context.SaveChanges();

            
    
            return RedirectToAction("Login");
        }






    }
}
