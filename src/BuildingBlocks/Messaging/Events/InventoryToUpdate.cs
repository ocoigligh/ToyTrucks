using System;
namespace ToyTrucks.Messaging.Events
{
    public class InventoryToUpdate
    {
        public int Quantity { get; set; }
        public Guid TruckId { get; set; }
        public string TruckName { get; set; }
    }
}