using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore
{
    public class MusicStoreContext : DbContext
    {
        public MusicStoreContext(DbContextOptions<MusicStoreContext> options) : base(options) { }

        // Si necesitas IHttpContextAccessor, puedes añadir el otro ctor opcionalmente:
        // private readonly IHttpContextAccessor? _httpContextAccessor;
        // public MusicStoreContext(DbContextOptions<MusicStoreContext> options, IHttpContextAccessor accessor) : base(options) { _httpContextAccessor = accessor; }

        public DbSet<Album> Albums { get; set; } = default!;
        public DbSet<Artist> Artists { get; set; } = default!;
        public DbSet<Genre> Genres { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Album>()
                        .HasIndex(a => a.Code)
                        .IsUnique();

            modelBuilder.Entity<Album>()
                        .HasOne(a => a.Genre)
                        .WithMany(g => g.Albums)
                        .HasForeignKey(a => a.GenreId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Album>()
                        .HasOne(a => a.Artist)
                        .WithMany(ar => ar.Albums)
                        .HasForeignKey(a => a.ArtistId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>(e =>
            {
                e.Property(p => p.CartItemId).HasColumnName("Id");
                e.Property(p => p.Count).HasColumnName("Quantity");
                e.HasIndex(p => new { p.CartId, p.AlbumId });
            });
        }
    }
}
