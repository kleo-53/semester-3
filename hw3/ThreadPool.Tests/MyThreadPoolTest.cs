namespace ThreadPool.Tests;

using NUnit.Framework;
using System;
using System.Threading;

public class MyThreadPoolTest
{
    const int SIZE = 3;
    private MyThreadPool threadPool;
    private readonly object locker = new();

    [SetUp]
    public void Setup()
    {
        threadPool = new MyThreadPool(SIZE);
    }

    [Test]
    public void EnqueueFunctionTest()
    {
        var tasks = new IMyTask<int>[5];
        for (int i = 0; i < tasks.Length; ++i)
        {
            var localI = i;
            tasks[localI] = threadPool.Enqueue(() => localI * localI + localI);
            Assert.AreEqual(localI * localI + localI, tasks[localI].Result);
        }
        threadPool.Shutdown();
    }

    [Test]
    public void ThreadPoolSizeTest()
    {
        var tasks = new IMyTask<int>[SIZE];
        var size = 0;

        for (int i = 0; i < tasks.Length; ++i)
        {
            var localI = i;
            tasks[localI] = threadPool.Enqueue(() =>
            {
                lock (locker)
                {
                    ++size;
                    return localI;
                }
            });
        }
        for (int i = 0; i < tasks.Length; ++i)
        {
            if (i != tasks[i].Result)
            {
                Assert.Fail();
            }
        }
        Assert.AreEqual(SIZE, size); 
        threadPool.Shutdown(); 
    }

    [Test]
    public void AddingAfterAfterShutdownThrowsExceptionTest()
    {
        var task = threadPool.Enqueue(() => 1);
        threadPool.Shutdown();
        Assert.Throws<AggregateException>(() => task.ContinueWith(x => x + 10));
        Assert.Throws<AggregateException>(() => threadPool.Enqueue(() => "qweqe"));

    }
}
