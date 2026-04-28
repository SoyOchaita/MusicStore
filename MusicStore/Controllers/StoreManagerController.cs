using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicStore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StoreManagerController : Controller
    {
        private readonly MusicStoreContext _context;
        private readonly IWebHostEnvironment _env;

        public StoreManagerController(MusicStoreContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /StoreManager
        public async Task<IActionResult> Index(string? stockOrder)
        {
            var resolvedStockOrder = string.IsNullOrWhiteSpace(stockOrder) ? "stockDesc" : stockOrder;
            ViewBag.SelectedStockOrder = resolvedStockOrder;

            var query = _context.Albums
                .Include(product => product.Genre)
                .Include(product => product.Artist)
                .AsQueryable();

            query = resolvedStockOrder switch
            {
                "stockAsc" => query
                    .OrderBy(product => product.Stock)
                    .ThenBy(product => product.Code),
                "codeAsc" => query
                    .OrderBy(product => product.Code),
                _ => query
                    .OrderByDescending(product => product.Stock)
                    .ThenBy(product => product.Code)
            };

            var products = await query.ToListAsync();

            return View(products);
        }

        // GET: /StoreManager/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null) return NotFound();
            return View(album);
        }

        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            PopulateLookups();
            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Title,ProductClass,Stock,Price,GenreId,ArtistId,AlbumArtUrl")] Album album, IFormFile? albumImage)
        {
            if (!ModelState.IsValid)
            {
                PopulateLookups(album.GenreId, album.ArtistId);
                return View(album);
            }

            // Si viene archivo, se guarda en /wwwroot/uploads/albums y se asigna la ruta relativa
            if (albumImage is { Length: > 0 })
            {
                album.AlbumArtUrl = await SaveFileAsync(albumImage, "uploads/albums");
            }

            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();

            PopulateLookups(album.GenreId, album.ArtistId);
            return View(album);
        }

        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Title,ProductClass,Stock,Price,GenreId,ArtistId,AlbumArtUrl")] Album album, IFormFile? albumImage)
        {
            if (id != album.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateLookups(album.GenreId, album.ArtistId);
                return View(album);
            }

            // Si se sube un archivo nuevo, reemplaza la URL/Path
            if (albumImage is { Length: > 0 })
            {
                album.AlbumArtUrl = await SaveFileAsync(albumImage, "uploads/albums");
            }

            try
            {
                _context.Update(album);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Albums.AnyAsync(a => a.Id == id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /StoreManager/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (album == null) return NotFound();
            return View(album);
        }

        // POST: /StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateLookups(int? selectedGenreId = null, int? selectedArtistId = null)
        {
            ViewData["GenreId"] = new SelectList(
                _context.Genres.OrderBy(g => g.Name),
                nameof(Genre.Id),
                nameof(Genre.Name),
                selectedGenreId
            );

            ViewData["ArtistId"] = new SelectList(
                _context.Artists.OrderBy(a => a.Name),
                nameof(Artist.Id),
                nameof(Artist.Name),
                selectedArtistId
            );
        }

        private async Task<string> SaveFileAsync(IFormFile file, string relativeFolder)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, relativeFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(uploadsRoot);

            var safeFile = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, safeFile);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Devuelve ruta relativa para usar en <img src="/uploads/...">
            return $"/{relativeFolder.Trim('/')}/{safeFile}";
        }
    }
}