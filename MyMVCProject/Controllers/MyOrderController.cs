using Microsoft.AspNetCore.Mvc;
using MyMVCProject.DataModel;
using MyUserProject.Models;

namespace MyUserProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MyOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class OrderItemDto
        {
            public int OrderItemId { get; set; }
            public int ProductId { get; set; }

            public string Size { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }

            public string ProductTitle { get; set; }
            public string ImageUrl { get; set; }
            public bool HasReviewed { get; set; }
        }
        public class MyOrdersDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }

            public List<OrderItemDto> Items { get; set; }
        }


        [HttpGet("MyOrders")]
        public IActionResult GetMyOrders()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Please login first" });

            int userId = int.Parse(userIdClaim);

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderId)
                .Select(o => new MyOrdersDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,

                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        Size = oi.Size,
                        Quantity = oi.Quantity,
                        Price = oi.Price,

                        ProductTitle = oi.Product.Title,     // CORRECT
                        ImageUrl = oi.Product.ImageUrl,

                        HasReviewed = _context.ProductReviews
                            .Any(r => r.UserId == userId && r.ProductId == oi.ProductId)
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        [HttpPost("AddReview")]
        public IActionResult AddReview([FromBody] ProductReviewDto model)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Please login first" });

            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data" });

            var review = new ProductReview
            {
                UserId = userId,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            _context.SaveChanges();

            return Ok(new { message = "Review added successfully!" });
        }

        public class ProductReviewDto
        {
            public int ProductId { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
        }


    }
}