using System.Collections.Generic;

namespace ToyTrucks.Basket.Api.Dtos
{
    public class CustomerBasket
    {
        public string UserId { get; set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public CustomerBasket()
        {

        }

        public CustomerBasket(string customerId)
        {
            UserId = customerId;
        }
    }
}