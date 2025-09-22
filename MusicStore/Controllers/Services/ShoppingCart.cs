using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Models
{
    public class ShoppingCart
    {
        private readonly MusicStoreContext _db;
        private readonly string _cartId;

        public const string CartSessionKey = "CartId";

        private ShoppingCart(MusicStoreContext db, string cartId)
        {
            _db = db; _cartId = cartId;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            var context = services.GetRequiredService<MusicStoreContext>();
            var http = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext!;
            var cartId = http.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                http.Session.SetString(CartSessionKey, cartId);
            }
            return new ShoppingCart(context, cartId);
        }

        public async Task AddToCartAsync(Album album)
        {
            var item = await _db.CartItems.SingleOrDefaultAsync(c => c.CartId == _cartId && c.AlbumId == album.Id);
            if (item == null)
            {
                item = new CartItem { AlbumId = album.Id, CartId = _cartId, Quantity = 1, DateCreated = DateTime.UtcNow };
                _db.CartItems.Add(item);
            }
            else item.Quantity++;
            await _db.SaveChangesAsync();
        }

        public async Task<int> RemoveFromCartAsync(int albumId)
        {
            var item = await _db.CartItems.SingleAsync(c => c.CartId == _cartId && c.AlbumId == albumId);
            if (item.Quantity > 1) item.Quantity--;
            else _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
            return item.Quantity;
        }

        public Task<List<CartItem>> GetItemsAsync() =>
            _db.CartItems.Include(c => c.Album).ThenInclude(a => a!.Artist)
                         .Include(c => c.Album)!.ThenInclude(a => a!.Genre)
                         .Where(c => c.CartId == _cartId).ToListAsync();

        public Task<int> GetCountAsync() =>
            _db.CartItems.Where(c => c.CartId == _cartId).SumAsync(c => c.Quantity);

        public Task<decimal> GetTotalAsync() =>
            _db.CartItems.Where(c => c.CartId == _cartId)
               .Select(c => c.Quantity * c.Album!.Price).SumAsync();
    }
}
