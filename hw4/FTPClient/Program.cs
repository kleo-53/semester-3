using System.Net;

namespace FTPClient;

/// <summary>
/// Main class of client
/// </summary>
public class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length == 2 && int.TryParse(args[0], out var port) && port > 0 && port <= 65565)
        {
            var client = new Client(port, IPAddress.Parse(args[1]));
            Console.WriteLine("Enter 'stop' to stop client");
            while (true)
            {
                var request = Console.ReadLine()!;
                var srequest = request.Split();
                if (srequest.Length == 2 && (string.Equals(srequest[0], "1") || string.Equals(srequest[0], "2")))
                {
                    var result = await client.Start(request);
                    Console.WriteLine(result);
                }
                else if (string.Equals(request, "stop"))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Incorrect request");
                }
            }
        }
        else
        {
            Console.WriteLine("Incorrect port or IPAddress");
        }
    }
}



/*
class Program
{
    static void Main(string[] args)
    {
        var path = "../../../../FTPClient/Request.txt";
        if (args.Length == 2 && int.TryParse(args[0], out var port) && port > 0 && port <= 65565)
        {
            var client = new Client(port, IPAddress.Parse(args[1]));
            while (true)
            {
                client.Start(Console.ReadLine()!);
            }
        }
        else
        {
            throw new ArgumentException("Incorrect port.");
        }
    }
}
*/