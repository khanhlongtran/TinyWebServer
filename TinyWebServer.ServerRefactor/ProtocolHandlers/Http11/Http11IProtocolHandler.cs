using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Abstractions.Http;
using TinyWebServer.Abstractions.HttpParser.Http11;
using TinyWebServer.Server.Http;
using static TinyWebServer.Abstractions.ProtocolHandlerStates;

namespace TinyWebServer.Server.ProtocolHandlers.Http11
{
    /// <summary>
    /// This is concrete product in abstract factory pattern for Protocol
    /// </summary>
    public class Http11IProtocolHandler : IProtocolHandler
    {
        private readonly IHttp11Parser http11Parser;
        private readonly ILogger logger;
        public Http11IProtocolHandler(IHttp11Parser http11Parser, ILogger logger)
        {
            this.logger = logger;
            this.http11Parser = http11Parser;
        }
        public int ProtocolVersion => 101;

        public async Task<BuildRequestStates> ReadRequest(TcpClient tcpClient, IHttpRequestBuilder httpRequestBuilder, ProtocolHandlerData data)
        {
            BuildRequestStates state = BuildRequestStates.InProgress;

            data.Data ??= new Http11ProtocolData(
                new StreamReader(tcpClient.GetStream()),
                new StreamWriter(tcpClient.GetStream())
                );

            var d = (Http11ProtocolData)data.Data;

            if (d.CurrentReadingPart == Http11RequestMessageParts.Header) // Read request line and header
            {
                if (d.Reader.Peek() != -1)
                {
                    string? requestLineText = await d.Reader.ReadLineAsync();

                    if (requestLineText != null)
                    {
                        logger.LogInformation("{requestLine}", requestLineText);

                        var requestLine = http11Parser.ParseRequestLine(requestLineText);

                        if (requestLine != null)
                        {
                            var method = GetHttpMethod(requestLine.Method);

                            d.HttpMethod = method;
                            httpRequestBuilder
                                .SetMethod(method)
                                .SetUrl(requestLine.Url);

                            string? headerLineText = d.Reader.ReadLine();
                            while (headerLineText != null && d.CurrentReadingPart == Http11RequestMessageParts.Header)
                            {
                                if (!string.IsNullOrEmpty(headerLineText))
                                {
                                    var headerLine = http11Parser.ParseHeaderLine(headerLineText);

                                    if (headerLine != null)
                                    {
                                        httpRequestBuilder.AddHeader(headerLine.Name, headerLine.Value);
                                    }
                                    else
                                    {
                                        logger.LogError("Invalid header line");
                                        state = BuildRequestStates.Failed;
                                    }

                                    headerLineText = d.Reader.ReadLine();
                                }
                                else
                                {
                                    // an empty line indicates the end of header
                                    d.CurrentReadingPart = Http11RequestMessageParts.Body;
                                }
                            }

                        }
                        else
                        {
                            logger.LogError("Invalid request line");
                            state = BuildRequestStates.Failed;
                        }
                    }
                    else
                    {
                        logger.LogError("Invalid request line");
                        state = BuildRequestStates.Failed;
                    }
                }
            }
            else if (d.CurrentReadingPart == Http11RequestMessageParts.Body)
            {
                // read body parts
                if (d.Reader.Peek() != -1)
                {
                    if (d.HttpMethod == HttpMethod.Get) // GET doesn't have body
                    {
                        logger.LogError("GET cannot have a body");
                        state = BuildRequestStates.Failed;
                    }
                }

                d.CurrentReadingPart = Http11RequestMessageParts.Done;
            }
            else
            {
                state = BuildRequestStates.Succeeded;
            }

            return state;
        }

        public async Task SendResponse(TcpClient tcpClient, IHttpResponseBuilder responseBuilder, ProtocolHandlerData data)
        {
            // Wrong protocol
            // Data is byte[], so we convert to Http11ProtocolData to read and write. But if it not, it wrong
            if (data.Data is not Http11ProtocolData d)
            {
                logger.LogError("Invalid ProtocolHandlerData");
                await SendResponseStatus(new StreamWriter(tcpClient.GetStream()), HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            else
            {
                var response = responseBuilder.Build();
                await SendResponseStatus(d.Writer, response.StatusCode, response.ReasonPhrase);
                foreach (var header in response.Headers)
                {
                    await d.Writer.WriteLineAsync($"{header.Key}: {header.Value}");
                }

                await d.Writer.WriteLineAsync();
                await response.Content.WriteTo(d.Writer);
                await d.Writer.FlushAsync();
            }
        }
        private static async Task SendResponseStatus(StreamWriter writer, HttpStatusCode statusCode, string errorMessage)
        {
            await writer.WriteLineAsync($"HTTP/1.1 {((int)statusCode)} {errorMessage}");
        }
        private static HttpMethod GetHttpMethod(string method)
        {
            if (HttpMethod.Connect.Method == method)
                return HttpMethod.Connect;
            else if (HttpMethod.Delete.Method == method)
                return HttpMethod.Delete;
            else if (HttpMethod.Get.Method == method)
                return HttpMethod.Get;
            else if (HttpMethod.Head.Method == method)
                return HttpMethod.Head;
            else if (HttpMethod.Options.Method == method)
                return HttpMethod.Options;
            else if (HttpMethod.Patch.Method == method)
                return HttpMethod.Patch;
            else if (HttpMethod.Post.Method == method)
                return HttpMethod.Post;
            else if (HttpMethod.Put.Method == method)
                return HttpMethod.Put;
            else if (HttpMethod.Trace.Method == method)
                return HttpMethod.Trace;

            throw new ArgumentException(null, nameof(method));
        }

    }

    public class Http11ProtocolData
    {
        public Http11ProtocolData(StreamReader reader, StreamWriter writer)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }
        public Http11RequestMessageParts CurrentReadingPart { get; set; } = Http11RequestMessageParts.Header;
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
    }

    public enum Http11RequestMessageParts
    {
        Header,
        Body,
        Done
    }

}
