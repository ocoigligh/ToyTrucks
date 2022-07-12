using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ToyTrucks.Web.Extensions;
using ToyTrucks.Web.Models;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Settings _settings;
        private readonly ILogger<BasketService> _logger;
        private readonly IConfiguration _configuration;
        private string _accessToken;

        public BasketService(HttpClient client, Settings settings, IHttpContextAccessor httpContextAccessor, ILogger<BasketService> logger, IConfiguration configuration)
        {
            _client = client;
            _settings = settings;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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
                    ClientId = "hesstoytrucks",
                    ClientSecret = "3322cccf-b6ff-4558-aefb-6c159cd566a0",
                    Scope = "basket.fullaccess hesstoysgateway.fullaccess"
                });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;
        }

        public async Task<CustomerBasket> GetBasket(string basketId)
        {
            if (string.IsNullOrWhiteSpace(basketId))
            {
                basketId = CreateBasketCookie();
            }
               _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/v1/basket/{basketId}");

            return await response.ReadContentAs<CustomerBasket>();
        }

        public async Task<CustomerBasket> AddLine(string basketId, BasketItem basketItem)
        {
            if (string.IsNullOrWhiteSpace(basketId))
            {
                basketId = CreateBasketCookie();
            }
            if (basketItem == null)
            {
                throw new ArgumentNullException(nameof(basketItem));
            }

            var customerBasket = await GetBasket(basketId);
            if (customerBasket == null)
            {
                throw new ArgumentNullException($"customer basket does not exist for basketid of{basketId}");
            }
            if (customerBasket.Items.Any(bl => bl.TruckId == basketItem.TruckId))
            {
                customerBasket.Items.First(bl => bl.TruckId == basketItem.TruckId).Quantity += basketItem.Quantity;
            }
            else
            {
                basketItem.Id = Guid.NewGuid().ToString();
                customerBasket.Items.Add(basketItem);
            }
            _client.SetBearerToken(await GetToken());
            var response = await _client.PostAsJson($"api/v1/basket", customerBasket);
            return await response.ReadContentAs<CustomerBasket>();
        }

        public async Task DeleteBasket(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
             _client.SetBearerToken(await GetToken());
            await _client.DeleteAsync($"api/v1/basket/{userId}");
        }

        public async Task<BasketForCheckout> Checkout(string basketId, BasketForCheckout basketForCheckout)
        {
            if (string.IsNullOrWhiteSpace(basketId))
            {
                throw new ArgumentNullException(nameof(basketId));
            }
            if (basketForCheckout == null)
            {
                throw new ArgumentNullException(nameof(basketForCheckout));
            }
             _client.SetBearerToken(await GetToken());
            var response = await _client.PostAsJson($"api/v1/basket/checkout", basketForCheckout);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<BasketForCheckout>();
            else
            {
                throw new Exception("Something went wrong placing your order. Please try again.");
            }

        }

        private string CreateBasketCookie()
        {
            var basketId = Guid.NewGuid();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(21);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                _settings.BasketIdCookieName, basketId.ToString(), cookieOptions);
            return basketId.ToString();
        }

        public async Task<CustomerBasket> RemoveLine(string basketId, string lineId)
        {
            CustomerBasket basket = await GetBasket(basketId);
            var basketItem = basket?.Items?.FirstOrDefault(bi => bi.Id == lineId);
            if (basketItem != null)
            {
                basket.Items.Remove(basketItem);
                var response = await _client.PostAsJson($"api/v1/basket", basket);
                return await response.ReadContentAs<CustomerBasket>();
            }
            else
            {
                _logger.LogWarning($"No basket item found with id of {lineId}");
                return basket;
            }
        }

        public async Task UpdateLine(string basketId, BasketLineForUpdate basketLineForUpdate)
        {
            CustomerBasket basket = await GetBasket(basketId);
            var basketItem = basket?.Items?.FirstOrDefault(bi => bi.Id == basketLineForUpdate.LineId);
            if (basketItem != null)
            {
                basketItem.Quantity = basketLineForUpdate.Quantity;
                var response = await _client.PostAsJson($"api/v1/basket", basket);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"{response.ReasonPhrase}");
                }
            }
            else
            {
                _logger.LogWarning($"No basket item found with id of {basketLineForUpdate.LineId}");
            }
        }

        public async Task<bool> HasLineItems(string basketId)
        {
            if (string.IsNullOrWhiteSpace(basketId))
            {
                basketId = CreateBasketCookie();
            }
             _client.SetBearerToken(await GetToken());
            var response = await _client.GetAsync($"api/v1/basket/{basketId}");

            var basket = await response.ReadContentAs<CustomerBasket>();

            return basket.Items.Any();
        }
    }
}