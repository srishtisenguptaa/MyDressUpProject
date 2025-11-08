using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Models
{
    public class Product
    {

        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public double Rating { get; set; }

        public string? Gender { get; set; }

        public string? Type { get; set; }

        public string? ImageUrl { get; set; }

        public bool InStock { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
