using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Abstractions.Http;

namespace TinyWebServer.Server.Http
{
    public class HttpWebResponseBuilder : IHttpResponseBuilder
    {
        private HttpStatusCode statusCode = HttpStatusCode.InternalServerError; // default
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private string reasonPhrase = string.Empty;
        private Abstractions.Http.HttpContent content = new Content.StringContent(string.Empty); // body
        public IHttpResponseBuilder AddHeader(string name, string value)
        {
            headers.TryAdd(name, value);

            return this;
        }

        public IHttpResponseBuilder AddHeaders(IEnumerable<KeyValuePair<string, string>> kv)
        {
            foreach (var k in kv)
            {
                headers.TryAdd(k.Key, k.Value);
            }

            return this;
        }

        public HttpResponse Build()
        {
            var response = new HttpResponse(statusCode, reasonPhrase, headers, content);
            return response;
        }

        public IHttpResponseBuilder SetContent(Abstractions.Http.HttpContent content)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
            return this;
        }

        public IHttpResponseBuilder SetReasonPhrase(string messsage)
        {
            this.reasonPhrase = messsage ?? string.Empty;
            return this;
        }

        public IHttpResponseBuilder SetStatusCode(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
            return this;
        }
    }
}
