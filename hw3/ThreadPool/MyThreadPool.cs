namespace ThreadPool;

using System;
using System.Collections.Generic;

/// <summary>
/// Class of the thread pool
/// </summary>
public class MyThreadPool
{
    private Thread[] threads;
    private readonly object locker = new();
    private Queue<Action> queue;
    private AutoResetEvent reset;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken token;

    /// <summary>
    /// Constructor of the ThreadPool
    /// </summary>
    /// <param name="quantity">The quantity of threads</param>
    public MyThreadPool(int quantity)
    {
        threads = new Thread[quantity];
        queue = new Queue<Action>();
        reset = new AutoResetEvent(true);
        cancellationTokenSource = new CancellationTokenSource();
        token = cancellationTokenSource.Token;

        for (int i = 0; i < quantity; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (queue.TryDequeue(out var action))
                    {
                        action.Invoke();
                    }
                    else
                    {
                        reset.WaitOne();
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    /// <summary>
    /// Adds the function to the threads queue
    /// </summary>
    /// <typeparam name="TResult">Type of the result of function</typeparam>
    /// <param name="function">Given function</param>
    /// <returns>Added Task</returns>
    /// <exception cref="AggregateException">Exception if thread was stopped</exception>
    public IMyTask<TResult> Enqueue<TResult>(Func<TResult> function)
    {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
            lock (locker)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    var newTask = new MyTask<TResult>(function, this, token);
                    queue.Enqueue(newTask.Calculate);
                    reset.Set();
                    return newTask;
                }
            }
        }
        throw new AggregateException("Thread was stopped");
    }

    /// <summary>
    /// Interrupts the thread pool
    /// </summary>
    public void Shutdown()
    {
        lock (locker)
        {
            cancellationTokenSource.Cancel();
        }
        for (int i = 0; i < threads.Length; ++i)
        {
            reset.Set();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
