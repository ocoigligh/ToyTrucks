using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using ToyTrucks.Web.Extensions;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private string _accessToken;

        public OrderService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }
        private async Task<string> GetToken()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            } 
            //var discoDocumentResponse =await _client.GetDiscoveryDocumentAsync(_configuration["IdentityUri"]);// "https://localhost:5010/");
            var discoDocumentResponse =await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{
                Address = _configuration["IdentityUri"],
                Policy = {
                     RequireHttps = false
                 }
            
            });
            if (discoDocumentResponse.IsError)
            {
                throw new Exception(discoDocumentResponse.Error);
            }

            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoDocumentResponse.TokenEndpoint,
                    ClientId = "ordersm2m",
                    ClientSecret = "5db61bcb-77c7-4d9e-bfcb-d02d4fdd58fd",
                    Scope = "orders.fullaccess"
                });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;
        }
        public async Task<IEnumerable<Order>> GetOrdersForUser(Guid userId)
        {
            _client.SetBearerToken(await GetToken());
            System.Console.WriteLine(_accessToken);
            var response = await _client.GetAsync($"api/orders/user/{userId}");
            return await response.ReadContentAs<IEnumerable<Order>>();
        }
        public async Task<Order> GetOrderDetails(Guid orderId)
        {
            _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/orders/{orderId}");
            var order = await response.ReadContentAs<Order>();
            return order;
        }
    }
}