namespace FTP.Tests;

using FTPClient;
using FTPServer;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class Tests
{
    private Server server = new(8888);
    private Client client = new(8888, IPAddress.Parse("127.0.0.1"));
    private int port;
    private string pathToFile = "../../../tests/TestRequest.txt";
    private string pathToCorrectFile = Path.GetFullPath("../../../../FTP.Tests/tests/CorrectFile.txt");
    private string pathToResultFile = Path.GetFullPath("./Download/TestRequest.txt");
    private Task serverTask = new(() => { });

    [SetUp]
    public void Setup()
    {
        port = 8888;
        server = new Server(port);
        serverTask = server.Start();
        client = new Client(port, IPAddress.Parse("127.0.0.1"));
    }

    [TearDown]
    public async void TearDown()
    {
        server.Stop();
        await serverTask;
    }

    [Test]
    public async Task FileListingTest()
    {
        var result = await client.Start($"1 ../../../../FTP.Tests/tests");
        Assert.AreEqual(result, "3 ..\\..\\..\\tests\\CorrectFile.txt false ..\\..\\..\\tests\\TestRequest.txt false ..\\..\\..\\tests\\EmptyDirectory true ");
        result = await client.Start("1 ../../../../FTP.Tests/aboba.txt");
        Assert.AreEqual(result, "No such file or directory");
        result = await client.Start("1 ../../../../FTP.Tests/FTP.Tests.csproj");
        Assert.AreEqual(result, "1 ..\\..\\..\\FTP.Tests.csproj false");
    }

    [Test]
    public async Task FileDownloadingTest()
    {
        if (File.Exists(pathToResultFile))
        {
            File.Delete(pathToResultFile);
        }
        var result = await client.Start($"2 {pathToFile}");
        Assert.IsTrue(File.ReadAllBytes(pathToResultFile).SequenceEqual(File.ReadAllBytes(pathToCorrectFile)));
    }
}
