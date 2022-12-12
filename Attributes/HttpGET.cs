using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Attributes
{
    internal class HttpGET : Attribute
    {
        public string MethodURI;
        public HttpGET(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
