using System;
using System.Collections.Generic;

namespace ToyTrucks.Web.Models.Api
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; }
        public int OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public bool OrderPaid { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public string Message { get; set; }
    }
}