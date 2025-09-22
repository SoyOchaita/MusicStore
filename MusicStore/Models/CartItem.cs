using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string CartId { get; set; } = ""; // identifica la “bolsa” del usuario (sesión/cookie)
        public int AlbumId { get; set; }
        public Album? Album { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
