using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MusicStore.Models

{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MusicStoreContext(
                serviceProvider.GetRequiredService<DbContextOptions<MusicStoreContext>>());

            // Si ya hay datos, no insertar de nuevo
            if (context.Genres.Any() || context.Artists.Any() || context.Albums.Any())
                return;

            var genres = new[]
            {
                new Genre { Name = "Rock" },
                new Genre { Name = "Jazz" },
                new Genre { Name = "Disco" },
                new Genre {Name = "Techno"}
            };

            var artists = new[]
            {
                new Artist { Name = "Queen" },
                new Artist { Name = "Miles Davis" },
                new Artist { Name = "Bee Gees" },
                new Artist {Name = "Zapravka"}
            };

            var albums = new[]
            {
                new Album { Title = "A Night at the Opera", Price = 9.99m, Genre = genres[0], Artist = artists[0] },
                new Album { Title = "Kind of Blue", Price = 8.99m, Genre = genres[1], Artist = artists[1] },
                new Album { Title = "Saturday Night Fever", Price = 7.99m, Genre = genres[2], Artist = artists[2] },
                new Album {Title =  "АИ-98", Price = 8.99m, Genre =  genres[3], Artist = artists[3]}
            };

            context.Genres.AddRange(genres);
            context.Artists.AddRange(artists);
            context.Albums.AddRange(albums);
            context.SaveChanges();
        }

    }
}
