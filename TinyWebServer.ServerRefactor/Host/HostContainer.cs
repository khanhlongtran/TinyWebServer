using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;

namespace TinyWebServer.Server.Host
{
    public class HostContainer
    {
        public HostContainer(string host, DirectoryInfo directory, IRoutingService routingService)
        {
            Host = host;
            Directory = directory;
            RoutingService = routingService;
        }
        public string Host { get; }
        public DirectoryInfo Directory { get; }
        // Find route in this hostContainer (in directory)
        public IRoutingService RoutingService { get; }
    }
}
