using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Abstractions.Http;
using TinyWebServer.Server.Host;
using TinyWebServer.Server.Http.Helpers;

namespace TinyWebServer.Server
{

    public class TinyWebServer : IServer
    {
        /// <summary>
        /// Config to create connection
        /// IProtocolHandlerFactory to handle data income
        /// HostContainer contain resource of it's host
        /// </summary>
        private readonly TinyWebServerConfiguration config;
        private readonly IProtocolHandlerFactory protocolHandlerFactory;
        private readonly Dictionary<string, HostContainer> hostContainers;
        // Listen from server >< TcpClient
        private TcpListener? server;
        private bool running;
        private int nextClientId = 1;
        private readonly ConcurrentQueue<TinyWebClientConnection> waitingClients = new();
        private static EventWaitHandle clientConnectionWaitHandle = new(false, EventResetMode.AutoReset);
        private bool disposed = false;
        private CancellationTokenSource acceptCancellationTokenSource = new();
        private CancellationToken acceptCancellationToken;
        private readonly ILogger logger;

        public TinyWebServer(TinyWebServerConfiguration config, IProtocolHandlerFactory protocolHandlerFactory, Dictionary<string, HostContainer> hostContainers, ILogger logger)
        {
            this.config = config;
            this.protocolHandlerFactory = protocolHandlerFactory;
            this.hostContainers = hostContainers;
            running = false;
            this.logger = logger;
        }


        public void Start()
        {
            logger.LogInformation("Starting web server...");
            running = true;

            acceptCancellationToken = acceptCancellationTokenSource.Token;

            // Create Thread to listen in server
            new Thread(ClientConnectionListeningProc) { IsBackground = false }.Start();


            for (int i = 1; i <= config.ThreadPoolSize; i++)
            {
                var thread = new Thread(ClientConnectionProcessingProc) { IsBackground = false };
                thread.Start(i);
            }
        }

        private async void ClientConnectionProcessingProc(object? data)
        {
            int n = (int)data; // convert number of thread
            logger.LogInformation("Starting ThreadPool.Thread {n}", n);
            while (running)
            {
                logger.LogDebug("ThreadPool.Thread {n} processing...", n);
                // Set() be called. This wil be process
                clientConnectionWaitHandle.WaitOne(TimeSpan.FromSeconds(3));
                if (running)
                {
                    if (waitingClients.TryDequeue(out var client))
                    {
                        var newClient = await ProcessClientConnection(client);
                        if (newClient != null)
                        {
                            waitingClients.Enqueue(newClient);
                            clientConnectionWaitHandle.Set(); //Nếu còn thì xử lý tiếp
                        }
                    }

                }
            }
            logger.LogInformation("ThreadPool.Thread {n} stopped", n);
        }

        private async Task<TinyWebClientConnection> ProcessClientConnection(TinyWebClientConnection client)
        {
            logger.LogDebug("Processing client connection: {client}", client.Id);
            try
            {
                if (client.State == TinyWebClientConnection.States.Pending || client.State == TinyWebClientConnection.States.BuildingRequestObject)
                {
                    // Http11IProtocolHandler
                    var state = await client.ProtocolHandler.ReadRequest(client.TcpClient, client.RequestObjectBuilder, client.ProtocolHandlerData);
                    if (state == ProtocolHandlerStates.BuildRequestStates.Failed)
                    {
                        StandardResponseBuilderHelpers.BadRequest(client.ResponseObjectBuilder);
                        client.State = TinyWebClientConnection.States.RequestObjectReady;
                    }
                    else if (state == ProtocolHandlerStates.BuildRequestStates.Succeeded)
                    {
                        client.State = TinyWebClientConnection.States.RequestObjectReady;
                    }
                }
                else if (client.State == TinyWebClientConnection.States.RequestObjectReady)
                {
                    var requestObject = client.RequestObjectBuilder.Build();
                    if (hostContainers.TryGetValue(requestObject.Host, out var hostContainer) ||
                        hostContainers.TryGetValue(string.Empty, out hostContainer))
                    {
                        // When using callable, if it find, it will build response object and return != null
                        var callable = hostContainer.RoutingService.FindRoute(requestObject.Url);
                        if (callable != null)
                        {
                            await CallByMethod(callable, requestObject.Method, requestObject, client.ResponseObjectBuilder);
                            client.State = NextState(client.State);

                        }
                        else
                        {
                            StandardResponseBuilderHelpers.NotFound(client.ResponseObjectBuilder);
                            client.State = TinyWebClientConnection.States.ResponseObjectReady;
                        }
                    }
                    else
                    {
                        //unknown host
                        StandardResponseBuilderHelpers.NotFound(client.ResponseObjectBuilder);
                        client.State = TinyWebClientConnection.States.ResponseObjectReady;
                    }
                }
                else if (client.State == TinyWebClientConnection.States.CallingResource)
                {
                    client.State = NextState(client.State);
                }
                else if (client.State == TinyWebClientConnection.States.CallingResourceReady)
                {
                    StandardResponseBuilderHelpers.Ok(client.ResponseObjectBuilder);

                    client.State = NextState(client.State);
                }
                else if (client.State == TinyWebClientConnection.States.ResponseObjectReady)
                {
                    await client.ProtocolHandler.SendResponse(client.TcpClient, client.ResponseObjectBuilder, client.ProtocolHandlerData);

                    client.State = NextState(client.State);
                }
                else if (client.State == TinyWebClientConnection.States.ReadyToClose)
                {
                    ReleaseResources(client);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex, "Error processing client connection");
                client.TcpClient.Close();
                return null;
            }

            return client;
        }

        private async Task CallByMethod(ICallableResource callable, HttpMethod method, HttpRequest request, IHttpResponseBuilder responseObjectBuilder)
        {
            try
            {
                if (method == HttpMethod.Get)
                {
                    await callable.OnGet(request, responseObjectBuilder);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex, "Error executing resource");
            }
        }

        private void ReleaseResources(TinyWebClientConnection client)
        {
            CloseConnection(client.TcpClient);

        }

        private void CloseConnection(TcpClient tcpClient)
        {
            tcpClient.Close();
        }

        private async void ClientConnectionListeningProc()
        {
            server = new(config.HttpEndPoint); // Listening from LoopBack IP with Port 80
            server.Start();
            logger.LogInformation("Server has started on {binding}.", config.HttpEndPoint);
            while (running)
            {
                try
                {
                    TcpClient client = await server.AcceptTcpClientAsync(acceptCancellationToken);
                    HandleNewClientConnection(client);
                } catch (Exception ex)
                {
                    logger.LogError(ex, null);
                }
            }
        }
        // handle a client when connection is established
        private void HandleNewClientConnection(TcpClient tcpClient)
        {
            var client = new TinyWebClientConnection(nextClientId++,
                tcpClient,
                protocolHandlerFactory.Create(ProtocolHandlerFactory.HTTP11),
                TinyWebClientConnection.States.Pending);
            waitingClients.Enqueue(client);
            logger.LogInformation("New client connected");
            clientConnectionWaitHandle.Set(); // allow proceed

        }

        public void Stop()
        {
            running = false;
            acceptCancellationTokenSource.Cancel();
            server?.Stop();

            Task.Delay(5000).Wait();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                if (running)
                {
                    Stop();
                }

                disposed = true;
            }
        }
        #endregion

        private static TinyWebClientConnection.States NextState(TinyWebClientConnection.States state) => state switch
        {
            TinyWebClientConnection.States.Pending => TinyWebClientConnection.States.BuildingRequestObject,
            TinyWebClientConnection.States.BuildingRequestObject => TinyWebClientConnection.States.RequestObjectReady,
            TinyWebClientConnection.States.RequestObjectReady => TinyWebClientConnection.States.CallingResource,
            TinyWebClientConnection.States.CallingResource => TinyWebClientConnection.States.CallingResourceReady,
            TinyWebClientConnection.States.CallingResourceReady => TinyWebClientConnection.States.ResponseObjectReady,
            TinyWebClientConnection.States.ResponseObjectReady => TinyWebClientConnection.States.ReadyToClose,
            _ => throw new InvalidOperationException()
        };
    }
}
