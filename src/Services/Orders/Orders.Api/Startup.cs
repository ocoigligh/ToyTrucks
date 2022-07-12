using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ToyTrucks.Orders.Api.DBContexts;
using ToyTrucks.Orders.Api.Services;
using MassTransit;
using ToyTrucks.Messaging.Common;
using ToyTrucks.Orders.Api.Entities;
using MongoDB;
using ToyTrucks.Orders.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace ToyTrucks.Orders.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient();
            services.AddScoped<TokenValidationService>();
            var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            var builder = services.AddControllers(options =>
            {
                 options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "orders";
                    options.Authority = Configuration["IdentityServerUrl"];
                    options.RequireHttpsMetadata = false;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders", Version = "v1" });
            });

            //    services.AddDbContext(Configuration);
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IOrderContext, OrderContext>();
            services.AddScoped<BasketCheckoutConsumer>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHealthChecks();

            services.AddClientMassTransit(Configuration);

            services.AddApiVersioning(options =>
          {
              options.AssumeDefaultVersionWhenUnspecified = true;
              options.DefaultApiVersion = new ApiVersion(1, 0);
              options.ReportApiVersions = true;
              options.ApiVersionReader = new HeaderApiVersionReader("api-version");
          });

            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/liveness");
            });
        }
    }

    public static class ServiceCollectionExtensionMethods
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            // services.AddMongoDb(); 
            return services;
        }

        public static IServiceCollection AddClientMassTransit(this IServiceCollection services, IConfiguration config)
        {
            var queueSettingsSection = new QueueSettings();
            config.Bind("RabbitMQ:QueueSettings", queueSettingsSection);

            services.AddMassTransit(configuration =>
            {
                configuration.AddConsumer<BasketCheckoutConsumer>();
                configuration.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(queueSettingsSection.HostName, queueSettingsSection.VirtualHost,
                   cg =>
                   {
                       cg.Username(queueSettingsSection.UserName);
                       cg.Password(queueSettingsSection.Password);
                   });
                    cfg.ReceiveEndpoint(config["BasketCheckoutQueue"], c =>
                   {
                       c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                   });
                });
            });
            services.AddMassTransitHostedService();

            return services;
        }

    }
}
