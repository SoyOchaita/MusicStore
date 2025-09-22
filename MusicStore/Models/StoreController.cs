using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly MusicStoreContext _db;
        public StoreController(MusicStoreContext db) => _db = db;

        // /Store
        public async Task<IActionResult> Index()
        {
            var genres = await _db.Genres
                                  .OrderBy(g => g.Name)
                                  .ToListAsync();
            return View(genres);
        }

        // /Store/Browse?genreId=1
        public async Task<IActionResult> Browse(int genreId)
        {
            var genre = await _db.Genres
                                 .Include(g => g.Albums)
                                 .ThenInclude(a => a.Artist)
                                 .FirstOrDefaultAsync(g => g.Id == genreId);

            if (genre == null) return NotFound();
            return View(genre);
        }

        // /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _db.Albums
                                 .Include(a => a.Genre)
                                 .Include(a => a.Artist)
                                 .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null) return NotFound();
            return View(album);
        }
    }
}
