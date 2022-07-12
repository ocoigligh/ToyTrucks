using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ToyTrucks.OcelotApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"ocelot.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true);
                    System.Console.WriteLine($"host: {hostContext.HostingEnvironment.EnvironmentName}");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((host, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(host.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                });

    }
}
