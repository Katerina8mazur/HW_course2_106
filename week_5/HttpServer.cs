using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace HttpServer_1
{
    public class HttpServer: IDisposable
    {

        public ServerStatus Status = ServerStatus.Stop;
        private ServerSetting serverSetting;

        private readonly HttpListener listener;

        public HttpServer()
        {
            listener = new HttpListener();
        }

        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }

            serverSetting = JsonSerializer.Deserialize<ServerSetting>(File.ReadAllBytes("./settings.json"));

            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://localhost:{serverSetting.Port}/");

            Console.WriteLine("Запуск сервера...");
            listener.Start();

            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Start;

            Listening(); 
        }

        public void Stop()
        {
            if (Status == ServerStatus.Stop) return;

            Console.WriteLine("Остановка сервера...");
            listener.Stop();

            Status = ServerStatus.Stop;
            Console.WriteLine("Сервер остановлен");
        }


        private void Listening() {
            
                listener.BeginGetContext(new AsyncCallback(ListenerCallBack), listener);
        }



        private void ListenerCallBack(IAsyncResult result)
        {
            try
            {
                if (listener.IsListening)
                {
                    //метод GetContext блокирует текущий поток, ожидая получение запроса
                    var httpContext = listener.EndGetContext(result);


                    HttpListenerRequest request = httpContext.Request;

                    //получаeм объект ответа
                    HttpListenerResponse response = httpContext.Response;

                    byte[] buffer;


                    string path = httpContext.Request.RawUrl.Replace("%20", " ");

                    string contentType;
                    if (Directory.Exists(serverSetting.Path))
                    {

                        buffer = FileLoader.GetFile(serverSetting.Path, path, out contentType);

                    }
                    else
                    {
                        string err = $"Directory '{serverSetting.Path}' not found";
                        contentType = "plain";
                        buffer = Encoding.UTF8.GetBytes(err);
                    }


                    if (buffer == null)
                    {
                        contentType = "plain";

                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string err = "404 - not found";

                        buffer = Encoding.UTF8.GetBytes(err);
                    }

                    response.Headers.Set("Content-Type", $"text/{contentType}");

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    //закрываем поток
                    output.Close();

                    Listening();
                }
            }
            catch (Exception e)
            {
                Stop();
                Console.WriteLine(e.ToString());
            }
        }

        public void Dispose()
        {
            Stop();
        }
     }
 }

