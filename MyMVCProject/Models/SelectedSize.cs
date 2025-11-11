using System;
using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Models
{
    public class SelectedSize
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "Size must be S, M, or L")]
        public string Size { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime SelectedAt { get; set; } = DateTime.Now;
    }
}
