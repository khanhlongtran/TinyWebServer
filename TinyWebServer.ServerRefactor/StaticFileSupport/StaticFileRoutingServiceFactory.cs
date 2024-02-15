using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;

namespace TinyWebServer.Server.StaticFileSupport
{
    /// <summary>
    /// Concrete Factory
    /// </summary>
    public class StaticFileRoutingServiceFactory : IRoutingServiceFactory
    {
        private readonly ILogger logger;
        public StaticFileRoutingServiceFactory(ILogger logger)
        {
            this.logger = logger;
        }
        public IRoutingService Create(string root)
        {
            // Factory create concrete
            return new StaticFileRoutingService(new DirectoryInfo(root), logger);
        }
    }
}
