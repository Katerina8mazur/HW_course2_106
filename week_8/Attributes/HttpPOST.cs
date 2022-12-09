using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Attributes
{
    internal class HttpPOST : Attribute
    {
        public string MethodURI;
        public HttpPOST(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}
