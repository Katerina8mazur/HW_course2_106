using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Attributes
{
    internal class HttpController : Attribute
    {
        public string ControllerName;
        public HttpController(string controllerName)
        {
            ControllerName = controllerName;
        }
    }
}
