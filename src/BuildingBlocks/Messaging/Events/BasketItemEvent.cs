using System;

namespace ToyTrucks.Messaging.Events
{
    public class BasketItemEvent
    {
        public string Id { get; set; }
        public Guid TruckId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int Year { get; set; }
        public int Quantity { get; set; }
        public string DefaultPhotoPath { get; set; }
    }
}