namespace MyPriorityQueue.Tests;

using MyPriorityQueue;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

public class Tests
{
    [Test]
    public void EnqueueSizeAndDequeueFromOneThread()
    {
        var priorityQueue = new PriorityQueue<int>();
        priorityQueue.Enqueue(1231, -1);
        priorityQueue.Enqueue(1231, 1);
        Assert.AreEqual(2, priorityQueue.Size);
        priorityQueue.Enqueue(1, 23);
        Assert.AreEqual(3, priorityQueue.Size);
        Assert.AreEqual(1, priorityQueue.Dequeue());
        Assert.AreEqual(2, priorityQueue.Size);
        priorityQueue.Enqueue(12, 1);
        Assert.AreEqual(3, priorityQueue.Size);
        Assert.AreEqual(1231, priorityQueue.Dequeue());
        Assert.AreEqual(2, priorityQueue.Size);
        Assert.AreEqual(12, priorityQueue.Dequeue());
        Assert.AreEqual(1, priorityQueue.Size);
        Assert.AreEqual(1231, priorityQueue.Dequeue());
        Assert.AreEqual(0, priorityQueue.Size);
    }

    [Test]
    public void EnqueueFromDifferentThreads()
    {
        var priorityQueue = new PriorityQueue<int>();
        var threads = new Thread[3];
        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                priorityQueue.Enqueue(localI, 1);
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
        Assert.AreEqual(3, priorityQueue.Size);
    }

    [Test]
    public void DequeueFromDifferentThreadsAndSavingFIFO()
    {
        var priorityQueue = new PriorityQueue<string>();
        priorityQueue.Enqueue("true", 1);
        priorityQueue.Enqueue("false", 1);
        priorityQueue.Enqueue("abobus", 1);
        var values = new string[3];
        var threads = new Thread[3];
        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                values[localI] = priorityQueue.Dequeue();
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
        var correctValues = new string[3];
        correctValues[0] = "true";
        correctValues[1] = "false";
        correctValues[2] = "abobus";
        Assert.AreEqual(correctValues, values);
        Assert.AreEqual(0, priorityQueue.Size);
    }

    [Test]
    public void EnqueueAndDequeueFromDifferentThreadsWithoutExceptions()
    {
        var priorityQueue = new PriorityQueue<string>();

        var threads = new Thread[2];
        threads[0] = new Thread(() =>
        {
            priorityQueue.Dequeue();
        });
        threads[1] = new Thread(() =>
        {
            priorityQueue.Enqueue("ololo", 131);
        });
        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
        Assert.AreEqual(0, priorityQueue.Size);
    }
}
