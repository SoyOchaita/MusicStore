using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly MusicStoreContext _context;
        private readonly IWebHostEnvironment _env;

        public ArtistsController(MusicStoreContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Artists
        public async Task<IActionResult> Index()
        {
            var list = await _context.Artists.OrderBy(a => a.Name).ToListAsync();
            return View(list);
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create() => View();

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artist artist, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(artist);

            if (imageFile is { Length: > 0 })
            {
                artist.ImageUrl = await SaveFileAsync(imageFile, "uploads/artists");
            }

            _context.Add(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artist artist, IFormFile? imageFile)
        {
            if (id != artist.Id) return NotFound();
            if (!ModelState.IsValid) return View(artist);

            if (imageFile is { Length: > 0 })
            {
                artist.ImageUrl = await SaveFileAsync(imageFile, "uploads/artists");
            }

            try
            {
                _context.Update(artist);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Artists.AnyAsync(a => a.Id == id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFileAsync(IFormFile file, string relativeFolder)
        {
            var root = Path.Combine(_env.WebRootPath, relativeFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(root);

            var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var full = Path.Combine(root, name);

            await using var fs = new FileStream(full, FileMode.Create);
            await file.CopyToAsync(fs);

            return $"/{relativeFolder.Trim('/')}/{name}";
        }
    }
}
