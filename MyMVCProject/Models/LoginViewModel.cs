using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
