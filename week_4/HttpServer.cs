using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1
{
    internal class HttpServer
    {
        HttpListener listener;
        public HttpServer()
        {
            listener = new HttpListener();

            // установка адресов прослушки
            listener.Prefixes.Add("http://localhost:2323/google/");
        }

        public void Start()
        {
            if (listener.IsListening)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            listener.Start(); // начинаем прослушивать входящие подключения

            Console.WriteLine("Сервер запущен");

            ProcessAsync();
        }

        
        
        public async Task ProcessAsync(){
            try
            {
                //метод GetContext блокирует текущий поток, ожидая получение запроса
                var context = await listener.GetContextAsync();
                var request = context.Request;

                //получаeм объект ответа
                HttpListenerResponse response = context.Response;

                // создаем ответ в виде кода html
                string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\google.html";
                string responseText = File.ReadAllText(path);

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                // получаем поток ответа и пишем в него ответ
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                //закрываем поток
                output.Close();
                ProcessAsync();
            }
            catch (Exception e)
            {
                Stop();
            }
        }

        public void Stop(){
            if (!listener.IsListening)
            {
                Console.WriteLine("Сервер уже остановлен");
                return;
            }
            //останавливаем прослушивание подключений
            listener.Stop();
            Console.WriteLine("Сервер остановлен");

        }
    }
}
