using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMVCProject.Models
{
    [Table("OtpVerifications")]
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string OtpCode { get; set; } = string.Empty;

        [Column("ExpiryTime")]   // 👈 This maps the C# property to your DB column
        public DateTime ExpirationTime { get; set; }
    }
}
