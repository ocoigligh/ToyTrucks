using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Shopping.Aggregator.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CategoryModel>> GetCategories();
        Task<IEnumerable<CategoryModel>> GetCategoriesBySize(bool isMini);

        Task<IEnumerable<TruckModel>> GetTrucks();
        Task<IEnumerable<TruckModel>> GetTrucksByCategory(int categoryId);
        Task<TruckModel> GetTruckById(Guid truckId);
    }
}