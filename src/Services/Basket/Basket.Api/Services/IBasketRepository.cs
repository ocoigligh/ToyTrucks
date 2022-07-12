using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyTrucks.Basket.Api.Dtos;

namespace ToyTrucks.Basket.Api.Services
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string userId);
        IEnumerable<string> GetUsers();
        Task<IEnumerable<CustomerBasket>> GetBasketsWithTruck(Guid truckId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string id);
    }
}