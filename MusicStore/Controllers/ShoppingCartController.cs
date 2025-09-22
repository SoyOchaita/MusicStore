using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly MusicStoreContext _db;
        public ShoppingCartController(IServiceProvider services, MusicStoreContext db)
        { _services = services; _db = db; }

        // GET /ShoppingCart
        public async Task<IActionResult> Index()
        {
            var cart = ShoppingCart.GetCart(_services);
            var items = await cart.GetItemsAsync();
            ViewBag.Total = await cart.GetTotalAsync();
            return View(items);
        }

        // POST /ShoppingCart/Add/5
        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            var album = await _db.Albums.FirstOrDefaultAsync(a => a.Id == id);
            if (album == null) return NotFound();
            await ShoppingCart.GetCart(_services).AddToCartAsync(album);
            return RedirectToAction(nameof(Index));
        }

        // POST /ShoppingCart/Remove/5
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            await ShoppingCart.GetCart(_services).RemoveFromCartAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
