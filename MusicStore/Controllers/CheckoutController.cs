using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
using MusicStore.Services; 

namespace MusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly MusicStoreContext _db;

        public CheckoutController(IServiceProvider services, MusicStoreContext db)
        {
            _services = services;
            _db = db;
        }

        // GET /Checkout/AddressAndPayment
        [HttpGet]
        public IActionResult AddressAndPayment()
        {
            var order = new Order
            {
                Username = User?.Identity?.Name ?? string.Empty
            };
            return View(order);
        }

        // POST /Checkout/AddressAndPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(Order order)
        {
            if (!ModelState.IsValid) return View(order);

            var cart = ShoppingCart.GetCart(HttpContext.RequestServices);
            var items = await cart.GetItemsAsync();
            if (!items.Any())
            {
                ModelState.AddModelError(string.Empty, "El carrito está vacío.");
                return View(order);
            }

            order.Username = User?.Identity?.Name ?? string.Empty;
            order.OrderDate = DateTime.UtcNow;
            order.Total = await cart.GetTotalAsync();

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in items)
            {
                _db.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    AlbumId = item.AlbumId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Album?.Price ?? 0m
                });
            }
            await _db.SaveChangesAsync();
            await cart.EmptyCartAsync();

            return RedirectToAction(nameof(Complete), new { id = order.OrderId });
        }

        // GET /Checkout/Complete/{id}
        [HttpGet]
        public async Task<IActionResult> Complete(int id)
        {
            var userName = User?.Identity?.Name ?? "";
            var order = await _db.Orders
                                 .Include(o => o.OrderDetails!)
                                     .ThenInclude(d => d.Album)
                                 .FirstOrDefaultAsync(o => o.OrderId == id && o.Username == userName);

            if (order == null) return NotFound();
            return View(order);
        }
    }
}