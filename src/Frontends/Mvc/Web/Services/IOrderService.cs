using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersForUser(Guid userId);
        Task<Order> GetOrderDetails(Guid orderId);
    }
}