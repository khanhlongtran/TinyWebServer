using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyWebServer.Abstractions.Http;

namespace TinyWebServer.Abstractions
{
    /// <summary>
    /// Used to build the response if a resource already exists. If not found, null is returned
    /// </summary>
    public interface ICallableResource
    {
        Task OnGet(HttpRequest request, IHttpResponseBuilder responseObjectBuilder);
    }
}
