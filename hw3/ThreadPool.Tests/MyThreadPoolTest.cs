namespace ThreadPool.Tests;

using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Class with ThreadPool tests
/// </summary>
public class MyThreadPoolTest
{
    const int SIZE = 3;
    private MyThreadPool threadPool = new(SIZE);
    private Thread[] threads = new Thread[SIZE];
    private ManualResetEvent manualResetEvent = new(false);
    private ConcurrentQueue<int> results = new();

    [SetUp]
    public void Setup()
    {
        threadPool = new(SIZE);
        threads = new Thread[SIZE];
        results = new();
        manualResetEvent = new(false);
    }
    [TearDown]
    public void Teardown()
    {
        threadPool.Shutdown();
    }

    [Test]
    public void EnqueueFunctionTest()
    {
        for (var i = 0; i < SIZE; ++i)
        {
            var localI = i;
            threads[localI] = new Thread(() => results.Enqueue(threadPool.Enqueue(() => localI * localI + localI).Result));
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            if (!thread.Join(100))
            {
                Assert.Fail("Deadlock");
            }
        }
        var a = 0;
        foreach (var result in results)
        {
            Assert.AreEqual(a * a + a, result);
            a++;
        }
    }

    [Test]
    public void ThreadSafetyTest()
    {
        var task = threadPool.Enqueue(() =>
        {
            manualResetEvent.WaitOne();
            return 1;
        });

        for (var i = 0; i < SIZE; ++i)
        {
            var localI = i;
            threads[localI] = new Thread(() => results.Enqueue(task.Result));
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        manualResetEvent.Set();

        foreach (var thread in threads)
        {
            if (!thread.Join(1000))
            {
                Assert.Fail("Deadlock");
            }
        }
        Assert.AreEqual(SIZE, results.Count);

        foreach (var result in results)
        {
            Assert.AreEqual(1, result);
        }
    }

    [Test]
    public void AddingAfterAfterShutdownThrowsOperationCanceledExceptionTest()
    {
        var task = threadPool.Enqueue(() => 1);
        threadPool.Shutdown();
        Assert.Throws<OperationCanceledException>(() => task.ContinueWith(x => x + 10));
        Assert.Throws<OperationCanceledException>(() => threadPool.Enqueue(() => "qweqe"));
    }

   
}
