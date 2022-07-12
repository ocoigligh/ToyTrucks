using Microsoft.Extensions.Logging;
using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Shopping.Aggregator.Extensions;
namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {

        private readonly HttpClient _client;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(HttpClient client, ILogger<CatalogService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            var response = await _client.GetAsync("/api/categories");
            if (response == null)
            {
                _logger.LogError("no response from categoryservice ");
            }
            return await response.ReadContentAs<IEnumerable<CategoryModel>>();

        }
        public async Task<IEnumerable<CategoryModel>> GetCategoriesBySize(bool isMini)
        {
            var response = await _client.GetAsync($"/api/categories/{isMini}");
            if (response == null)
            {
                _logger.LogError("no response from categoryservice");
            }
            return await response.ReadContentAs<IEnumerable<CategoryModel>>();

        }

        public async Task<IEnumerable<TruckModel>> GetTrucks()
        {
            var response = await _client.GetAsync("/api/trucks");
            if (response == null)
            {
                _logger.LogError("no response from categoryservice");
            }
            return await response.ReadContentAs<IEnumerable<TruckModel>>();
        }
        public async Task<IEnumerable<TruckModel>> GetTrucksByCategory(int categoryId)
        {
            var response = await _client.GetAsync($"/api/trucks/{categoryId}");
            if (response == null)
            {
                _logger.LogError("no response from categoryservice");
            }
            return await response.ReadContentAs<IEnumerable<TruckModel>>();
        }
        public async Task<TruckModel> GetTruckById(Guid truckId)
        {
            var response = await _client.GetAsync($"/api/trucks/{truckId}");
            if (response == null)
            {
                _logger.LogError("no response from categoryservice");
            }
            return await response.ReadContentAs<TruckModel>();
        }
    }
}