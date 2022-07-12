using System;

namespace ToyTrucks.Messaging.Events
{
    public class UpdatedInventory
    {
        public Guid TruckId { get; set; }
        public string TruckName { get; set; }
        public int NewQuantity { get; set; }
    }
}