using MyMVCProject.Models;

public class PersonalInfoViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }

    // List of user addresses
    public List<UserAddress>? Addresses { get; set; }
}

public class EditAddressDto
{
    public int AddressId { get; set; }  // must match backend primary key
    public string Address { get; set; }
    public string Pincode { get; set; }
    public string District { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
}

