using HttpServer_1;

var httpServer = new HttpServer();
httpServer.Start();

while (true)
{
    CheckInput(Console.ReadLine(), httpServer);
}

void CheckInput(string input, HttpServer httpServer)
{
    switch(input)
    {
        case "start":
            httpServer.Start();
            break;
        case "restart":
            httpServer.Stop();
            httpServer.Start();
            break;
        case "stop":
            httpServer.Stop();
            break;
        default:
            Console.WriteLine("Неизвестная команда");
            break;
    }    
}