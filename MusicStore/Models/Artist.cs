using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        // Imagen (URL externa o ruta relativa a wwwroot, p.ej. /uploads/artists/xxx.jpg)
        [StringLength(512)]
        public string? ImageUrl { get; set; }

        public List<Album> Albums { get; set; } = new();
    }
}
