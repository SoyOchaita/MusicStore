using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStore.Models
{
    public class CartItem
    {
        // La tabla existente usa PK "Id"
        [Column("Id")]
        public int CartItemId { get; set; }

        [Required]
        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;

        // La columna existente en BD se llama "Quantity"
        [Column("Quantity")]
        [Range(1, int.MaxValue)]
        public int Count { get; set; } = 1;

        // Alias de compatibilidad para código existente que usa "Quantity"
        // No se mapea a BD para evitar una columna extra
        [NotMapped]
        public int Quantity
        {
            get => Count;
            set => Count = value;
        }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Required, StringLength(256)]
        public string CartId { get; set; } = string.Empty;
    }
}
