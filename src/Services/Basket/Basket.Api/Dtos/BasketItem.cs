using System;

namespace ToyTrucks.Basket.Api.Dtos
{
    public class BasketItem
    {
        public string Id { get; set; }
        public Guid TruckId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int Year { get; set; }
        public int Quantity { get; set; }
        public string DefaultPhotoPath { get; set; }
        public bool OutOfStock { get; set; }
        public bool StockDecreased { get; set; }
    }
}