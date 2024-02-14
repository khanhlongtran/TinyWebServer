using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.Http
{
    /// <summary>
    /// https://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html
    /// Tham khảo các cái status code và reason phrase đi kèm với code đó
    /// </summary>
    public interface IHttpResponseBuilder
    {
        IHttpResponseBuilder SetStatusCode(HttpStatusCode statusCode);
        IHttpResponseBuilder AddHeader(string name, string value);
        IHttpResponseBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        IHttpResponseBuilder SetReasonPhrase(string messsage);
        // Set tạm thời vì content khá phức tạp. Ta chỉ phân tích đơn giản một object về response sẽ gồm những gì thui
        IHttpResponseBuilder SetContent(HttpContent content);


    }
}
