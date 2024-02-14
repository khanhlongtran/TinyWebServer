using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Server.Host
{
    public class HostContainer
    {
        public HostContainer(string host, DirectoryInfo directory)
        {
            Host = host;
            Directory = directory;
        }
        public string Host { get; }
        public DirectoryInfo Directory { get; }
    }
}
