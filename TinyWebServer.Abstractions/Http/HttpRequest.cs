using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.Http
{
    public class HttpRequest : IWebRequest
    {
        public HttpMethod Method { get; }

        public string Url { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public HttpRequest(HttpMethod method, string url, IReadOnlyDictionary<string, string> headers)
        {
            Method = method;
            Url = url ?? throw new ArgumentNullException(nameof(url))   ;
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));  
        }
    }
}
