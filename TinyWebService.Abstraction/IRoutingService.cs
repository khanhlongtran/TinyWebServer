using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// This is abstract product in abstract factory pattern.
    /// </summary>
    public interface IRoutingService
    {
        ICallableResource? FindRoute(string url);
    }
}
