using System;

namespace MyMVCProject.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedOn { get; set; }

        // Navigation
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
