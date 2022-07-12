using ToyTrucks.Orders.Api.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
namespace ToyTrucks.Orders.Api
{
    public class OrderSeedData
    {


        public static void SeedData(IMongoCollection<OrderDto> ordersCollection)
        {
            bool orders = ordersCollection.Find(p => true).Any();
        }

        static IEnumerable<OrderDto> PopulateOrders()
        {
            return null;

        }
    }
}

