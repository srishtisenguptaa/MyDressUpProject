using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.DataModel;
using MyMVCProject.Models;
using MyMVCProject.ViewModels;
using System.Text.Json;

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
        var selectedAddressId = HttpContext.Session.GetInt32("SelectedAddressId");

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
    [HttpPost]
    [HttpPost]
    public IActionResult SaveAddressSelection([FromBody] JsonElement data)
    {
        int selectedAddressId = data.GetProperty("addressId").GetInt32();
        HttpContext.Session.SetInt32("SelectedAddressId", selectedAddressId);
        return Json(new { success = true });
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (userIdClaim == null)
            return Json(new { success = false, message = "Please login first" });

        int userId = int.Parse(userIdClaim);

        // Get selected address from session (nullable)
        int? selectedAddressId = HttpContext.Session.GetInt32("SelectedAddressId");

        var cartItems = await _db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return Json(new { success = false, message = "Cart is empty" });

        // Create Order
        var order = new Order
        {
            UserId = userId,
            AddressId = (int)selectedAddressId,  // Can be null if user has no address
            TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
            OrderDate = DateTime.Now,
            Status = "Confirmed"
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Create OrderItems
        foreach (var item in cartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Size = item.Size,
                Quantity = item.Quantity,
                Price = item.Price
            };
            _db.OrderItems.Add(orderItem);
        }

        // Clear cart
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }


    public class UpdateQtyRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }


   

}
