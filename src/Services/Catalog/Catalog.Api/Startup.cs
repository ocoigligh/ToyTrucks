using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToyTrucks.Catalog.Api.DbContexts;
using ToyTrucks.Catalog.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using MassTransit;
using ToyTrucks.Catalog.Api.Events;
using ToyTrucks.Messaging.Common;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToyTrucks.Catalog.Api
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

            services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventDto Catalog API", Version = "v1" });
       });

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddClientMassTransit(Configuration);

            services.AddPostgresDbContext(Configuration);
            services.AddScoped<ITruckRepository, TruckRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddApiVersioning(options =>
          {
              options.AssumeDefaultVersionWhenUnspecified = true;
              options.DefaultApiVersion = new ApiVersion(1, 0);
              options.ReportApiVersions = true;
              options.ApiVersionReader = new HeaderApiVersionReader("api-version");
          });

            services.AddHealthChecks();

            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

            services.AddControllers(configure =>
            {
                 configure.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority =  Configuration["IdentityServerUrl"];
                options.Audience = "catalog";
                options.RequireHttpsMetadata = false;
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.Api v1"));
            }

            //app.UseHttpsRedirection();

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
        public static IServiceCollection AddPostgresDbContext(this IServiceCollection services, IConfiguration config)
        {

            //Create  postgres container
            //docker run -e POSTGRES_DB=catalogdb -e POSTGRES_USER=marcas -e POSTGRES_PASSWORD=password -p 5432:5432 --name postgres_catalog -d postgres
            //run bash to access postgres container
            //docker exec -it 480c32e2bb53 "bash" //where 480c3.. is the container id

            //psql -h localhost -p 5432 -U marcas -d catalogdb
            // \l lists dbs 
            // \c <db> connects to db 
            // \d lists db objects
            var server = config["POSTGRES_SERVER"];// ?? "localhost";

            var port = config["POSTGRES_PORT"];// ?? "5432";
            var database = config["POSTGRES_DB"];// ?? "catalogdb";


            var user = config["POSTGRES_USER"];// ?? "marcas";
            var password = config["POSTGRES_PASSWORD"];// ?? "password";

            var connectionString =   config["DBConnectionString"];// $"Host={server}; Port={port}; Database={database}; Username={user}; Password={password};";
            Console.WriteLine($"CONNECTION STRING Catalog: {connectionString}");

            //"User ID =postgres;Password=password;Server=localhost;Port=5432;Database=testDb;Integrated Security=true;Pooling=true;" //alternative
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseNpgsql(connectionString)
                       .UseSnakeCaseNamingConvention()
                    );

            return services;
        }

        public static IServiceCollection AddClientMassTransit(this IServiceCollection services, IConfiguration config)
        {
            var queueSettingsSection = new QueueSettings();
            config.Bind("RabbitMQ:QueueSettings", queueSettingsSection);

            services.AddMassTransit(configuration =>
            {
                configuration.AddConsumer<InventorySoldOutConsumer>();
                configuration.AddConsumer<InventoryToUpdateConsumer>();
                configuration.UsingRabbitMq((ctx, cfg) =>
                {
                    // cfg.Host(config["EventBusAddress"]);
                    cfg.Host(queueSettingsSection.HostName, queueSettingsSection.VirtualHost,
                 //cfg.Host("localhost", queueSettingsSection.VirtualHost,
                 cg =>
                 {
                     cg.Username(queueSettingsSection.UserName);
                     cg.Password(queueSettingsSection.Password);
                 });
                    // cfg.ExchangeType = ExchangeType.Direct;
                    cfg.ReceiveEndpoint(config["OutOfStockQueue"], c =>
                    {
                        c.ConfigureConsumer<InventorySoldOutConsumer>(ctx);
                    });
                    cfg.ReceiveEndpoint(config["InventoryUpdatedQueue"], c =>
                    {
                        c.ConfigureConsumer<InventoryToUpdateConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();

            return services;

        }
    }
}
