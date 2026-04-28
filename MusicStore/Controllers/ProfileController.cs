using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly MusicStoreContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public ProfileController(MusicStoreContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var username = User?.Identity?.Name ?? "";
            var orders = await _db.Orders
                                  .Where(o => o.Username == username)
                                  .OrderByDescending(o => o.OrderDate)
                                  .ToListAsync();
            return View(orders);
        }
    }
}