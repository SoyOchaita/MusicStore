using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        // Datos del cliente y contacto
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PaymentMethod { get; set; }

        // Totales y fecha
        public DateTime OrderDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        // Auditoría
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Navegación
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}