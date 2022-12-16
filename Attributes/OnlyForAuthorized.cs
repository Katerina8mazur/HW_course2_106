using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Attributes
{
    internal class OnlyForAuthorized: Attribute
    {
        public bool NeedAccountId;

        public OnlyForAuthorized(bool needAccountId = false)
        {
            NeedAccountId = needAccountId;
        }
    }
}
