using System;
using System.ComponentModel.DataAnnotations;

namespace ToyTrucks.Orders.Api.Entities
{
    public class OrderItem
    {
        public Guid OrderLineId { get; set; }
        [Required]
        public Guid OrderId { get; set; }

        public string Message { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid TruckId { get; set; }
        public string TruckName { get; set; }
        public string PhotoPath { get; set; }

    }
}