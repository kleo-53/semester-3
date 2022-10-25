namespace FTPServer;

using System;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 1 && int.TryParse(args[0], out var port) && port > 0 && port <= 65535)
        {
            var server = new Server(port);
            server.Start();
        }
        else
        {
            throw new ArgumentException("Incorrect port.");
        }
    }
}
