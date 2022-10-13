namespace ThreadPool;

using System;

/// <summary>
/// Class of the Task
/// </summary>
/// <typeparam name="TResult">Type of the result of function</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private Func<TResult>? function;
    private MyThreadPool threadPool;
    private Exception? exception; 
    private TResult result;
    private CancellationToken token;
    private ManualResetEvent reset;

    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Constructor of the Task
    /// </summary>
    /// <param name="function">Given function</param>
    /// <param name="threadPool">Thread pool, in which the program is running</param>
    /// <param name="token">The token to check status of the threat</param>
    /// <exception cref="ArgumentNullException">Exception if there are empty parametrs</exception>
    public MyTask(Func<TResult>? function, MyThreadPool threadPool, CancellationToken token)
    {
        if (function == null || threadPool == null)
        {
            throw new ArgumentNullException("Task parameters can not be empty!");
        }
        this.function = function;
        this.threadPool = threadPool;
        this.token = token;
        IsCompleted = false; 
        reset = new ManualResetEvent(false);
    }

    public TResult Result
    {
        get
        {
            if (!IsCompleted)
            {
                reset.WaitOne();
                if (token.IsCancellationRequested)
                {
                    reset.Set();
                    token.ThrowIfCancellationRequested();
                }
                if (exception != null)
                {
                    throw new AggregateException(exception.Message);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Calculates the Task
    /// </summary>
    public void Calculate()
    {
        try
        {
            result = function();
            IsCompleted = true;
            function = null;
            reset.Set();
        }
        catch (Exception exception)
        {
            this.exception = exception;
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException();
        }
        return threadPool.Enqueue(() => function(Result));
    }
}
