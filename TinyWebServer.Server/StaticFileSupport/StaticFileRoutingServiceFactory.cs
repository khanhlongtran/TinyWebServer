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
        public IRoutingService Create(string root)
        {
            // Factory create concrete
            return new StaticFileRoutingService(new DirectoryInfo(root));
        }
    }
}
