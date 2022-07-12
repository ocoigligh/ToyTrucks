using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using ToyTrucks.Web.Models;
using ToyTrucks.Web.Services;

namespace ToyTrucks.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _environment;
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _config = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(config =>
            {
                config.IdleTimeout = TimeSpan.FromSeconds(20);
                //config.Cookie.HttpOnly=true;
                config.Cookie.IsEssential = true;
            });

            services.AddAccessTokenManagement();
            services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(_config["TruckCatalogUri"]))
            .AddUserAccessTokenHandler();
             
            services.AddHttpClient<IBasketService, BasketService>(c =>
            {
                c.BaseAddress = new Uri(_config["BasketUri"]);
                //c.DefaultRequestHeaders.Add("api-version", "2.0");

            }).AddUserAccessTokenHandler();
            services.AddHttpClient<IOrderService, OrderService>(c =>
            {
                c.BaseAddress = new Uri(_config["OrdersUri"]);
            }).AddUserAccessTokenHandler();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddSingleton<Settings>();
     
            
            var builder = services.AddControllersWithViews();

        

            if (_environment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
         

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=TruckCatalog}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/liveness");
            });
        }
    }
}
