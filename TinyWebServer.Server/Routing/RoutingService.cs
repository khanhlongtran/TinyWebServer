using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;

namespace TinyWebServer.Server.Routing
{
    public class RoutingService : IRoutingService
    {
        private readonly List<IRoutingService> children = new();

        public ICallableResource? FindRoute(string url)
        {
            // Using lock to avoid deadlock
            lock (children)
            {
                foreach (var child in children)
                {
                    var resource = child.FindRoute(url);

                    if (resource != null)
                        return resource;
                }
            }
            return null;
        }
        public void AddRoute(IRoutingService routingService)
        {
            lock (children)
            {
                children.Add(routingService);
            }
        }
        public void AddRoutes(IEnumerable<IRoutingService> routingServices)
        {
            lock (children)
            {
                children.AddRange(routingServices);
            }
        }
    }
}
