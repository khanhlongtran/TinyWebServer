using Microsoft.Extensions.DependencyInjection;
using TinyWebServer.Abstractions;   

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
                Console.WriteLine("Error creating ServerBuilder");
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
        }
    }
}