using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using MyMVCProject.ViewModels;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;

    public CartController(ApplicationDbContext db)
    {
        _db = db;
    }

    // --------------------------------------
    // 1️⃣ ADD TO CART
    // --------------------------------------
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddCartRequest request)
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null)
            return Json(new { success = false, message = "Please login first" });

        int userId = int.Parse(userIdClaim);

        var product = await _db.Products.FindAsync(request.ProductId);
        if (product == null)
            return Json(new { success = false, message = "Product not found" });

        // Check if same product+size already exists
        var existingItem = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId &&
                                      c.ProductId == request.ProductId &&
                                      c.Size == request.Size);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = request.ProductId,
                Size = request.Size,
                Quantity = request.Quantity,
                Price = product.Price,
                AddedOn = DateTime.Now
            };

            _db.CartItems.Add(cartItem);
        }

        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    // DTO for add
    public class AddCartRequest
    {
        public int ProductId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }


    // --------------------------------------
    // 2️⃣ REMOVE ITEM
    // --------------------------------------
    [HttpPost]
    public async Task<IActionResult> Remove([FromBody] RemoveRequest req)
    {
        var item = await _db.CartItems.FindAsync(req.CartItemId);
        if (item == null)
            return Json(new { success = false });

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    public class RemoveRequest
    {
        public int CartItemId { get; set; }
    }


    // --------------------------------------
    // 3️⃣ UPDATE QUANTITY
    // --------------------------------------
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQtyRequest req)
    {
        var item = await _db.CartItems.FindAsync(req.CartItemId);
        if (item == null)
            return Json(new { success = false });

        item.Quantity = req.Quantity;
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    public class UpdateQtyRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }


   

}
