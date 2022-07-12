using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ToyTrucks.Orders.Api.Entities;

namespace ToyTrucks.Orders.Api.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public bool OrderPaid { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        public string Message { get; set; }
    }
}