using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore;
using MusicStore.Models;
using MusicStore.Services; // Usar el servicio de carrito unificado

namespace MusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly MusicStoreContext _db;

        public ShoppingCartController(IServiceProvider services, MusicStoreContext db)
        {
            _services = services;
            _db = db;
        }

        // GET: /ShoppingCart
        public IActionResult Index()
        {
            var cart = ShoppingCart.GetCart(_services);
            var items = cart.GetCartItems();
            ViewBag.Total = cart.GetTotal();
            return View(items);
        }

        // POST: /ShoppingCart/AddToCart/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id)
        {
            var album = await _db.Albums
                                 .Include(a => a.Artist)
                                 .Include(a => a.Genre)
                                 .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null) return NotFound();

            var cart = ShoppingCart.GetCart(_services);
            cart.AddToCart(album);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/RemoveFromCart/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(_services);
            cart.RemoveFromCart(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/EmptyCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmptyCart()
        {
            var cart = ShoppingCart.GetCart(_services);
            await cart.EmptyCartAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}