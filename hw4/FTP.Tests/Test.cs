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
    private Server server;
    private Client client;
    private int port;
    private string pathToFile;
    private string pathToCorrectFile;
    private string pathToResultFile;

    [SetUp]
    public void Setup()
    {
        port = 8888;
        pathToCorrectFile = Path.GetFullPath("../../../../FTP.Tests/tests/CorrectFile.txt");
        pathToResultFile = Path.GetFullPath("./Download/TestRequest.txt");
        pathToFile = "../../../tests/TestRequest.txt";
        server = new Server(port);
        server.Start();
        client = new Client(port, IPAddress.Parse("127.0.0.1"));
    }

    [TearDown]
    public void TearDown()
    {
        server.Stop();
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
        try
        {
            var result = await client.Start($"2 {pathToFile}");
            Assert.IsTrue(File.ReadAllBytes(pathToResultFile).SequenceEqual(File.ReadAllBytes(pathToCorrectFile)));
        }
        catch
        {
            Assert.Fail();
        }
    }
}
