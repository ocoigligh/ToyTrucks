using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyTrucks.Orders.Api.Entities;

namespace ToyTrucks.Orders.Api.Services
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserId(Guid userId);
        Task<IReadOnlyList<Order>> GetOrdersAsync();
        Task<Order> GetByOrderIdAsync(Guid orderId);
        Task<Order> AddOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(Guid orderId);
    }
}