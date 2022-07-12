using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ToyTrucks.Web.Models.Api;

using ToyTrucks.Web.Extensions;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Http;
 
namespace ToyTrucks.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _accessToken;
        public CatalogService(HttpClient client, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetToken()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            } 
            var discoDocumentResponse =await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{
                Address = _configuration["IdentityUri"],
                Policy = {
                     RequireHttps = false
                 }
            
            }); // "https://localhost:5010/");
            if (discoDocumentResponse.IsError)
            {
                throw new Exception(discoDocumentResponse.Error);
            }

            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoDocumentResponse.TokenEndpoint,
                    ClientId = "catalogsm2m", //"hesstoytrucks",
                    ClientSecret = "dba781a5-6eae-4d63-b4e0-dd672083fd9c",// "3322cccf-b6ff-4558-aefb-6c159cd566a0",
                    Scope = "catalog.read"
                });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;
        }

        public async Task<IEnumerable<Truck>> GetTrucksByCategoryId(int categoryId)
        {
            _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/trucks/{categoryId}");
            var trucks = await response.ReadContentAs<List<Truck>>();
            var orderedTrucks = trucks.OrderBy(t => t.Year);
            return orderedTrucks;
        }
        public async Task<Truck> GetTruckById(Guid truckId)
        {
            _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/trucks/{truckId}");
            return await response.ReadContentAs<Truck>();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync("api/categories");
            return await response.ReadContentAs<List<Category>>();
        }

        public async Task<IEnumerable<Truck>> GetTrucks()
        {
            _client.SetBearerToken(await GetToken());
            System.Console.WriteLine(_accessToken);
            var response = await _client.GetAsync("api/trucks");
            var trucks = await response.ReadContentAs<List<Truck>>();
            var orderedTrucks = trucks.OrderBy(t => t.Year);
            return orderedTrucks;
        }

        public async Task<TruckInventory> GetTruckInventory(Guid truckId)
        {
            _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/trucks/inventory/{truckId}");
            return await response.ReadContentAs<TruckInventory>();
        }
    }
}