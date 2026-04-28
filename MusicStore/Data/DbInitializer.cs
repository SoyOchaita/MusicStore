using MusicStore.Models;

namespace MusicStore.Data
{
    public static class DbInitializer
    {
        public static void Seed(MusicStoreContext context)
        {
            if (context.Genres.Any()) return; // evita duplicados

            var genres = new[]
            {
                new Genre { Name = "Rock" },
                new Genre { Name = "Pop" },
                new Genre { Name = "Jazz" },
                new Genre { Name = "Clásico" }
            };
            context.Genres.AddRange(genres);

            var artists = new[]
            {
                new Artist { Name = "Queen" },
                new Artist { Name = "The Beatles" },
                new Artist { Name = "Luis Miguel" },
                new Artist { Name = "Beethoven" }
            };
            context.Artists.AddRange(artists);

            var albums = new[]
            {
                new Album { Title = "A Night at the Opera", Price = 120.00M, Genre = genres[0], Artist = artists[0] },
                new Album { Title = "Abbey Road", Price = 110.00M, Genre = genres[0], Artist = artists[1] },
                new Album { Title = "Romances", Price = 90.00M, Genre = genres[1], Artist = artists[2] },
                new Album { Title = "Sinfonía No. 5", Price = 80.00M, Genre = genres[3], Artist = artists[3] }
            };
            context.Albums.AddRange(albums);

            context.SaveChanges();
        }
    }
}
