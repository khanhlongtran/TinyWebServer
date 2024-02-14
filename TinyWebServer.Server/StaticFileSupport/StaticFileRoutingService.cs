using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;

namespace TinyWebServer.Server.StaticFileSupport
{
    /// <summary>
    /// Concrete product (implement abstract product)
    /// </summary>
    public class StaticFileRoutingService : IRoutingService
    {
        private readonly DirectoryInfo directoryInfo;
        public StaticFileRoutingService(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
        }

        public ICallableResource? FindRoute(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (directoryInfo.Exists)
            {
                if (url.StartsWith("/"))
                {
                    url = url[1..];
                }
                url = url.Replace('/', Path.DirectorySeparatorChar);

                // Combine directory to this folder and this file in this folder (url)
                var file = new FileInfo(Path.Combine(directoryInfo.FullName, url));
                if (file.Exists)
                {
                    return new StaticFileCallable(file);
                }
            }
            return null;
        }
    }
}
