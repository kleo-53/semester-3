namespace FTPServer;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/// <summary>
/// Class of server
/// </summary>
public class Server
{
    private int port;
    private string currentDirectory;
    private TcpListener listener;
    private readonly CancellationTokenSource cts;

    /// <summary>
    /// Creates server running at given port
    /// </summary>
    /// <param name="port">Given port</param>
    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        this.port = port;
        currentDirectory = Directory.GetCurrentDirectory();
        cts = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts server and interacts with client
    /// </summary>
    public async void Start()
    {
        listener.Start();
        Console.WriteLine($"Listen on port {port}...");
        while (!cts.IsCancellationRequested)
        {
            Interact();
        }
        cts.Cancel();
        listener.Stop();
    }

    private async void Interact()
    {
        var socket = await listener.AcceptSocketAsync();
        _ = Task.Run(async () =>
        {
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            var writer = new StreamWriter(stream);
            Console.WriteLine($"Received data: {data}");
            try
            {
                var listOfData = data.ToString().Split(' ');
                var fullPath = Path.GetFullPath(listOfData[1]);
                if (listOfData[0] == "1")
                {
                    if (File.Exists(fullPath))
                    {
                        await writer.WriteAsync($"1 {Path.GetRelativePath(currentDirectory, fullPath)} false");
                        await writer.FlushAsync();
                    }
                    else if (Directory.Exists(fullPath))
                    {
                        string[] fileEntries = Directory.GetFiles(fullPath);
                        string[] subdirectoryEntries = Directory.GetDirectories(fullPath);

                        await writer.WriteAsync($"{fileEntries.Length + subdirectoryEntries.Length} ");

                        foreach (string fileName in fileEntries)
                        {
                            await writer.WriteAsync($"{Path.GetRelativePath(currentDirectory, fileName)} false ");
                        }

                        foreach (string subdirectory in subdirectoryEntries)
                        {
                            await writer.WriteAsync($"{Path.GetRelativePath(currentDirectory, subdirectory)} true ");
                        }
                    }
                    else
                    {
                        await writer.WriteAsync("-1");
                    }
                }
                else if (listOfData[0] == "2")
                {
                    if (File.Exists(fullPath))
                    {
                        await writer.WriteAsync($"{new FileInfo(fullPath).Length} ");
                        using (var sr = new StreamReader(fullPath))
                        {
                            await writer.WriteAsync(sr.ReadToEnd());
                        }
                    }
                    else
                    {
                        await writer.WriteAsync("-1");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await writer.FlushAsync();

            socket.Close();
        });
    }
}
