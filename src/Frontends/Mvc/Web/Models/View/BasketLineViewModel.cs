using System;

namespace ToyTrucks.Web.Models.View
{
    public class BasketLineViewModel
    {
        int quantity;
        public string LineId { get; set; }
        public Guid TruckId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Year { get; set; }
        public string DefaultPhotoPath { get; set; }
        public bool OutOfStock { get; set; }
        public bool StockDecreased { get; set; }
        public int TruckQuantity
        {
            get { return quantity; }
            set
            {
                quantity = (value > 5) ? 5 : value;
            }
        }
    }
}
