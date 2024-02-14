using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    public interface IServerBuilder
    {
        IServerBuilder UseHttpPort(int httpPort);
        IServerBuilder BindToAddress(string ipAddress);
        IServer Build();
    }
}
