using MyMVCProject.Models;
using System.ComponentModel.DataAnnotations;

namespace MyUserProject.ViewModels
{
    public class MyOrdersViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemVM> Items { get; set; }
    }

    public class OrderItemVM
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // ADD THIS ↓↓↓
        public Product Product { get; set; }
        public string ProductTitle { get; set; }
        public string ImageUrl { get; set; }
        public bool HasReviewed { get; set; }
    }

    
}
