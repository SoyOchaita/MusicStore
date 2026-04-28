using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = "";

        [Required, StringLength(20)]
        public string Code { get; set; } = "";

        [Required, StringLength(50)]
        public string ProductClass { get; set; } = "";

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        [Required]
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        // URL relativa a wwwroot (por ejemplo: /img/albums/album-1.jpg)
        [StringLength(512)]
        public string? AlbumArtUrl { get; set; }

        // Auditoría
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
