using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyMVCProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public AccountController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // -----------------------------
        // LOGIN
        // -----------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in with persistent cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,                      // Persist across browser sessions
                        ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1) // 1 year expiration
                    });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // -----------------------------
        // LOGOUT
        // -----------------------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // -----------------------------
        // REGISTER
        // -----------------------------
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

            var user = new User
            {
                FullName = FullName,
                Email = email,
                PasswordHash = password, // TODO: Hash this in real app
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Automatically log in the user after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1)
                });

            return RedirectToAction("Index", "Home");
        }

        // OTP and JWT AUTH

        [HttpGet]
        public IActionResult OtpLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendOtp(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return Json(new { success = false, message = "Email not registered!" });

            var otp = new Random().Next(100000, 999999).ToString();

            var existing = _context.OtpVerifications.FirstOrDefault(o => o.Email == email);
            if (existing != null)
            {
                existing.OtpCode = otp;
                existing.ExpirationTime = DateTime.Now.AddMinutes(5);
            }
            else
            {
                _context.OtpVerifications.Add(new OtpVerification
                {
                    Email = email,
                    OtpCode = otp,
                    ExpirationTime = DateTime.Now.AddMinutes(5)
                });
            }
            _context.SaveChanges();

            // TODO: send OTP via email (for testing just return)
            return Json(new { success = true, otp = otp, message = "OTP sent successfully (for testing)" });
        }

        [HttpPost]
      
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var record = _context.OtpVerifications.FirstOrDefault(o => o.Email == email && o.OtpCode == otp);
            if (record == null || record.ExpirationTime < DateTime.Now)
                return Json(new { success = false, message = "Invalid or expired OTP" });

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            // ✅ Generate JWT (for future API or hybrid use)
            var token = GenerateJwtToken(user);

            // ✅ Create claims for cookie authentication (to show username in navbar)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim("UserId", user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ Sign in user (like normal login)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

            // ✅ Remove used OTP
            _context.OtpVerifications.Remove(record);
            _context.SaveChanges();

            // ✅ Return success with token (you can store it in localStorage if needed)
            return Json(new { success = true, token });
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FullName", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
