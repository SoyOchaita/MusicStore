using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace MusicStore.Controllers
{
    public class StoreManagerController : Controller
    {
        private readonly MusicStoreContext _context;

        public StoreManagerController(MusicStoreContext context)
        {
            _context = context;
        }

        // GET: StoreManager
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .OrderBy(a => a.Title)
                .ToListAsync();

            return View(albums);
        }

        // GET: StoreManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: StoreManager/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres.OrderBy(g => g.Name), "Id", "Name");
            ViewData["ArtistId"] = new SelectList(_context.Artists.OrderBy(a => a.Name), "Id", "Name");
            return View();
        }

        // POST: StoreManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Price,GenreId,ArtistId")] Album album)
        {
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres.OrderBy(g => g.Name), "Id", "Name", album.GenreId);
            ViewData["ArtistId"] = new SelectList(_context.Artists.OrderBy(a => a.Name), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // GET: StoreManager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();

            ViewData["GenreId"] = new SelectList(_context.Genres.OrderBy(g => g.Name), "Id", "Name", album.GenreId);
            ViewData["ArtistId"] = new SelectList(_context.Artists.OrderBy(a => a.Name), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // POST: StoreManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,GenreId,ArtistId")] Album album)
        {
            if (id != album.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["GenreId"] = new SelectList(_context.Genres.OrderBy(g => g.Name), "Id", "Name", album.GenreId);
                ViewData["ArtistId"] = new SelectList(_context.Artists.OrderBy(a => a.Name), "Id", "Name", album.ArtistId);
                return View(album);
            }

            try
            {
                _context.Update(album);                  // <-- importante
                await _context.SaveChangesAsync();       // <-- importante
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Albums.Any(a => a.Id == id)) return NotFound();
                throw;
            }
        }

        // GET: StoreManager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
