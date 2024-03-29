﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Abstractions.Http;
using TinyWebServer.Server.Http.Helpers;

namespace TinyWebServer.Server.StaticFileSupport
{

    // Build response builder object if url exists. If not, response is not found state
    public class StaticFileCallable : ICallableResource
    {
        private readonly FileInfo file;
        private readonly ILogger logger;
        public StaticFileCallable(FileInfo file, ILogger logger)
        {
            this.file = file;
            this.logger = logger;
        }
        public async Task OnGet(HttpRequest request, IHttpResponseBuilder responseObjectBuilder)
        {
            if (file.Exists)
            {
                try
                {
                    // Create stream reader to read file
                    using (var reader = file.OpenText())
                    {
                        var contentFile = await reader.ReadToEndAsync();

                        responseObjectBuilder.AddHeader("Content-type", "text/html");
                        responseObjectBuilder.AddHeader("Content-length", contentFile.Length.ToString());
                        responseObjectBuilder.SetContent(new Http.Content.StringContent(contentFile));

                        StandardResponseBuilderHelpers.Ok(responseObjectBuilder);

                    }
                } catch (Exception ex)
                {
                    logger.LogError(ex, message: null);
                }

            }
            else
            {
                StandardResponseBuilderHelpers.NotFound(responseObjectBuilder);
            }
        }
    }
}
