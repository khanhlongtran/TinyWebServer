using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// 1 WebRequest include some information when a request incoming like Method, Url, Headers
    /// Using interface to more loose coupling
    /// </summary>
    public interface IWebRequest
    {
        HttpMethod Method { get; }
        string Url { get; }
        IReadOnlyDictionary<string, string> Headers { get; }

    }
}