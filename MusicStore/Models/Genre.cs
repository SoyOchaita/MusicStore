using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = "";
        public List<Album> Albums { get; set; } = new();
    }
}
