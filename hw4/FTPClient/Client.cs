namespace FTPClient;

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Class of client
/// </summary>
public class Client
{
    private readonly int port;
    private readonly IPAddress iPAddress;
    private string pathToResultDirectory = "./Download/";

    /// <summary>
    /// Creates client
    /// </summary>
    /// <param name="port">Port of server</param>
    /// <param name="iPAddress"IPAddress of the client</param>
    public Client(int port, IPAddress iPAddress)
    {
        this.port = port;
        this.iPAddress = iPAddress;
    }

    /// <summary>
    /// Starts the execution of client by given request
    /// </summary>
    /// <param name="line">Given request</param>
    /// <returns>The result of work</returns>
    public async Task<string> Start(string line)
    {
        var splitLine = line.Split();
        if (string.Equals(splitLine[0], "1"))
        {
            return await List(line);
        }
        return await Get(line);
        //Console.WriteLine("File is downoloaded");
    }

    /// <summary>
    /// Gives the listing of given directory or file
    /// </summary>
    /// <param name="line">Given path to directory or file</param>
    /// <returns>String with list of the files and directories of the given directory</returns>
    public async Task<string> List(string line)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(iPAddress, port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(line);
            await writer.FlushAsync();

            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            var splitData = data!.Split();
            if (string.Equals(data, "-1"))
            {
                return "No such file or directory";
            }
            return data!;
        }
    }

    /// <summary>
    /// Downloads file by path to file
    /// </summary>
    /// <param name="line">Given path to file</param>
    /// <returns>The size and content of the file (in bytes)</returns>
    public async Task<string> Get(string line)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(iPAddress, port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(line);
            await writer.FlushAsync();

            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            var splitData = data!.Split();
            if (string.Equals(data, "-1"))
            {
                return "Such file doesn't exist";
            }

            var resultPath = pathToResultDirectory + line.Split()[1].Split('/', '\\').Last();
            if (!Directory.Exists(pathToResultDirectory))
            {
                Directory.CreateDirectory(pathToResultDirectory);
            }
            var info = new UTF8Encoding(true).GetBytes(data);
            await File.WriteAllBytesAsync(resultPath, info);
            return data;
        }
    }
}