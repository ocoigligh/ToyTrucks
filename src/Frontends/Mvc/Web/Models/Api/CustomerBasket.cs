using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyTrucks.Web.Models.Api
{
    public class CustomerBasket
    {
        public string UserId { get; set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.Price * x.Quantity), 2);
        }
    }
}