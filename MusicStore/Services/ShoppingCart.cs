using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MusicStore;            // MusicStoreContext
using MusicStore.Models;     // Album, CartItem

namespace MusicStore.Services
{
    // Carrito único por usuario/sesión
    public class ShoppingCart
    {
        private readonly MusicStoreContext _db;
        private readonly string _cartId;

        public const string CartSessionKey = "CartId";

        private ShoppingCart(MusicStoreContext db, string cartId)
        {
            _db = db;
            _cartId = cartId;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            var db = services.GetRequiredService<MusicStoreContext>();
            var http = services.GetRequiredService<IHttpContextAccessor>();
            var ctx = http.HttpContext ?? throw new InvalidOperationException("No HttpContext disponible");
            var session = ctx.Session ?? throw new InvalidOperationException("Session no disponible. Registra AddDistributedMemoryCache, AddSession y UseSession.");

            var principal = ctx.User;
            var isAuth = principal?.Identity?.IsAuthenticated == true;

            // Usa SIEMPRE el NameIdentifier (Id único de Identity). Fallback a Name si faltara el claim.
            string? userId = null;
            if (isAuth && principal != null)
            {
                userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    userId = principal.Identity?.Name;
            }

            var current = session.GetString(CartSessionKey);

            if (!string.IsNullOrEmpty(userId))
            {
                // Migra carrito anónimo si existía y es distinto
                if (!string.IsNullOrEmpty(current) && current != userId)
                {
                    MigrateCart(db, current, userId);
                }

                session.SetString(CartSessionKey, userId);
                return new ShoppingCart(db, userId);
            }

            // Invitado: GUID por sesión
            if (string.IsNullOrWhiteSpace(current))
            {
                current = Guid.NewGuid().ToString("N");
                session.SetString(CartSessionKey, current);
            }
            return new ShoppingCart(db, current);
        }

        private static void MigrateCart(MusicStoreContext db, string fromId, string toId)
        {
            var fromItems = db.CartItems.Where(c => c.CartId == fromId).ToList();
            if (fromItems.Count == 0) return;

            var toItems = db.CartItems.Where(c => c.CartId == toId).ToList();

            foreach (var fi in fromItems)
            {
                var existing = toItems.FirstOrDefault(t => t.AlbumId == fi.AlbumId);
                if (existing == null)
                {
                    fi.CartId = toId;
                    db.CartItems.Update(fi);
                }
                else
                {
                    existing.Count += fi.Count;
                    db.CartItems.Remove(fi);
                    db.CartItems.Update(existing);
                }
            }
            db.SaveChanges();
        }

        public void AddToCart(Album album)
        {
            if (album == null) return;

            var item = _db.CartItems.SingleOrDefault(c => c.CartId == _cartId && c.AlbumId == album.Id);
            if (item == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    AlbumId = album.Id,
                    CartId = _cartId,
                    Count = 1,
                    DateCreated = DateTime.UtcNow
                });
            }
            else
            {
                item.Count += 1;
                _db.CartItems.Update(item);
            }
            _db.SaveChanges();
        }

        public void RemoveFromCart(int albumId)
        {
            var item = _db.CartItems.SingleOrDefault(c => c.CartId == _cartId && c.AlbumId == albumId);
            if (item == null) return;

            if (item.Count > 1)
            {
                item.Count -= 1;
                _db.CartItems.Update(item);
            }
            else
            {
                _db.CartItems.Remove(item);
            }
            _db.SaveChanges();
        }

        public void EmptyCart()
        {
            var items = _db.CartItems.Where(c => c.CartId == _cartId);
            _db.CartItems.RemoveRange(items);
            _db.SaveChanges();
        }

        public List<CartItem> GetCartItems()
        {
            return _db.CartItems
                      .Where(c => c.CartId == _cartId)
                      .Include(c => c.Album)
                          .ThenInclude(a => a!.Artist)
                      .ToList();
        }

        public decimal GetTotal()
        {
            return _db.CartItems
                      .Include(c => c.Album)
                      .Where(c => c.CartId == _cartId)
                      .Sum(c => c.Album!.Price * c.Count);
        }

        public Task<List<CartItem>> GetItemsAsync()
        {
            return _db.CartItems
                      .Where(c => c.CartId == _cartId)
                      .Include(c => c.Album)
                          .ThenInclude(a => a!.Artist)
                      .ToListAsync();
        }

        public async Task EmptyCartAsync()
        {
            var items = await _db.CartItems.Where(c => c.CartId == _cartId).ToListAsync();
            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public Task<decimal> GetTotalAsync()
        {
            return _db.CartItems
                      .Include(c => c.Album)
                      .Where(c => c.CartId == _cartId)
                      .SumAsync(c => c.Album!.Price * c.Count);
        }
    }
}
