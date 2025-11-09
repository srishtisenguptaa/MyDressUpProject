using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMVCProject.Models
{
    public class UserAddress
    {
        [Key] // ✅ This tells EF it's the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Address { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
