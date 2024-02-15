using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.Http
{
    // This is real request (using build() method in HttpWebRequestBuilder
    public class HttpRequest : IWebRequest
    {
        public HttpMethod Method { get; }

        public string Url { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public HttpRequest(HttpMethod method, string url, IReadOnlyDictionary<string, string> headers)
        {
            Method = method;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }
        // Trong headers gửi tới cũng sẽ phải xác định cả host thì mới biết được resouce ở đâu 
        // Nên ta đồng thời phải tách được cái host (quan trọng nhất từ header ra để xử lý)
        // Mấy header còn lại bên trong tính sau
        public string Host
        {
            get
            {
                return TryGetHeader("Host");
            }
        }

        // Get HostName từ Headers
        private string TryGetHeader(string hostName)
        {
            if (Headers.TryGetValue(hostName, out var value))
                return value;

            return string.Empty;
        }
    }
}
