using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel; // ✅ Replace with your actual namespace

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
// 1️⃣ Register Services
// -------------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Optional: Session (if you need it for other purposes)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor (for accessing session or other context in controllers)
builder.Services.AddHttpContextAccessor();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------------------------------------------
// 2️⃣ Cookie-based Authentication (Persistent Login)
// -------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // Redirect here if not logged in
        options.LogoutPath = "/Account/Logout"; // Redirect here on logout
        options.ExpireTimeSpan = TimeSpan.FromDays(365); // Persistent for 1 year
        options.SlidingExpiration = true;      // Extend expiration on activity
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// -------------------------------------------------------
// 3️⃣ Build Application
// -------------------------------------------------------
var app = builder.Build();

// -------------------------------------------------------
// 4️⃣ Middleware Pipeline
// -------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Optional: keep session for other purposes
app.UseSession();

// -------------------------------------------------------
// 5️⃣ Route Configuration
// -------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -------------------------------------------------------
// 6️⃣ Run the App
// -------------------------------------------------------
app.Run();
