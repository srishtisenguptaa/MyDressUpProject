using System;
using System.ComponentModel.DataAnnotations;

namespace PlayDressUp.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required, StringLength(10)]
        public string Size { get; set; } = "M";

        public int Quantity { get; set; } = 1;

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
