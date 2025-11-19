using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyMVCProject.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        // Navigation
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(10)]
        public string Size { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Navigation
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public Product Product { get; set; }
    }
}