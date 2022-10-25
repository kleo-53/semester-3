namespace FTP.Tests;

using FTPClient;
using FTPServer;
using NUnit.Framework;
using System.IO;
using System.Linq;

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
        pathToFile = Path.GetFullPath("../../../../FTP.Tests/tests/TestRequest.txt");
        pathToCorrectFile = Path.GetFullPath("../../../../FTP.Tests/CorrectResultOfTest.txt");
        pathToResultFile = Path.GetFullPath("../../../../FTP.Tests/tests/FTP.Tests.csproj");
        server = new Server(port);
        client = new Client(port, pathToFile, Path.GetFullPath("../../../../FTP.Tests/tests"));
    }

    [Test]
    public async void FileDownloadingTest()
    {
        if (File.Exists(pathToResultFile))
        {
            File.Delete(pathToResultFile);
        }
        try
        {
            client.Start();
            server.Start();
        }
        catch
        {
            Assert.Fail();
        }
        finally
        {
            Assert.IsTrue(File.Exists(pathToResultFile));
            Assert.IsTrue(File.ReadAllBytes(pathToCorrectFile).SequenceEqual(File.ReadAllBytes(pathToResultFile)));
        }
    }
}
