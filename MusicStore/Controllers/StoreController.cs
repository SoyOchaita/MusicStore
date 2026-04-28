using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly MusicStoreContext _db;
        public StoreController(MusicStoreContext db) => _db = db;

        public async Task<IActionResult> Index(int? brandId, int? typeId, string? productClass, string? search, string? stockOrder)
        {
            ViewBag.Brands = await _db.Artists.OrderBy(brand => brand.Name).ToListAsync();
            ViewBag.Types = await _db.Genres.OrderBy(type => type.Name).ToListAsync();
            ViewBag.Classes = await _db.Albums
                .Select(product => product.ProductClass)
                .Distinct()
                .OrderBy(productClassName => productClassName)
                .ToListAsync();
            var resolvedStockOrder = string.IsNullOrWhiteSpace(stockOrder) ? "stockDesc" : stockOrder;

            ViewBag.SelectedBrandId = brandId;
            ViewBag.SelectedTypeId = typeId;
            ViewBag.SelectedProductClass = productClass;
            ViewBag.Search = search;
            ViewBag.SelectedStockOrder = resolvedStockOrder;

            var query = _db.Albums
                .Include(product => product.Artist)
                .Include(product => product.Genre)
                .AsQueryable();

            if (brandId.HasValue)
                query = query.Where(product => product.ArtistId == brandId.Value);

            if (typeId.HasValue)
                query = query.Where(product => product.GenreId == typeId.Value);

            if (!string.IsNullOrWhiteSpace(productClass))
                query = query.Where(product => product.ProductClass == productClass);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                query = query.Where(product =>
                    product.Code.Contains(searchTerm) ||
                    product.Title.Contains(searchTerm) ||
                    product.Artist!.Name.Contains(searchTerm));
            }

            IQueryable<Album> orderedQuery = resolvedStockOrder switch
            {
                "stockAsc" => query
                    .OrderByDescending(product => product.Stock > 0)
                    .ThenBy(product => product.Stock)
                    .ThenBy(product => product.Title),
                "nameAsc" => query
                    .OrderByDescending(product => product.Stock > 0)
                    .ThenBy(product => product.Artist!.Name)
                    .ThenBy(product => product.Title),
                _ => query
                    .OrderByDescending(product => product.Stock > 0)
                    .ThenByDescending(product => product.Stock)
                    .ThenBy(product => product.Artist!.Name)
                    .ThenBy(product => product.Title)
            };

            var products = await orderedQuery.ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Albums
                .Include(item => item.Genre)
                .Include(item => item.Artist)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }
    }
}