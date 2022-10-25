namespace FTPClient;

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Class of client
/// </summary>
public class Client
{
    private readonly int port;
    private readonly string pathToFile;
    private string pathToResultDirectory = Environment.CurrentDirectory;

    /// <summary>
    /// Creates client
    /// </summary>
    /// <param name="port">Port of server</param>
    /// <param name="pathToRequestFile">Path to file with requests</param>
    public Client(int port, string pathToRequestFile, string pathToResultDirectory)
    {
        this.port = port;
        pathToFile = Path.GetFullPath(pathToRequestFile);
        if (pathToResultDirectory != null)
        {
            this.pathToResultDirectory = Path.GetFullPath(pathToResultDirectory);
        }
    }

    /// <summary>
    /// Starts client
    /// </summary>
    public async void Start()
    {
        using StreamReader reader = new StreamReader(pathToFile);
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
                writer.WriteLine(line);
                writer.Flush();

                var streamReader = new StreamReader(stream);
                var data = streamReader.ReadToEnd();
                Console.WriteLine($"Received: {data}");
                var splitLine = line.Split(' ');

                if (splitLine[0] == "2")
                {
                    var resultPath = pathToResultDirectory + "/" + splitLine[1].Split('/').Last();
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
            }
        }
    }
}
