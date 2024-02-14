using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.Http
{
    /// <summary>
    /// Xây dựng một request từ HttpRequestBuilder. Ta chỉ cần .Build
    /// HttpRequestBuilder này sẽ làm việc với connection để lấy ra data từ stream (TcpClient gửi tới)
    /// </summary>
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder SetMethod(HttpMethod method);
        IHttpRequestBuilder SetUrl(string url);
        IHttpRequestBuilder AddHeader(string name, string value);
        IHttpRequestBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        HttpRequest Build();
    }
}
