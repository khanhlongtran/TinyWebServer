using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions;
using TinyWebServer.Abstractions.Http;
using TinyWebServer.Server.Http;

namespace TinyWebServer.Server
{
    /// <summary>
    /// This class is used to set default setting when the TCP protocol is established
    /// It's easy to see that for the client to communicate with the server, the first object is connection (TCP protocol), then the client sends a request, the server sends a response.
    /// </summary>
    public class TinyWebClientConnection
    {
        public int Id { get; set; }
        // Lấy data từ client (byte[] or stream)
        public TcpClient TcpClient { get; }
        // Chịu trách nhiệm parse data
        public IProtocolHandler ProtocolHandler { get; }
        // Hold kết quả 
        public ProtocolHandlerData ProtocolHandlerData { get; set; }
        // Sử dụng state để đánh giá từng quá trình 
        public States State { get; set; }
        // Xây dựng request sau khi parse
        public IHttpRequestBuilder RequestObjectBuilder { get; set; }
        // Xây dựng response để gửi đi client 
        public IHttpResponseBuilder ResponseObjectBuilder { get; set; }

        public TinyWebClientConnection(int id, TcpClient tcpClient, IProtocolHandler protocolHandler, States initState)
        {
            Id = id;
            TcpClient = tcpClient;
            ProtocolHandler = protocolHandler;
            this.State = initState;
            ProtocolHandlerData = new();
            RequestObjectBuilder = new HttpWebRequestBuilder();
            ResponseObjectBuilder = new HttpWebResponseBuilder();
        }

        /// <summary>
        /// Once TCP is set up, when the client sends a request to the server and when the server generates a response sent back to the client, the following states may be included:
        /// </summary>
        public enum States
        {
            //The initial state where the client is preparing to make a request to the server.
            //At this stage, the client may be waiting for some trigger or event to initiate the request.
            Pending,

            //The client is actively constructing the HTTP request object that will be sent to the server.
            //This involves assembling the necessary headers, body, and other components of the request.
            BuildingRequestObject,

            // The client has successfully built the request object and is ready to send it to the server.
            // Any necessary data or parameters have been set, and the client is in a state of readiness to initiate the request.
            RequestObjectReady,

            //The client is in the process of making the actual network call to the server to send the prepared request.
            //This may involve establishing a connection and transmitting the request over the network.
            CallingResource,
            //The client has successfully made the network call, and it is now waiting for the server to process the request and send a response.
            //This state indicates that the client has done its part in sending the request.
            CallingResourceReady,
            //The client has received the response from the server.
            // At this stage, the client may start processing the received data, including handling the response headers and body.
            ResponseObjectReady,

            //The client has completed the interaction with the server and is ready to close the connection or take further actions.
            ReadyToClose
        }

    }


}
