using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions.Http;
using static TinyWebServer.Abstractions.ProtocolHandlerStates;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// This is abstract product in abstract factory pattern
    /// </summary>
    public interface IProtocolHandler
    {
        /// <summary>
        /// This is abstract product (abstract factory pattern)  for Protocol
        /// </summary>
        int ProtocolVersion
        {
            get;
        }
        Task<BuildRequestStates> ReadRequest(TcpClient tcpClient, IHttpRequestBuilder requestBuilder, ProtocolHandlerData data);
        Task SendResponse(TcpClient tcpClient, IHttpResponseBuilder responseBuilder, ProtocolHandlerData data);
    }
}
