using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // Relaciones
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int AlbumId { get; set; }
        public Album? Album { get; set; }

        // Datos de línea
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
    }
}