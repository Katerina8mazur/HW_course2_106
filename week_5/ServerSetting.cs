using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1
{
    public class ServerSetting
    {
        public int Port { get; set; } = 7700;

        public string Path { get; set; } = @"./site/";
    }
}
