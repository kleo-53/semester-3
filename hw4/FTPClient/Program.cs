namespace FTPClient;

class Program
{
    static void Main(string[] args)
    {
        var path = "../../../../FTPClient/Request.txt";
        if (args.Length == 1 && int.TryParse(args[0], out var port) && port > 0 && port <= 35565)
        {
            var client = new Client(port, Path.GetFullPath(path), null);
            client.Start();
        }
        else
        {
            throw new ArgumentException("Incorrect port.");
        }
    }
}
