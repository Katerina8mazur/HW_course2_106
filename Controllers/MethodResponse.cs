using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Controllers
{
    internal class MethodResponse
    {
        public object? Response { get; set; }
        public Cookie? Cookie { get; set; }

        public MethodResponse() { }

        protected MethodResponse(object? response, Cookie? cookie)
        {
            Response = response;
            Cookie = cookie;
        }
    }

    internal class MethodResponse<T> : MethodResponse
    {
        public MethodResponse(T? response, Cookie? cookie = null) : base(response, cookie) { }
    }
}