namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class of the chat client
/// </summary>
public class ChatClient
{
    private TcpClient tcpclient;
    private NetworkStream stream;
    private StreamReader streamReader;
    private StreamWriter streamWriter;

    /// <summary>
    /// Constructor of the client by IP address and port
    /// </summary>
    /// <param name="address">Given address</param>
    /// <param name="port">Given port</param>
    public ChatClient(IPAddress address, int port)
    {
        tcpclient = new TcpClient(address.ToString(), port);
        stream = tcpclient.GetStream();
        streamWriter = new StreamWriter(stream);
        streamReader = new StreamReader(stream);
    }

    /// <summary>
    /// Stsrts the client
    /// </summary>
    public async Task Run(CancellationToken cancellationToken = default)
    {
        using (cancellationToken.Register(() => Close()))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                if (!Equals(line, ""))
                    {
                    await streamWriter.WriteLineAsync(line);

                    await streamWriter.FlushAsync();
                    if (Equals(line, "exit"))
                    {
                        Close();
                    }
                }
                else
                {
                    var line2 = await streamReader.ReadToEndAsync();
                    if (Equals(line2, "exit"))
                    {
                        Close();
                    }
                    Console.WriteLine($"Received: {line2}");
                }

            }
        }
    }

    /// <summary>
    /// Sends the message to server
    /// </summary>
    public async Task SendMessage(string line)
    {
        await streamWriter.WriteLineAsync(line);
        await streamWriter.FlushAsync();
        if (Equals(line, "exit"))
        {
            Close();
        }
    }

    /// <summary>
    /// Recieves the message from server
    /// </summary>
    public async Task RecieveMessage()
    {
        //using var reader = new StreamReader(client.GetStream());
        var line = await streamReader.ReadToEndAsync();
        if (Equals(line, "exit"))
        {
            Close();
        }
        Console.WriteLine($"Received: {line}");
    }

    /// <summary>
    /// Stops the client
    /// </summary>
    public async Task Close()
    {
        streamReader.Close();
        streamWriter.Close();
        tcpclient.Close();
    }
}
