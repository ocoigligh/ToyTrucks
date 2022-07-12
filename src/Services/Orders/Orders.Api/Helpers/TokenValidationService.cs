using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace ToyTrucks.Orders.Api.Helpers
{
    public class TokenValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;



        public TokenValidationService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<bool> ValidateTokenAsync(string tokenToValidate,
            DateTime receivedAt)
        {
            var client = _httpClientFactory.CreateClient();
            var discoveryDocumentResponse =await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{
            Address = _config["IdentityServerUrl"],
            Policy = {
                    RequireHttps = false
                }
            });

            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            try
            {

                var issuerSigningKeys = new List<SecurityKey>();
                foreach (var webKey in discoveryDocumentResponse.KeySet.Keys)
                {
                    var e = Base64Url.Decode(webKey.E);
                    var n = Base64Url.Decode(webKey.N);

                    var key = new RsaSecurityKey(new RSAParameters
                    { Exponent = e, Modulus = n })
                    {
                        KeyId = webKey.Kid
                    };

                    issuerSigningKeys.Add(key);
                }

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = "orders",
                    ValidIssuer = _config["IdentityServerUrl"],// "https://localhost:5010",
                    
                    IssuerSigningKeys = issuerSigningKeys,
                    LifetimeValidator = (notBefore, expires, securityToken, tokenValidationParameters) =>
                    {
                        return expires.Value.ToUniversalTime() > receivedAt.ToUniversalTime();
                    }
                };

                _ = new JwtSecurityTokenHandler().ValidateToken(tokenToValidate,
                    tokenValidationParameters, out var rawValidatedToken);

                return true;
            }
            catch (SecurityTokenValidationException se)
            {
                System.Console.WriteLine(se);
                // Validation failed - log this if needed
                return false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return false;
            }
        }
    }
}