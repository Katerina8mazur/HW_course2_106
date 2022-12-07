using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1
{
    public class FileLoader
    {
        public static byte[] GetFile(string sitePath, string rawUrl, out string contentType)
        {
            byte[] buffer = null;
            var filePath = sitePath + rawUrl;

            if (Directory.Exists(filePath))
            {
                //Каталог
                filePath = filePath + "/index.html";
                if (File.Exists(filePath))
                {
                    buffer = File.ReadAllBytes(filePath);
                }
            }
            else if (File.Exists(filePath))
            {
                //Файл
                buffer = File.ReadAllBytes(filePath);
            }
            contentType = filePath.Split('.').Last();
            return buffer;
        }
    }
}
