using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// Because data is byte[] (stream) when we work with networking. So we a obj the most general way to contain that byte array.
    /// </summary>
    public class ProtocolHandlerData
    {
        public object? Data
        {
            get; set;
        }
    }
}
