using Microsoft.Extensions.DependencyInjection;
using TinyWebServer.Abstractions;
using TinyWebServer.Server;
using Microsoft.Extensions.Logging;
namespace TinyWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceBuilder = serviceProvider.GetService<IServerBuilder>();

            if (serviceBuilder == null)
            {
                serviceProvider.GetService<ILogger>()?.LogError("Error creating ServerBuilder");
                return;
            }

            var server = serviceBuilder.UseHttpPort(8080)
                                       .UseStaticFiles()
                                       .Build();

            server.Start();

            Console.WriteLine("Press enter to stop....");
            Console.ReadLine();

            server.Stop();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());
            services.AddTransient<IServerBuilder>(services => new TinyWebServerBuilder(services.GetService<ILogger<TinyWebServerBuilder>>()));
        }
    }
}