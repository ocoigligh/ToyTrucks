using ToyTrucks.Web.Models.Api;
using System.Collections.Generic;
namespace ToyTrucks.Web.Models.View
{
    public class OrderListViewModel
    {
        public bool HasMessage { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}