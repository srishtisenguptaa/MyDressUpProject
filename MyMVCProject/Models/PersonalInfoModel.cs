using MyMVCProject.Models;

public class PersonalInfoViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }

    // List of user addresses
    public List<UserAddress>? Addresses { get; set; }
}
