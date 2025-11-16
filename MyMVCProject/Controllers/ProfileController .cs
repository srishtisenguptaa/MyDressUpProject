using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;

public class ProfileController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public ProfileController(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

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

            // Fetch user details
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

            // Fetch user addresses with all fields
            SqlCommand addressCmd = new SqlCommand(
                "SELECT AddressId, Address, Pincode, District, State, City , Country FROM UserAddresses WHERE UserId = @UserId", con);
            addressCmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataReader addrReader = addressCmd.ExecuteReader();

            model.Addresses = new List<UserAddress>();
            while (addrReader.Read())
            {
                model.Addresses.Add(new UserAddress
                {
                    AddressId = (int)addrReader["AddressId"],
                    Address = addrReader["Address"].ToString(),
                    Pincode = addrReader["Pincode"]?.ToString(),
                    District = addrReader["District"]?.ToString(),
                    State = addrReader["State"]?.ToString(),
                    City = addrReader["City"]?.ToString(),
                    Country = addrReader["Country"]?.ToString()
                });
            }

            addrReader.Close();
            return View(model);
        }
    }

    // ✅ Add Address (using LINQ/EF)
    [HttpPost]
    public IActionResult AddAddress([FromBody] UserAddress model)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null)
            return Json(new { success = false, message = "Please log in first." });

        int userId = int.Parse(userIdClaim);

        if (string.IsNullOrWhiteSpace(model.Address))
            return Json(new { success = false, message = "Address cannot be empty." });

        // Create new address record
        var newAddress = new UserAddress
        {
            UserId = userId,
            Address = model.Address?.Trim(),
            Pincode = model.Pincode?.Trim(),
            District = model.District?.Trim(),
            State = model.State?.Trim(),
            City = model.City?.Trim(),
            Country = model.Country?.Trim() ?? "India"
        };

        _context.UserAddresses.Add(newAddress);
        _context.SaveChanges();

        return Json(new { success = true, message = "Address added successfully!" });
    }

    // ✅ Edit Address (using EF)
    [HttpPost]

    [HttpPost]
    public IActionResult EditAddress([FromBody] EditAddressDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Address))
            return Json(new { success = false, message = "Address cannot be empty." });

        var address = _context.UserAddresses
                             .FirstOrDefault(a => a.AddressId == model.AddressId);

        if (address == null)
            return Json(new { success = false, message = "Address not found." });

        // Update fields
        address.Address = model.Address;
        address.Pincode = model.Pincode;
        address.District = model.District;
        address.State = model.State;
        address.Country = model.Country;
        address.City = model.City;
        _context.SaveChanges();

        return Json(new { success = true, message = "Address updated successfully!" });
    }




    // ✅ Delete Address
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
}
