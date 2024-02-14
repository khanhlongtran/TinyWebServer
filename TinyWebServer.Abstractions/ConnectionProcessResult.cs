using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    public class ConnectionProcessResult
    {
        public bool Success
        {
            get;
        }
        public bool CloseConnectionRequested
        {
            get;
        }
        // Maybe have many version protocol like 1.1/2/3
        public int ProtocolVersionRequested
        {
            get;
        }
    }
}
