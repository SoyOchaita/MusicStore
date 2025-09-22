using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = "";
        [DataType(DataType.Currency)]
       
        public decimal Price { get; set; }

        [Required] public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        [Required] public int ArtistId { get; set; }
        public Artist? Artist { get; set; }
    }
}
