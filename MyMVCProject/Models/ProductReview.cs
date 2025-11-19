using MyMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyUserProject.Models
{
    public class ProductReview
    {
        [Key]
        public int ReviewId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int ProductId { get; set; }

        // Review Data
        public int Rating { get; set; } // 1 – 5
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // Navigation Properties
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
