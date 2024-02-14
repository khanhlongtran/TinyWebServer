using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions.Http;

namespace TinyWebServer.Server.Http.Content
{
    /// <summary>
    /// This class is used to write data into response (ensure Single Responsibility)
    /// </summary>
    public class StringContent : Abstractions.Http.HttpContent
    {
        private readonly string content;
        public StringContent(String content)
        {
            this.content = content;
        }

        public override async Task WriteTo(StreamWriter writer)
        {
            await writer.WriteAsync(content);
        }
    }
}
