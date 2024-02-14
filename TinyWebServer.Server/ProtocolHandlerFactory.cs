using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.HttpParser.Http11;
using TinyWebServer.Server.ProtocolHandlers.Http11;

namespace TinyWebServer.Server
{
    /// <summary>
    /// This is concrete factory in abstract factory patteren for Protocol
    /// </summary>
    public class ProtocolHandlerFactory : IProtocolHandlerFactory
    {
        public const int HTTP11 = 101;
        public IProtocolHandler Create(int protocolVersion)
        {
            if (protocolVersion == HTTP11)
            {
                return new Http11IProtocolHandler(new RegexHttp11Parsers());
            }
            throw new ArgumentOutOfRangeException(nameof(protocolVersion), "Unknown protocol version");
        }
    }
}
