using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Server.Host;
using TinyWebServer.Server.StaticFileSupport;
using System.Reflection;
using TinyWebServer.Server.Routing;
using Microsoft.Extensions.Logging;

namespace TinyWebServer.Server
{
    /// <summary>
    /// Implement concrete builder after interface builder (IServerBuilder)
    /// </summary>
    public class TinyWebServerBuilder : IServerBuilder
    {
        // A socket to connect server need IP:Port
        private IPAddress address = IPAddress.Loopback;
        private int httpPort = 80;
        // A server can have many hosts. We will find each Host base name
        private readonly Dictionary<string, HostConfiguration> hosts = new();
        // Find route in hostContainer 
        private readonly List<IRoutingServiceFactory> routingServiceFactories = new();
        private readonly ILogger<TinyWebServerBuilder> logger;
        public TinyWebServerBuilder(ILogger<TinyWebServerBuilder>? logger)
        {
            this.logger = logger;
        }
        public IServerBuilder AddHost(string hostName, string hostDirectory)
        {
            hosts[hostName] = new HostConfiguration(hostName, hostDirectory);
            return this;
        }

        public IServerBuilder AddRoot(string rootDirectory)
        {
            return AddHost(string.Empty, rootDirectory);
        }

        public IServerBuilder BindToAddress(string ipAddress)
        {
            // return false if cannot parse
            if (!IPAddress.TryParse(ipAddress, out IPAddress? ip))
            {
                throw new ArgumentException(nameof(ipAddress));
            }
            else
            {
                this.address = ip;
                return this;
            }
        }

        public IServer Build()
        {
            // Empty host => setup default host to folder wwwroot
            if (!hosts.Any())
            {
                hosts.Add(string.Empty, new HostConfiguration(string.Empty, Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName ?? string.Empty, "wwwroot")));
            }

            Dictionary<string, HostContainer> hostContainers = new();
            // loop in HostConfiguration
            foreach (var host in hosts.Values)
            {
                var routingService = new RoutingService();

                // Now have only Factory is StaticFileRoutingServiceFactory
                // We know the rootDirectory, so we will create Route to track it
                // Behind it, we new DirectoryInfo(rootDirectory)
                // Each host have each own Directory 
                foreach (var factory in routingServiceFactories)
                {
                    routingService.AddRoute(factory.Create(host.RootDirectory));
                }
                // When create Dictionary to hold list host, We also create HostContainer for that host (store it in the dictionary).
                hostContainers.Add(host.HostName, new HostContainer(host.HostName, new DirectoryInfo(host.RootDirectory), routingService));
            }

            var server = new TinyWebServer(new TinyWebServerConfiguration()
            {
                HttpEndPoint = new(address, httpPort),
                Hosts = hosts.Values.ToList(),
            },
            new ProtocolHandlerFactory(logger),
            hostContainers,
            logger);
            return server;
        }

        public IServerBuilder UseHttpPort(int httpPort)
        {
            if (httpPort < 0 || httpPort > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(httpPort));
            }
            this.httpPort = httpPort;
            return this;
        }

        public IServerBuilder UseStaticFiles()
        {
            return AddRoutingServiceFactory(new StaticFileRoutingServiceFactory(logger));
        }
        public IServerBuilder AddRoutingServiceFactory(IRoutingServiceFactory routingServiceFactory)
        {
            routingServiceFactories.Add(routingServiceFactory);

            return this;
        }
    }
}
