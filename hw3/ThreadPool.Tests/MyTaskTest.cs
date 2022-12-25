namespace ThreadPool.Tests;

using NUnit.Framework;

/// <summary>
/// Class with MyTask tests
/// </summary>
public class MyTaskTest
{
    const int SIZE = 3;
    private MyThreadPool threadPool = new(SIZE);

    [SetUp]
    public void Setup()
    {
        threadPool = new(SIZE);
    }

    [Test]
    public void CorrectResultCalculationTest()
    {
        var x = 120;
        Assert.AreEqual(12321 * 2, threadPool.Enqueue(() => 12321 * 2).Result);
        Assert.AreEqual(x * x + x, threadPool.Enqueue(() => x * x + x).Result);
        Assert.AreEqual(null, threadPool.Enqueue(() => null as string).Result);
        threadPool.Shutdown();
    }

    [Test]
    public void ContinueWithWorksCorrectlyTest()
    {
        var x = 10;
        Assert.AreEqual(12321 * x, threadPool.Enqueue(() => 12321 * x).Result);
        Assert.AreEqual(12321 * x * x, threadPool.Enqueue(() => 12321 * x).ContinueWith(result => result * x).Result);
        Assert.AreEqual(null, threadPool.Enqueue(() => null as string).Result);
        Assert.AreEqual("3", threadPool.Enqueue(() => null as string).ContinueWith(result => result + 3).Result);
        threadPool.Shutdown();
    }

    [Test]
    public void ManyContinueWithWorksCorrectlyTest()
    {
        var x = 10;
        Assert.AreEqual(x, threadPool.Enqueue(() => x).Result);
        Assert.AreEqual(x * x, threadPool.Enqueue(() => x).ContinueWith(result => result * x).Result);
        var task = threadPool.Enqueue(() => x).ContinueWith(result => result * x).ContinueWith(result => result.ToString());
        Assert.AreEqual((x * x).ToString(), task.Result);
        Assert.AreEqual((x * x).ToString() + "qwerty", task.ContinueWith(result => result + "qwerty").Result);
        threadPool.Shutdown();
    }
}
