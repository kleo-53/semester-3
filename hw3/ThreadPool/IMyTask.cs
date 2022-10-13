namespace ThreadPool;

using System;

/// <summary>
/// Interface of the Task
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IMyTask <TResult>
{
    /// <summary>
    /// Returns if the Task was complited
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Returns the result of the Task execution
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Fccepts a function that can be applied to the result of a given function
    /// </summary>
    /// <typeparam name="TNewResult">Type of the result of function</typeparam>
    /// <param name="function">Given function</param>
    /// <returns>New Task to execute</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function);
}
