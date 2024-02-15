using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Server.Host
{
    /// <summary>
    /// 1 Server có thể có nhiều Host (mỗi host bên trong nó sẽ có các resouce riêng)
    /// Nên ta sẽ lưu trữ dựa trên Name-Value.
    /// https://chat.openai.com/share/fdebe65f-f5f3-4a1c-b55d-641f058dc57e
    /// </summary>
    public class HostConfiguration
    {
        public HostConfiguration(string hostName, string rootDirectory)
        {
            HostName = hostName;
            RootDirectory = rootDirectory;  
        }
        public string HostName { get; }
        // Directory to resource
        public string RootDirectory { get; }
    }
}
