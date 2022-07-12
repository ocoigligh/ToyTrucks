using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using ToyTrucks.Orders.Api.DBContexts;
using ToyTrucks.Orders.Api.Entities;
namespace ToyTrucks.Orders.Api.Services
{
    public class OrdersRepository : IOrdersRepository
    {

        private readonly IOrderContext _context;

        public OrdersRepository(IOrderContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        private async Task<bool> OrderExists(Guid orderId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(p => p.OrderId, orderId);
            return await _context.Orders
                                 .Find(filter)
                                 .AnyAsync();
        }

        public async Task<Order> GetByOrderIdAsync(Guid orderId)
        {

            if (!await OrderExists(orderId))
                throw new Exception($"Order doesnt exist with id of {orderId}");

            //   FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(p => p.Id, id);
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(p => p.OrderId, orderId);

            return await _context.Orders
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                                 .Find(_ => true)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(Guid userId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(p => p.UserId, userId);
            return await _context.Orders
                                 .Find(filter)
                                 .ToListAsync();
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            await _context.Orders.InsertOneAsync(order);
            return await Task.FromResult(order);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (!await OrderExists(order.OrderId))
                throw new Exception($"Order doesnt exist with id of {order.OrderId}");

            var updateResult = await _context.Orders
                                             .ReplaceOneAsync(filter: g => g.OrderId == order.OrderId, replacement: order);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;

            // _context.Entry(order).State = EntityState.Modified;
            // await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {


            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(p => p.OrderId, orderId);

            DeleteResult deleteResult = await _context.Orders
                                                      .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}