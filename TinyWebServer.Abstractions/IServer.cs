using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    public interface IServer : IDisposable
    {
        void Start();
        void Stop();
    }
}
