namespace FTPClient;

using System;
using System.Net.Sockets;
using System.Text;

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
        /*/const int port = 8888;
        var path = "../../../../FTPClient/Request.txt";
        using StreamReader reader = new StreamReader(Path.GetFullPath(path));
        while (true)
        {
            string? line = reader.ReadLine();
            if (line == null)
            {
                return;
            }
            using (var client = new TcpClient("localhost", port))
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream);
                //writer.WriteLine("2 ../../../../FTPClient/Program.cs");
                writer.WriteLine(line);
                writer.Flush();

                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadToEnd();
                Console.WriteLine($"Received: {data}");
                var splitLine = line.Split(' ');

                if (splitLine[0] == "2")
                {
                    var resultPath = Environment.CurrentDirectory + "/" + splitLine[1].Split('/').Last();
                    try
                    {
                        using (var fs = File.Create(resultPath))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(data);
                            fs.Write(info, 0, info.Length);
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }*/

        }
    }
}
