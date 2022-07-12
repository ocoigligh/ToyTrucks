using System;
using System.Collections.Generic;

namespace ToyTrucks.Web.Models.View
{
    public class BasketViewModel
    {
        public Guid BasketId { get; set; }
        public List<BasketLineViewModel> BasketLines { get; set; }
        public decimal ShoppingCartTotal { get; set; }
        public int Discount { get; set; }
    }
}
