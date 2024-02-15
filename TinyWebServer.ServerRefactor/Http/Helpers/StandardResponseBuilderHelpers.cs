using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions.Http;

namespace TinyWebServer.Server.Http.Helpers
{
    /// <summary>
    /// This class set statusCode and reason phrase in IHttpResponseBuilder
    /// </summary>
    public class StandardResponseBuilderHelpers
    {
        public static void InternalServerError(IHttpResponseBuilder responseBuilder)
        {
            responseBuilder.SetStatusCode(System.Net.HttpStatusCode.InternalServerError);
            responseBuilder.SetReasonPhrase("Internal Server Error");
        }
        public static void BadRequest(IHttpResponseBuilder responseBuilder)
        {
            responseBuilder.SetStatusCode(System.Net.HttpStatusCode.BadRequest);
            responseBuilder.SetReasonPhrase("Bad Request");
        }
        public static void Ok(IHttpResponseBuilder responseBuilder)
        {
            responseBuilder.SetStatusCode(System.Net.HttpStatusCode.OK);
            responseBuilder.SetReasonPhrase("Oke");
        }
        public static void NotFound(IHttpResponseBuilder responseBuilder)
        {
            responseBuilder.SetStatusCode(System.Net.HttpStatusCode.NotFound);
            responseBuilder.SetReasonPhrase("Not Found");
        }
    }
}
