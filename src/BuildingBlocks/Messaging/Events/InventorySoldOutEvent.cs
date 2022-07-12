using System;

namespace ToyTrucks.Messaging.Events
{
    public class InventorySoldOutEvent
    {
        public Guid TruckId { get; set; }

    }
}