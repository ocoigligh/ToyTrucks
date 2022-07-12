using System;
namespace ToyTrucks.Web.Models.Api
{
    public class TruckInventory
    {
        public Guid TruckId { get; set; }
        public int Quantity { get; set; }
    }
}