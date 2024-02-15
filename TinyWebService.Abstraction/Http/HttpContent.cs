using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebServer.Abstractions.Http
{
    public abstract class HttpContent
    {
        public virtual IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public abstract Task WriteTo(StreamWriter writer);
    }
}
