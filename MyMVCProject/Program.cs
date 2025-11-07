using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel; //  change this namespace to your actual Data folder namespace

var builder = WebApplication.CreateBuilder(args);

//  Add connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext for your SQL Server database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

//  Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
