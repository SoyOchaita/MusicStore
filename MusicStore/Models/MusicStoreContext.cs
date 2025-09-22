using Microsoft.EntityFrameworkCore;
// Elimina o corrige la siguiente línea según la ubicación real de tus modelos
using MusicStore.Models; // Si tus modelos están en MusicStore.Models

namespace MusicStore.Models
{
    public class MusicStoreContext : DbContext
    {
        // El constructor recibe las opciones (cadena de conexión y proveedor)
        public MusicStoreContext(DbContextOptions<MusicStoreContext> options)
            : base(options) { }

        // DbSets = “tablas” que EF Core rastrea
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        // Opcional: reglas/relaciones adicionales
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}
