using System;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ToyTrucks.OcelotApi.DelegatingHandlers;

namespace ToyTrucks.OcelotApi
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            
            
            services.AddAccessTokenManagement();
            // sub => http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authenticationScheme = "GloboTicketGatewayAuthenticationScheme";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(authenticationScheme, options =>
                {
                    options.Authority = _config["IdentityUri"];
                    options.Audience = "hesstoysgateway";
                });

            services.AddScoped<TokenExchangeDelegatingHandler>();

            services.AddOcelot()
            .AddDelegatingHandler<TokenExchangeDelegatingHandler>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            await app.UseOcelot();
        }
    }
}
