using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MusicStoreContext _db;
        public AdminController(MusicStoreContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                Products = await _db.Albums.CountAsync(),
                Orders = await _db.Orders.CountAsync()
            };
            ViewBag.Stats = stats;
            return View();
        }
    }
}