namespace ThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyThreadPool
{
    public bool IsComplited { get; private set; }

    private System.Threading.Thread[] threads;
    private readonly object locker = new();
    private static volatile bool flag = true;
    private Queue<Action> queue;

    public MyThreadPool(int count)
    {
        threads = new Thread[count];
        queue = new Queue<Action>();
        for (int i = 0; i < count; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                while (flag)
                {
                    if (queue.TryDequeue(out var action))
                    {
                        action.Invoke();
                    }
                }
            });
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        Console.ReadLine();
        flag = false;
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    public IMyTask<TResult> Enqueue<TResult>(Func<TResult> function)
    {
        if (flag)
        {
            lock (locker)
            {
                if (flag)
                {
                    var newTask = new MyTask<TResult>(function, this);

                    queue.Enqueue(newTask.Calculate);
                    return new MyTask<TResult>(newTask);
                }
            }
        }
        throw new InvalidOperationException("Pool is stopped");
    }

    public void Interruption()
    {
        flag = false;
        lock (locker)
        {
            foreach (var threadItem in threads) // is it necessary ?
            {
                threadItem.Join();
            }
        }
    }

    public static void Beginning(int n)
    {
        forks = new Object[n];
        for (int i = 0; i < n; i++)
        {
            forks[i] = new Object();
        }

        var threads = new Thread[n];
        for (int i = 0; i < n; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (int j = localI; j < n; j += threads.Length)
                {
                    Philosopher(localI);
                }
            });
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        Console.ReadLine();
        flag = false;
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
