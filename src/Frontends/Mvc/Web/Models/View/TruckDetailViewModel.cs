using System;
using System.Collections.Generic;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Models.View
{
    public class TruckDetailViewModel
    {
        int quantity;
        public Truck Truck { get; set; }

        public int Quantity
        {

            get
            {
                if (Truck.Quantity > 5)
                {
                    quantity = 5;
                }
                else
                {
                    quantity = Truck.Quantity;
                }
                return quantity;
            }
        }



    }
}