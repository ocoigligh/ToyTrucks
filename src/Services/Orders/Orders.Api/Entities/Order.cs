using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToyTrucks.Orders.Api.Entities
{
    public class Order //: EntityBase
    {
        [BsonId]
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public bool OrderPaid { get; set; }
        public string Message { get; set; }
    }
}