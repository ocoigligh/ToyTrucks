using System;

namespace ToyTrucks.Orders.Api.Models
{
    public class OrderLineDto
    {
        public Guid OrderLineId { get; set; }
        public Guid OrderId { get; set; }
        public string Message { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid TruckId { get; set; }
        public string TruckName { get; set; }
        public string PhotoPath { get; set; }
    }
}