namespace Chat;
using System.Net;

class Program
{
        public static async Task Main(string[] args)
        {
            var port = 8888;
            if (args.Length == 1)
            {
                var server = new ChatServer(IPAddress.Any, port);
                await server.Run();
            }
            else if (args.Length == 2)
            {
                var client = new ChatClient(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
                await client.Run();
            }
        }
    }