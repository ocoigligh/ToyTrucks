using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ToyTrucks.OcelotApi.DelegatingHandlers
{
    public class TokenExchangeDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly IConfiguration _config;

        public TokenExchangeDelegatingHandler(IHttpClientFactory httpClientFactory, IConfiguration config, IClientAccessTokenCache clientAccessTokenCache)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _clientAccessTokenCache = clientAccessTokenCache;
        }

        private async Task<string> GetAccessToken(string incomingToken)
        {
            var item = await _clientAccessTokenCache
                .GetAsync("hesstoytrucks_gateway_to_apis_tokenexchange_catalog");
            if (item != null)
            {
                System.Console.WriteLine($"token is in cache");
                return item.AccessToken;
            }

            var (accessToken, expiresIn) = await ExchangeToken(incomingToken);

            await _clientAccessTokenCache.SetAsync(
                "hesstoytrucks_gateway_to_apis_tokenexchange_catalog",
                accessToken,
                expiresIn);
            System.Console.WriteLine($"Token is new. It will expire in {expiresIn}");
            return accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // extract the current token
            var incomingToken = request.Headers.Authorization.Parameter;

            // set the bearer token
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    await GetAccessToken(incomingToken));

            //// exchange it
            //var newToken = await ExchangeToken(incomingToken);

            //// replace the incoming bearer token with our new one
            //request.Headers.Authorization = 
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<(string, int)> ExchangeToken(string incomingToken)
        {
            var client = _httpClientFactory.CreateClient();

            var discoveryDocumentResponse = await client
                .GetDiscoveryDocumentAsync(_config["IdentityUri"]);
            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var customParams = new Dictionary<string, string>
            {
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token"},
                { "subject_token", incomingToken},
                { "scope", "openid profile catalog.read" }
            };

            var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                Parameters = customParams,
                ClientId = "hesstoytrucks_gateway_to_apis_tokenexchange",
                ClientSecret = "775e5143-2eff-476e-9986-576557877d15"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return (tokenResponse.AccessToken, tokenResponse.ExpiresIn);

        }
    }
}