using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.HttpParser.Http11
{
    /// <summary>
    /// Ta cần parse Request line và header line
    /// Tham khảo: https://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html
    /// </summary>
    public interface IHttp11Parser
    {
        Http11RequestLine? ParseRequestLine(string text);
        Http11HeaderLine? ParseHeaderLine(string text);
    }
}
