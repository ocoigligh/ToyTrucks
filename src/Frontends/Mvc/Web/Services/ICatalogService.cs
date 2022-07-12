using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Truck>> GetTrucksByCategoryId(int categoryId);
        Task<IEnumerable<Truck>> GetTrucks();
        Task<Truck> GetTruckById(Guid truckId);
        Task<IEnumerable<Category>> GetCategories();
        Task<TruckInventory> GetTruckInventory(Guid truckId);

    }
}