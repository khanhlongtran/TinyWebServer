using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// This is Abstract Factory in abtract factory pattern
    /// </summary>
    public interface IRoutingServiceFactory
    {
        IRoutingService Create(string root);
    }
}
