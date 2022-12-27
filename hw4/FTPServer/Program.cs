using FTPServer;

if (args.Length == 1 && int.TryParse(args[0], out var port) && port > 0 && port <= 65535)
{
    var server = new Server(port);
    var serverTask = server.Start();
    Console.WriteLine("Enter 'stop' to stop server");

    while (true)
    {
        var request = Console.ReadLine()!;
        if (string.Equals(request, "stop"))
        {
            server.Stop();
            await serverTask;
            break;
        }
    }
}
else
{
    Console.WriteLine("Incorrect port");
}