using HttpServer_1.Attributes;
using HttpServer_1.Controllers;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HttpServer_1
{
    public class HttpServer : IDisposable
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


        private void Listening()
        {

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

                    if (!MethodHandler(httpContext))
                        GetFile(httpContext);


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


        //------------------------------------------------

        private bool MethodHandler(HttpListenerContext _httpContext)
        {
            // объект запроса
            HttpListenerRequest request = _httpContext.Request;

            // объект ответа
            HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return false;

            // /cname/method/param1/param2
            // "/", "cname/", "method/", "param1/", "param2"

            // /cname/param1/param2
            // "/", "cname/", "param1/", "param2"



            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");

            string methodName;
            if (_httpContext.Request.Url.Segments.Length > 2)
                methodName = _httpContext.Request.Url.Segments[2].Replace("/", "");
            else
                methodName = "";



            // ["method/", "param1/", "param2"]
            string[] strParams = _httpContext.Request.Url
                                    .Segments
                                    .Skip(2)
                                    .Select(s => s.Replace("/", ""))
                                    .ToArray();

            if (_httpContext.Request.HttpMethod == "POST")
            {
                // login=arina&password=123456
                var body = GetPostData(request);
                // ["login=arina", "password=123456"]
                strParams = body.Split('&')
                    .Select(p => p.Split('=').Last())
                    .ToArray();
            }


            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c =>
                    (c.GetCustomAttribute(typeof(HttpController)) as HttpController)
                        .ControllerName.ToLower() == controllerName.ToLower());

            if (controller == null) return false;

            var method = controller.GetMethods()
                .Where(m => m.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                .FirstOrDefault(m =>
                    Regex.IsMatch(methodName.ToLower(),
                        (_httpContext.Request.HttpMethod == "GET")
                            ? (m.GetCustomAttribute(typeof(HttpGET)) as HttpGET).MethodURI
                            : (m.GetCustomAttribute(typeof(HttpPOST)) as HttpPOST).MethodURI));

            if (method == null) return false;

            // ["name", "15", "42"]
            // int Method(string name, int age, int id)

            // ["name", 15, 42]
            object[] queryParams = method.GetParameters()
                                .Take(strParams.Length)
                                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                .ToArray();

            if (method.GetCustomAttribute(typeof(OnlyForAuthorized)) != null)
            {
                var attribute = method.GetCustomAttribute(typeof(OnlyForAuthorized)) as OnlyForAuthorized;
                var cookie = request.Cookies.FirstOrDefault(c => c.Name == "SessionId");
                if (cookie == null || !SessionManager.CheckSession(Guid.Parse(cookie.Value)))
                {
                    response.StatusCode = 401;
                    byte[] bufferError = Encoding.ASCII.GetBytes("ERROR 401: Not authorized.");
                    Stream outputError = response.OutputStream;   
                    outputError.Write(bufferError, 0, bufferError.Length);

                    outputError.Close();
                    return true;
                }

                if (attribute.NeedAccountId)
                {
                    // SessionId = IsAuthorize:true
                    // Id = 2

                    var session = SessionManager.GetSession(Guid.Parse(cookie.Value));
                    var accountId = session.AccountId;
                    queryParams = queryParams
                        .Append(accountId)
                        .ToArray();
                }
            }

            var methodResponse = (MethodResponse)method.Invoke(Activator.CreateInstance(controller), queryParams);

            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(methodResponse.Response));
            response.ContentLength64 = buffer.Length;

            if (methodResponse.Cookie != null)
                response.Cookies.Add(methodResponse.Cookie);

            //if (method.Name == "Login")
            //    response.Cookies.Add(new Cookie("NAME", "VALUE"));

            // if (_httpContext.Request.HttpMethod == "POST")
            //    response.Redirect("https://store.steampowered.com/");

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            output.Close();

            return true;
        }

        private void GetFile(HttpListenerContext httpContext)
        {
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
        }

        private string GetPostData(HttpListenerRequest request)
        {
            string text;
            using (var reader = new StreamReader(request.InputStream,
                                                 request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }
    }
 }

