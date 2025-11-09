using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
public class ProfileController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    public ProfileController(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
         _context = context;
    }
    //public ProfileController(ApplicationDbContext context)
    //{
    //   
    //}

    [HttpGet]
    public IActionResult PersonalInfo()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(userIdClaim);

        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT FullName, Email FROM Users WHERE UserId = @UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader reader = cmd.ExecuteReader();

            var model = new PersonalInfoViewModel();

            if (reader.Read())
            {
                model.FullName = reader["FullName"].ToString();
                model.Email = reader["Email"].ToString();
            }
            reader.Close();

            // Get all addresses
            SqlCommand addressCmd = new SqlCommand("SELECT AddressId, Address FROM UserAddresses WHERE UserId = @UserId", con);
            addressCmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader addrReader = addressCmd.ExecuteReader();

            model.Addresses = new List<UserAddress>();
            while (addrReader.Read())
            {
                model.Addresses.Add(new UserAddress
                {
                    AddressId = (int)addrReader["AddressId"],
                    Address = addrReader["Address"].ToString()
                });
            }

            return View(model);
        }
    }

    [HttpPost]
    public IActionResult AddAddress(string address)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null)
            return Json(new { success = false, message = "Please log in first." });

        int userId = int.Parse(userIdClaim);

        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO UserAddresses (UserId, Address) VALUES (@UserId, @Address)", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.ExecuteNonQuery();
        }

        return Json(new { success = true, message = "Address added successfully!" });
    }

    [HttpPost]
    public IActionResult DeleteAddress(int id)
    {
        var addr = _context.UserAddresses.FirstOrDefault(a => a.AddressId == id);
        if (addr == null)
            return Json(new { success = false, message = "Address not found." });

        _context.UserAddresses.Remove(addr);
        _context.SaveChanges();

        return Json(new { success = true, message = "Address deleted successfully!" });
    }

    [HttpPost]
    [HttpPost]
    public IActionResult EditAddress(int id, string newAddress)
    {
        if (string.IsNullOrWhiteSpace(newAddress))
            return Json(new { success = false, message = "Address cannot be empty." });

        var address = _context.UserAddresses.FirstOrDefault(a => a.AddressId == id);

        if (address == null)
            return Json(new { success = false, message = "Address not found." });

        // Update value
        address.Address = newAddress.Trim();

        // Tell EF to track this entity as modified
        _context.UserAddresses.Update(address);
        _context.SaveChanges();

        return Json(new { success = true, message = "Address updated successfully!" });
    }

}
