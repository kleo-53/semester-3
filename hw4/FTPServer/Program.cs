namespace FTPServer;

using System;

/// <summary>
/// Main class of server
/// </summary>
public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 1 && int.TryParse(args[0], out var port) && port > 0 && port <= 65535)
        {
            var server = new Server(port);
            server.Start();
            Console.WriteLine("Enter 'stop' to stop server");

            while (true)
            {
                var request = Console.ReadLine()!;
                if (string.Equals(request, "stop"))
                {
                    server.Stop();
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Incorrect port");
        }
    }
}