using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// This is abstract factory in abstract factory pattern  for Protocol
    /// </summary>
    public interface IProtocolHandlerFactory
    {
        IProtocolHandler Create(int protocolVersion);
    }
}
