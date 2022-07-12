using MongoDB.Driver;
using ToyTrucks.Orders.Api.Entities;
namespace ToyTrucks.Orders.Api.DBContexts
{
    public interface IOrderContext
    {
        IMongoCollection<Order> Orders { get; }
    }
}