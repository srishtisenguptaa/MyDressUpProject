using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel; // ✅ Make sure this matches your actual namespace

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


// ✅ Add Session Support (with timeout and cookie setup)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add HttpContextAccessor (for accessing session in controllers)
builder.Services.AddHttpContextAccessor();

// ✅ Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------------------------------------------
// 2️⃣ Build Application
// -------------------------------------------------------
var app = builder.Build();

// -------------------------------------------------------
// 3️⃣ Configure Middleware Pipeline
// -------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ Session must come BEFORE Routing
app.UseSession();

app.UseRouting();
app.UseAuthorization();

// -------------------------------------------------------
// 4️⃣ Route Configuration
// -------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -------------------------------------------------------
// 5️⃣ Run the App
// -------------------------------------------------------
app.Run();
