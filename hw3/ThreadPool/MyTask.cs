namespace ThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyTask<TResult> : IMyTask<TResult>
{
    private Func<TResult>? function;
    private MyThreadPool threadPool;
    private Exception? exception; 
    private TResult result;
    public bool IsInterrupted;

    public bool IsCompleted { get; private set; }

    public MyTask(Func<TResult>? function, MyThreadPool threadPool)
    {
        if (function == null || threadPool == null)
        {
            throw new ArgumentNullException("Task parameters can not be empty!");
        }
        this.function = function;
        this.threadPool = threadPool;
        IsCompleted = false;
        IsInterrupted = false;
    }

    public TResult Result
    {
        get
        {
            if (!IsCompleted)
            {
                if (exception != null)
                {
                    throw new AggregateException(exception.Message);
                }
                if (IsInterrupted)
                {
                    function = null;
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
            result = function();
            IsCompleted = true;
            function = null;
        }
        catch (Exception exception)
        {
            this.exception = exception;
        }
    }

    public void Interrupt()
    {
        IsInterrupted = true;
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
