using FTPClient;
using System.Net;

if (args.Length == 2 && int.TryParse(args[0], out var port) && port > 0 && port <= 65565)
{
    var client = new Client(port, IPAddress.Parse(args[1]));
    Console.WriteLine("Enter 'stop' to stop client");
    while (true)
    {
        var request = Console.ReadLine()!;
        var srequest = request.Split();
        if (srequest.Length == 2)
        {
            var result = "";
            if (string.Equals(srequest[0], "1"))
            {
                result = await client.List(request);
            }
            else if (string.Equals(srequest[0], "2"))
            {
                result = await client.Get(request);
            }
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
