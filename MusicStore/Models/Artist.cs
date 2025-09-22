using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        public List<Album> Albums { get; set; } = new();
    }
}
