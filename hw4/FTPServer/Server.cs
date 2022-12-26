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
    private TcpListener listener;
    private CancellationTokenSource cts = new();
    private List<Task> listOfRequests = new();

    /// <summary>
    /// Creates server running at given port
    /// </summary>
    /// <param name="port">Given port</param>
    public Server(int port)
    {
        listener = new(IPAddress.Any, port);
        this.port = port;
        cts = new();
    }

    /// <summary>
    /// Starts server and interacts with client
    /// </summary>
    public async void Start()
    {
        listener.Start();
        Console.WriteLine($"Server started on port {port}...");
        while (!cts.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            listOfRequests.Add(Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                var splitData = data?.Split();
                var writer = new StreamWriter(stream);
                if (string.Equals(splitData![0], "1"))
                {
                    await List(splitData[1], writer);
                }
                else if (string.Equals(splitData[0], "2"))
                {
                    await Get(splitData[1], writer);
                }
                socket.Close();
            }));
        }
        await Task.WhenAll(listOfRequests);
        listener.Stop();
    }

    private async Task List(string path, StreamWriter writer)
    {
        if (File.Exists(path))
        {
            await writer.WriteAsync($"1 {Path.GetRelativePath(Directory.GetCurrentDirectory(), path)} false");
            await writer.FlushAsync();
        }
        else if (Directory.Exists(path))
        {
            string[] fileEntries = Directory.GetFiles(path);
            string[] subdirectoryEntries = Directory.GetDirectories(path);

            await writer.WriteAsync($"{fileEntries.Length + subdirectoryEntries.Length} ");

            foreach (string fileName in fileEntries)
            {
                await writer.WriteAsync($"{Path.GetRelativePath(Directory.GetCurrentDirectory(), fileName)} false ");
            }

            foreach (string subdirectory in subdirectoryEntries)
            {
                await writer.WriteAsync($"{Path.GetRelativePath(Directory.GetCurrentDirectory(), subdirectory)} true ");
            }
        }
        else
        {
            await writer.WriteAsync("-1");
        }
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    private async Task Get(string path, StreamWriter writer)
    {
        if (File.Exists(path))
        {
            await writer.WriteAsync($"{new FileInfo(path).Length} ");
            using (var sr = new StreamReader(path))
            {
                await writer.WriteAsync(sr.ReadToEnd());
            }
        }
        else
        {
            await writer.WriteAsync("-1");
        }
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    /// <summary>
    /// Stops server
    /// </summary>
    public void Stop()
    {
        cts.Cancel();
    }
}