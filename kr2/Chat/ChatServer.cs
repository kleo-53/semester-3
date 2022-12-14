namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class of server
/// </summary>
public class ChatServer
{
    private TcpListener tcplistener;

    /// <summary>
    /// Constructor of the server by and port
    /// </summary>
    /// <param name="address">Any address</param>
    /// <param name="port">Given port</param>
    public ChatServer(IPAddress address, int port)
    {
        tcplistener = new TcpListener(address, port);
        tcplistener.Start();
    }

    /// <summary>
    /// Stsrts the server
    /// </summary>
    public async Task Run(CancellationToken cancellationToken = default)
    {
        using (cancellationToken.Register(() => tcplistener.Stop()))
        {
            var tasks = new Queue<Task>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await tcplistener.AcceptTcpClientAsync();
                var line = Console.ReadLine();
                if (!Equals(line, ""))
                {
                    using var streamWriter = new StreamWriter(tcpClient.GetStream());
                    await streamWriter.WriteLineAsync(line);
                    await streamWriter.FlushAsync();
                    if (Equals(line, "exit"))
                    {
                        tcpClient.Close();
                    }
                }
                else
                {
                    using var streamReader = new StreamReader(tcpClient.GetStream());
                    var line2 = await streamReader.ReadToEndAsync();
                    if (Equals(line2, "exit"))
                    {
                        tcpClient.Close();
                    }
                    Console.WriteLine($"Received: {line2}");

                    tcpClient.Close();
                }
            }
        }
    }

    /// <summary>
    /// Sends the message to client
    /// </summary>
    public async Task SendMessage(TcpClient tcpClient, string line)
    {
        using var streamWriter = new StreamWriter(tcpClient.GetStream());
        await streamWriter.WriteLineAsync(line);
        await streamWriter.FlushAsync();
        if (Equals(line, "exit"))
        {
            tcpClient.Close();
        }
    }
    

    /// <summary>
    /// Recieves the message from client
    /// </summary>
    public async Task RecieveMessage(TcpClient tcpClient)
    {
        using var streamReader = new StreamReader(tcpClient.GetStream());
        var line = await streamReader.ReadToEndAsync();
        if (Equals(line, "exit"))
        {
            tcpClient.Close();
        }
        Console.WriteLine($"Received: {line}");

        tcpClient.Close();
    }
}
