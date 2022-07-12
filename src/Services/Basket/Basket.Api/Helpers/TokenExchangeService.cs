using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;

namespace ToyTrucks.Basket.Api.Helpers
{
    public class TokenExchangeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly IConfiguration _config;

        public TokenExchangeService(IHttpClientFactory httpClientFactory, IConfiguration config, IClientAccessTokenCache clientAccessTokenCache)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _clientAccessTokenCache = clientAccessTokenCache;
        }

        public async Task<string> GetTokenAsync(string incomingToken, string apiScope)
        {
            var item = await _clientAccessTokenCache
                .GetAsync($"hesstoytrucks_baskets_to_downstream_tokenexchange_{apiScope}");
            if (item != null)
            {
                return item.AccessToken;
            }

            var client = _httpClientFactory.CreateClient();

            var discoveryDocumentResponse = await client
                .GetDiscoveryDocumentAsync(_config["IdentityServerUrl"]);
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var customParams = new Dictionary<string, string>
            {
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token"},
                { "subject_token", incomingToken},
                { "scope", $"{apiScope}" }
            };

            var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                Parameters = customParams,
                ClientId = "hesstoytrucks_baskets_to_downstream_tokenexchange",
                ClientSecret = "b438b4c0-9963-444d-882f-74a754e667d1"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            await _clientAccessTokenCache.SetAsync(
                $"hesstoytrucks_baskets_to_downstream_tokenexchange_{apiScope}",
                tokenResponse.AccessToken,
                tokenResponse.ExpiresIn);

            return tokenResponse.AccessToken;
        }

    }
}