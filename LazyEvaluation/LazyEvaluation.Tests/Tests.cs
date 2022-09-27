namespace LazyEvaluation.Tests;

using NUnit.Framework;
using System.Threading;

public class Tests
{
    [Test]
    public void LazyGetFunctionTest()
    {
        int testValue = 5;
        Lazy<int> lazy = new Lazy<int>(() => testValue * testValue - 234);
        for (int i = 0; i < 1000; ++i)
        {
            Assert.AreEqual(-209, lazy.Get());
        }
    }

    [Test]
    public void LazyThreadsGetFunctionTest()
    {
        int testValue = 5;
        LazyThreads<int> lazyThreads = new LazyThreads<int>(() => testValue * testValue - 234);
        var threads = new Thread[System.Environment.ProcessorCount];
        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() =>
            {
                Assert.AreEqual(-209, lazyThreads.Get());

            });
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    [Test]
    public void BothLazyReturnNullBySupplierTest()
    {
        Lazy<string> lazy = new Lazy<string>(() => null);
        LazyThreads<string> lazyThreads = new LazyThreads<string>(() => null);
        var threads = new Thread[System.Environment.ProcessorCount];
        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() =>
            {
                Assert.IsNull(lazyThreads.Get());

            });
            Assert.IsNull(lazy.Get());
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
