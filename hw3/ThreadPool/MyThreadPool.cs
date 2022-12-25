namespace ThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class of the ThreadPool
/// </summary>
public class MyThreadPool
{
    private Thread[] threads;
    private readonly object threadPoolLocker = new();
    private ConcurrentQueue<Action> queue = new();
    private ManualResetEvent freeThread = new(true);
    private CancellationTokenSource cancellationTokenSource = new();
    private CancellationToken token;
    private volatile int numberOfContinuationTasks = 0;

    /// <summary>
    /// Class of the MyTask
    /// </summary>
    /// <typeparam name="TResult">Type of the result of function</typeparam>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult>? function;
        private MyThreadPool threadPool;
        private Exception? exception;
        private TResult? result;
        private CancellationToken token;
        private ManualResetEvent endOfCalculation = new(false);
        private readonly List<Action> continuationTasks = new();
        private readonly object locker = new();
        public volatile bool IsCompleted = false;

        /// <summary>
        /// Constructor of the MyTask
        /// </summary>
        /// <param name="function">Given function</param>
        /// <param name="threadPool">Thread pool, in which the program is running</param>
        /// <param name="token">The token to check status of the threat</param>
        /// <exception cref="ArgumentNullException">Exception if there are empty parametrs</exception>
        public MyTask(Func<TResult>? function, MyThreadPool threadPool, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(function);
            ArgumentNullException.ThrowIfNull(threadPool);
            this.function = function;
            this.threadPool = threadPool;
            this.token = token;
        }

        public TResult? Result
        {
            get
            {
                if (!IsCompleted)
                {
                    endOfCalculation.WaitOne();
                    token.ThrowIfCancellationRequested();
                    if (exception != null)
                    {
                        throw new AggregateException(exception.Message);
                    }
                    return result;
                }
                return result;
            }
        }

        public void Calculate()
        {
            try
            {
                if (function == null)
                {
                    result = default(TResult);
                }
                else
                {
                    result = function();
                }
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                lock (locker)
                {
                    IsCompleted = true;
                }
                function = null;
                endOfCalculation.Set();
                foreach (var continuation in continuationTasks)
                {
                    threadPool.Enqueue(continuation);
                    Interlocked.Decrement(ref threadPool.numberOfContinuationTasks);
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
        {
            ArgumentNullException.ThrowIfNull(function);
            token.ThrowIfCancellationRequested();
            lock (locker)
            {
                if (IsCompleted)
                {
                    return threadPool.Enqueue(() => function(result));
                }
            }
            lock (threadPool.threadPoolLocker)
            {
                token.ThrowIfCancellationRequested();
                var newTask = new MyTask<TNewResult>(() => function(result), threadPool, token);
                continuationTasks.Add(newTask.Calculate);
                Interlocked.Increment(ref threadPool.numberOfContinuationTasks);
                return newTask;
            }
        }
    }

    /// <summary>
    /// Constructor of the ThreadPool
    /// </summary>
    /// <param name="quantity">The quantity of threads</param>
    public MyThreadPool(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Number of threads must be > 0!");
        }
        threads = new Thread[quantity];
        token = cancellationTokenSource.Token;

        for (int i = 0; i < quantity; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                while (!token.IsCancellationRequested || !queue.IsEmpty || numberOfContinuationTasks > 0)
                {
                    if (queue.TryDequeue(out var action))
                    {
                        action.Invoke();
                    }
                    else
                    {
                        freeThread.WaitOne();
                        if (queue.Count > 1 || token.IsCancellationRequested)
                        {
                            freeThread.Set();
                        }
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
    /// <returns>Added MyTask</returns>
    public IMyTask<TResult> Enqueue<TResult>(Func<TResult> function)
    {
        token.ThrowIfCancellationRequested();
        lock (threadPoolLocker)
        {
            token.ThrowIfCancellationRequested();
            var newTask = new MyTask<TResult>(function, this, token);
            queue.Enqueue(newTask.Calculate);
            freeThread.Set();
            return newTask;
        }
    }

    /// <summary>
    /// Adds the action to the threads queue
    /// </summary>
    /// <param name="action">Given action</param>
    public void Enqueue(Action action)
    {
        token.ThrowIfCancellationRequested();
        lock (threadPoolLocker)
        {
            token.ThrowIfCancellationRequested();
            queue.Enqueue(action);
            freeThread.Set();
        }
    }

    /// <summary>
    /// Interrupts the thread pool
    /// </summary>
    public void Shutdown()
    {
        lock (threadPoolLocker)
        {
            cancellationTokenSource.Cancel();
        }
        freeThread.Set();
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
